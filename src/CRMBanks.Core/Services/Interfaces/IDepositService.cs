using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface IDepositService
{
    Task<IEnumerable<DepositDto>> GetAllByBankAsync(int bankId);
    Task<PagedResponseDto<DepositDto>> GetPagedByBankAsync(int bankId, PagedRequestDto request);
    Task<DepositDto?> GetByIdAsync(int id);
    Task<DepositDto> CreateAsync(DepositDto dto, int bankId);
    Task<bool> UpdateAsync(int id, DepositDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<SelectItemDto>> GetSelectListAsync(int bankId);
}
