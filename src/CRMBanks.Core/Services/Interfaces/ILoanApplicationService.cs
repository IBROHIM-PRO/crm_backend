using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface ILoanApplicationService
{
    Task<PagedLoanApplicationDto> GetFilteredAsync(LoanApplicationFilterDto filter, int currentUserId, string userRole);
    Task<LoanApplicationDto?> GetByIdAsync(int id);
    Task<LoanApplicationDto> CreateAsync(CreateLoanApplicationDto dto, int userId);
    Task<bool> UpdateStatusAsync(int id, UpdateLoanApplicationStatusDto dto, int userId);
    Task<bool> DeleteAsync(int id);
    Task<List<LoanApplicationDto>> GetAvailableForWorkerAsync(int workerId);
    Task<List<LoanApplicationDto>> GetAllForBossAsync();
}
