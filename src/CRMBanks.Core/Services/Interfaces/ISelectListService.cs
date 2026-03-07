using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface ISelectListService
{
    Task<IEnumerable<SelectItemDto>> GetRolesAsync();
    Task<IEnumerable<SelectItemDto>> GetBanksAsync();
    Task<IEnumerable<SelectItemDto>> GetRegionsAsync();
    Task<IEnumerable<SelectItemDto>> GetTypeCreditsAsync();
    Task<IEnumerable<SelectItemDto>> GetTypeDepositsAsync();
    Task<IEnumerable<SelectItemDto>> GetUsersAsync(int bankId);
    Task<IEnumerable<SelectItemDto>> GetCreditsAsync(int bankId);
    Task<IEnumerable<SelectItemDto>> GetDepositsAsync(int bankId);
    Task<IEnumerable<SelectItemDto>> GetCardsAsync(int bankId);
    Task<IEnumerable<SelectItemDto>> GetRequestsAsync(int bankId);
}
