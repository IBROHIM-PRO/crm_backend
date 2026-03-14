using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface ITypeDepositService
{
    Task<IEnumerable<TypeDepositDto>> GetAllAsync();
    Task<PagedResponseDto<TypeDepositDto>> GetPagedAsync(PagedRequestDto request);
    Task<TypeDepositDto?> GetByIdAsync(int id);
    Task<TypeDepositDto> CreateAsync(TypeDepositDto dto);
    Task<bool> UpdateAsync(int id, TypeDepositDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<SelectItemDto>> GetSelectListAsync();
}
