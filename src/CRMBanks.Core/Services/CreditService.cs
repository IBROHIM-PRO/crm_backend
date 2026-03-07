using AutoMapper;
using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Core.Services;

public class CreditService(IRepository<Credit> repository, IMapper mapper) : ICreditService
{
    public async Task<IEnumerable<CreditDto>> GetAllByBankAsync(int bankId)
    {
        var credits = await repository.GetQuery()
            .Include(c => c.Bank)
            .Include(c => c.TypeCredit)
            .Where(c => c.BankId == bankId)
            .ToListAsync();
        return mapper.Map<IEnumerable<CreditDto>>(credits);
    }

    public async Task<PagedResponseDto<CreditDto>> GetPagedByBankAsync(int bankId, PagedRequestDto request)
    {
        var query = repository.GetQuery()
            .Include(c => c.Bank)
            .Include(c => c.TypeCredit)
            .Where(c => c.BankId == bankId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(c => 
                c.Name.Contains(request.SearchTerm) ||
                c.Description.Contains(request.SearchTerm) ||
                (c.TypeCredit != null && c.TypeCredit.Name.Contains(request.SearchTerm)));
        }

        var totalCount = await query.CountAsync();
        var credits = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var mappedCredits = mapper.Map<IEnumerable<CreditDto>>(credits);

        return new PagedResponseDto<CreditDto>
        {
            Data = mappedCredits,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<CreditDto?> GetByIdAsync(int id)
    {
        var credit = await repository.GetQuery()
            .Include(c => c.Bank)
            .Include(c => c.TypeCredit)
            .FirstOrDefaultAsync(c => c.Id == id);
        return credit == null ? null : mapper.Map<CreditDto>(credit);
    }

    public async Task<CreditDto> CreateAsync(CreditDto dto, int bankId)
    {
        var credit = mapper.Map<Credit>(dto);
        credit.Id = 0;
        credit.BankId = bankId;
        var createdCredit = await repository.AddAsync(credit);
        return mapper.Map<CreditDto>(createdCredit);
    }

    public async Task<bool> UpdateAsync(int id, CreditDto dto)
    {
        var credit = await repository.GetIdAsync(id);
        if (credit == null) return false;

        credit.Name = dto.Name;
        credit.Description = dto.Description;
        credit.TypeCreditId = dto.TypeCreditId;
        credit.Foiz = dto.Foiz;
        credit.AzSum = dto.AzSum;
        credit.ToSum = dto.ToSum;
        credit.Document = dto.Document;
        credit.TypeSumId = dto.TypeSumId;
        credit.AzSana = dto.AzSana;
        credit.ToSana = dto.ToSana;
        credit.Infoprot = dto.Infoprot;

        repository.Update(credit);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var credit = await repository.GetIdAsync(id);
        if (credit == null) return false;

        repository.Remove(credit);
        await repository.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<SelectItemDto>> GetSelectListAsync(int bankId)
    {
        return await repository.GetQuery()
            .Where(c => c.BankId == bankId)
            .Select(c => new SelectItemDto { Value = c.Id, Text = c.Name })
            .ToListAsync();
    }
}
