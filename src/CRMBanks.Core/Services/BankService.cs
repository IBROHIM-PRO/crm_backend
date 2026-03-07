using AutoMapper;
using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using CRMBanks.SharedKernel.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Infrastructure.Services;

public class BankService(IRepository<Bank> bankRepository, IMapper mapper) : IBankService
{
    public async Task<IEnumerable<BankDto>> GetAllAsync()
    {
        var banks = await bankRepository.GetAllAsync();
        return mapper.Map<IEnumerable<BankDto>>(banks);
    }

    public async Task<PagedResponseDto<BankDto>> GetPagedAsync(PagedRequestDto request)
    {
        var query = bankRepository.GetQuery();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(b => 
                b.Name.Contains(request.SearchTerm) ||
                b.Phone.Contains(request.SearchTerm) ||
                b.Email.Contains(request.SearchTerm) ||
                b.Address.Contains(request.SearchTerm));
        }

        var totalCount = await query.CountAsync();
        var banks = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var mappedBanks = mapper.Map<IEnumerable<BankDto>>(banks);

        return new PagedResponseDto<BankDto>
        {
            Data = mappedBanks,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<BankDto?> GetByIdAsync(int id)
    {
        var bank = await bankRepository.GetIdAsync(id);
        return bank == null ? null : mapper.Map<BankDto>(bank);
    }

    public async Task<BankDto> CreateAsync(BankDto dto)
    {
        var bank = mapper.Map<Bank>(dto);
        var createdBank = await bankRepository.AddAsync(bank);
        return mapper.Map<BankDto>(createdBank);
    }

    public async Task<bool> UpdateAsync(int id, BankDto dto)
    {
        var bank = await bankRepository.GetIdAsync(id);
        if (bank == null) return false;

        bank.Name = dto.Name;
        bank.Phone = dto.Phone;
        bank.Address = dto.Address;
        bank.Email = dto.Email;

        bankRepository.Update(bank);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var bank = await bankRepository.GetIdAsync(id);
        if (bank == null) return false;

        bankRepository.Remove(bank);
        await bankRepository.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<SelectItemDto>> GetSelectListAsync()
    {
        return await bankRepository.GetQuery()
            .Select(b => new SelectItemDto { Value = b.Id, Text = b.Name })
            .ToListAsync();
    }
}
