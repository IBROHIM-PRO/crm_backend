using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface IDepositApplicationService
{
    Task<PagedDepositApplicationDto> GetFilteredAsync(DepositApplicationFilterDto filter, int currentUserId, string userRole);
    Task<DepositApplicationDto?> GetByIdAsync(int id);
    Task<DepositApplicationDto> CreateAsync(CreateDepositApplicationDto dto, int userId);
    Task<bool> UpdateStatusAsync(int id, UpdateDepositApplicationStatusDto dto, int userId);
    Task<bool> DeleteAsync(int id);
    Task<List<DepositApplicationDto>> GetAvailableForWorkerAsync(int workerId);
    Task<List<DepositApplicationDto>> GetAllForBossAsync();
}
