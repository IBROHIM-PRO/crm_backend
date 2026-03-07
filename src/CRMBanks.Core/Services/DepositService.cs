using AutoMapper;
using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Core.Services;

public class DepositService(IRepository<Deposit> repository, IMapper mapper) : IDepositService
{
    public async Task<IEnumerable<DepositDto>> GetAllByBankAsync(int bankId)
    {
        var deposits = await repository.GetQuery()
            .Include(d => d.Bank)
            .Include(d => d.TypeDeposit)
            .Where(d => d.BankId == bankId)
            .ToListAsync();
        return mapper.Map<IEnumerable<DepositDto>>(deposits);
    }

    public async Task<PagedResponseDto<DepositDto>> GetPagedByBankAsync(int bankId, PagedRequestDto request)
    {
        var query = repository.GetQuery()
            .Include(d => d.Bank)
            .Include(d => d.TypeDeposit)
            .Where(d => d.BankId == bankId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(d => 
                d.Name.Contains(request.SearchTerm) ||
                d.Description.Contains(request.SearchTerm) ||
                (d.TypeDeposit != null && d.TypeDeposit.Name.Contains(request.SearchTerm)));
        }

        var totalCount = await query.CountAsync();
        var deposits = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var mappedDeposits = mapper.Map<IEnumerable<DepositDto>>(deposits);

        return new PagedResponseDto<DepositDto>
        {
            Data = mappedDeposits,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<DepositDto?> GetByIdAsync(int id)
    {
        var deposit = await repository.GetQuery()
            .Include(d => d.Bank)
            .Include(d => d.TypeDeposit)
            .FirstOrDefaultAsync(d => d.Id == id);
        return deposit == null ? null : mapper.Map<DepositDto>(deposit);
    }

    public async Task<DepositDto> CreateAsync(DepositDto dto, int bankId)
    {
        var deposit = mapper.Map<Deposit>(dto);
        deposit.Id = 0;
        deposit.BankId = bankId;
        var createdDeposit = await repository.AddAsync(deposit);
        return mapper.Map<DepositDto>(createdDeposit);
    }

    public async Task<bool> UpdateAsync(int id, DepositDto dto)
    {
        var deposit = await repository.GetIdAsync(id);
        if (deposit == null) return false;

        deposit.Name = dto.Name;
        deposit.Description = dto.Description;
        deposit.TypeDepositId = dto.TypeDepositId;
        deposit.Foiz = dto.Foiz;
        deposit.AzSum = dto.AzSum;
        deposit.ToSum = dto.ToSum;
        deposit.Document = dto.Document;
        deposit.TypeSumId = dto.TypeSumId;
        deposit.AzSana = dto.AzSana;
        deposit.ToSana = dto.ToSana;
        deposit.Infoprot = dto.Infoprot;

        repository.Update(deposit);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var deposit = await repository.GetIdAsync(id);
        if (deposit == null) return false;

        repository.Remove(deposit);
        await repository.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<SelectItemDto>> GetSelectListAsync(int bankId)
    {
        return await repository.GetQuery()
            .Where(d => d.BankId == bankId)
            .Select(d => new SelectItemDto { Value = d.Id, Text = d.Name })
            .ToListAsync();
    }
}
