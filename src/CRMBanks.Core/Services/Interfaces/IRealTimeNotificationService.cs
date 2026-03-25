using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface IRealTimeNotificationService
{
    Task NotifyEligibleWorkersForNewLoanApplicationAsync(LoanApplicationDto application);
    Task NotifyEligibleWorkersForNewDepositApplicationAsync(DepositApplicationDto application);
    Task<List<int>> GetEligibleWorkerIdsForLoanApplicationAsync(int creditId, int regionId, decimal amount, List<int> selectedBankIds);
    Task<List<int>> GetEligibleWorkerIdsForDepositApplicationAsync(int depositId, int regionId, decimal amount, List<int> selectedBankIds);
}
