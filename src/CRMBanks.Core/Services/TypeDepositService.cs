using AutoMapper;
using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Core.Services;

public class TypeDepositService(IRepository<TypeDeposit> typeDepositRepository, IMapper mapper) : ITypeDepositService
{
    public async Task<IEnumerable<TypeDepositDto>> GetAllAsync()
    {
        var typeDeposits = await typeDepositRepository.GetQuery()
            .Include(td => td.Deposits)
            .Include(td => td.TypeProducts)
            .ToListAsync();
        return mapper.Map<IEnumerable<TypeDepositDto>>(typeDeposits);
    }

    public async Task<PagedResponseDto<TypeDepositDto>> GetPagedAsync(PagedRequestDto request)
    {
        var query = typeDepositRepository.GetQuery()
            .Include(td => td.Deposits)
            .Include(td => td.TypeProducts)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(td => td.Name.Contains(request.SearchTerm));
        }

        var totalCount = await query.CountAsync();
        var typeDeposits = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var mappedTypeDeposits = mapper.Map<IEnumerable<TypeDepositDto>>(typeDeposits);

        return new PagedResponseDto<TypeDepositDto>
        {
            Data = mappedTypeDeposits,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<TypeDepositDto?> GetByIdAsync(int id)
    {
        var typeDeposit = await typeDepositRepository.GetQuery()
            .Include(td => td.Deposits)
            .Include(td => td.TypeProducts)
            .FirstOrDefaultAsync(td => td.Id == id);
        return typeDeposit == null ? null : mapper.Map<TypeDepositDto>(typeDeposit);
    }

    public async Task<TypeDepositDto> CreateAsync(TypeDepositDto dto)
    {
        var typeDeposit = mapper.Map<TypeDeposit>(dto);
        await typeDepositRepository.AddAsync(typeDeposit);
        return mapper.Map<TypeDepositDto>(typeDeposit);
    }

    public async Task<bool> UpdateAsync(int id, TypeDepositDto dto)
    {
        var typeDeposit = await typeDepositRepository.GetQuery()
            .Include(td => td.Deposits)
            .Include(td => td.TypeProducts)
            .FirstOrDefaultAsync(td => td.Id == id);
        if (typeDeposit == null) return false;

        typeDeposit.Name = dto.Name;
        typeDepositRepository.Update(typeDeposit);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var typeDeposit = await typeDepositRepository.GetIdAsync(id);
        if (typeDeposit == null) return false;

        typeDepositRepository.IsDelete(typeDeposit);
        return true;
    }

    public async Task<IEnumerable<SelectItemDto>> GetSelectListAsync()
    {
        return await typeDepositRepository.GetQuery()
            .Select(td => new SelectItemDto { Value = td.Id, Text = td.Name })
            .ToListAsync();
    }
}
