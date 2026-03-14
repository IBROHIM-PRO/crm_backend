using AutoMapper;
using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Core.Services;

public class TypeSumService(IRepository<TypeSum> typeSumRepository, IMapper mapper) : ITypeSumService
{
    public async Task<IEnumerable<TypeSumDto>> GetAllAsync()
    {
        var typeSums = await typeSumRepository.GetQuery()
            .Include(ts => ts.Credits)
            .Include(ts => ts.Deposits)
            .ToListAsync();
        return mapper.Map<IEnumerable<TypeSumDto>>(typeSums);
    }

    public async Task<PagedResponseDto<TypeSumDto>> GetPagedAsync(PagedRequestDto request)
    {
        var query = typeSumRepository.GetQuery()
            .Include(ts => ts.Credits)
            .Include(ts => ts.Deposits)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(ts => ts.Name.Contains(request.SearchTerm));
        }

        var totalCount = await query.CountAsync();
        var typeSums = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var mappedTypeSums = mapper.Map<IEnumerable<TypeSumDto>>(typeSums);

        return new PagedResponseDto<TypeSumDto>
        {
            Data = mappedTypeSums,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<TypeSumDto?> GetByIdAsync(int id)
    {
        var typeSum = await typeSumRepository.GetQuery()
            .Include(ts => ts.Credits)
            .Include(ts => ts.Deposits)
            .FirstOrDefaultAsync(ts => ts.Id == id);
        return typeSum == null ? null : mapper.Map<TypeSumDto>(typeSum);
    }

    public async Task<TypeSumDto> CreateAsync(TypeSumDto dto)
    {
        var typeSum = mapper.Map<TypeSum>(dto);
        await typeSumRepository.AddAsync(typeSum);
        return mapper.Map<TypeSumDto>(typeSum);
    }

    public async Task<bool> UpdateAsync(int id, TypeSumDto dto)
    {
        var typeSum = await typeSumRepository.GetQuery()
            .Include(ts => ts.Credits)
            .Include(ts => ts.Deposits)
            .FirstOrDefaultAsync(ts => ts.Id == id);
        if (typeSum == null) return false;

        typeSum.Name = dto.Name;
        typeSumRepository.Update(typeSum);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var typeSum = await typeSumRepository.GetIdAsync(id);
        if (typeSum == null) return false;

        typeSumRepository.IsDelete(typeSum);
        return true;
    }

    public async Task<IEnumerable<SelectItemDto>> GetSelectListAsync()
    {
        return await typeSumRepository.GetQuery()
            .Select(ts => new SelectItemDto { Value = ts.Id, Text = ts.Name })
            .ToListAsync();
    }
}
