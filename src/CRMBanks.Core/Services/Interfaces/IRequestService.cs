using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface IRequestService
{
    Task<IEnumerable<RequestDto>> GetAllAsync();
    Task<PagedResponseDto<RequestDto>> GetPagedAsync(PagedRequestDto request, int userId);
    Task<RequestDto?> GetByIdAsync(int id);
    Task<RequestDto> CreateAsync(RequestDto dto);
    Task<bool> UpdateStateAsync(int id, string state, string? reason = null, int? userId = null);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<SelectItemDto>> GetSelectListAsync();
}
