using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface ITypeCreditService
{
    Task<IEnumerable<TypeCreditDto>> GetAllAsync();
    Task<PagedResponseDto<TypeCreditDto>> GetPagedAsync(PagedRequestDto request);
    Task<TypeCreditDto?> GetByIdAsync(int id);
    Task<TypeCreditDto> CreateAsync(TypeCreditDto dto);
    Task<bool> UpdateAsync(int id, TypeCreditDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<SelectItemDto>> GetSelectListAsync();
}
