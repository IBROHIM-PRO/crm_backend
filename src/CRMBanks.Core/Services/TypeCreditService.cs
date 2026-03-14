using AutoMapper;
using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Core.Services;

public class TypeCreditService(IRepository<TypeCredit> typeCreditRepository, IMapper mapper) : ITypeCreditService
{
    public async Task<IEnumerable<TypeCreditDto>> GetAllAsync()
    {
        var typeCredits = await typeCreditRepository.GetQuery()
            .Include(tc => tc.Credits)
            .Include(tc => tc.TypeProducts)
            .ToListAsync();
        return mapper.Map<IEnumerable<TypeCreditDto>>(typeCredits);
    }

    public async Task<PagedResponseDto<TypeCreditDto>> GetPagedAsync(PagedRequestDto request)
    {
        var query = typeCreditRepository.GetQuery()
            .Include(tc => tc.Credits)
            .Include(tc => tc.TypeProducts)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(tc => tc.Name.Contains(request.SearchTerm));
        }

        var totalCount = await query.CountAsync();
        var typeCredits = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var mappedTypeCredits = mapper.Map<IEnumerable<TypeCreditDto>>(typeCredits);

        return new PagedResponseDto<TypeCreditDto>
        {
            Data = mappedTypeCredits,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<TypeCreditDto?> GetByIdAsync(int id)
    {
        var typeCredit = await typeCreditRepository.GetQuery()
            .Include(tc => tc.Credits)
            .Include(tc => tc.TypeProducts)
            .FirstOrDefaultAsync(tc => tc.Id == id);
        return typeCredit == null ? null : mapper.Map<TypeCreditDto>(typeCredit);
    }

    public async Task<TypeCreditDto> CreateAsync(TypeCreditDto dto)
    {
        var typeCredit = mapper.Map<TypeCredit>(dto);
        await typeCreditRepository.AddAsync(typeCredit);
        return mapper.Map<TypeCreditDto>(typeCredit);
    }

    public async Task<bool> UpdateAsync(int id, TypeCreditDto dto)
    {
        var typeCredit = await typeCreditRepository.GetQuery()
            .Include(tc => tc.Credits)
            .Include(tc => tc.TypeProducts)
            .FirstOrDefaultAsync(tc => tc.Id == id);
        if (typeCredit == null) return false;

        typeCredit.Name = dto.Name;
        typeCreditRepository.Update(typeCredit);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var typeCredit = await typeCreditRepository.GetIdAsync(id);
        if (typeCredit == null) return false;

        typeCreditRepository.IsDelete(typeCredit);
        return true;
    }

    public async Task<IEnumerable<SelectItemDto>> GetSelectListAsync()
    {
        return await typeCreditRepository.GetQuery()
            .Select(tc => new SelectItemDto { Value = tc.Id, Text = tc.Name })
            .ToListAsync();
    }
}
