using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface IBankService
{
    Task<IEnumerable<BankDto>> GetAllAsync();
    Task<PagedResponseDto<BankDto>> GetPagedAsync(PagedRequestDto request);
    Task<BankDto?> GetByIdAsync(int id);
    Task<BankDto> CreateAsync(BankDto dto);
    Task<bool> UpdateAsync(int id, BankDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<SelectItemDto>> GetSelectListAsync();
}
