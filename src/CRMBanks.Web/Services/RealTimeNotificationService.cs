using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using CRMBanks.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Web.Services;

public class RealTimeNotificationService(
    IRepository<User> userRepository,
    IRepository<Bank> bankRepository,
    IRepository<Credit> creditRepository,
    IRepository<Deposit> depositRepository,
    IHubContext<NotificationHub> hubContext) : IRealTimeNotificationService
{
    public async Task NotifyEligibleWorkersForNewLoanApplicationAsync(LoanApplicationDto application)
    {
        // Get the credit to find which bank it belongs to
        var credit = await creditRepository.GetIdAsync(application.CreditId);
        if (credit == null) return;

        List<int> targetBankIds;
        
        if (application.SendToAllBanks)
        {
            // Get all bank IDs
            var allBanks = await bankRepository.GetAllAsync();
            targetBankIds = allBanks.Select(b => b.Id).ToList();
        }
        else
        {
            // Get selected bank IDs from bank names
            targetBankIds = application.SelectedBankNames
                .Select(bankName => GetBankIdByName(bankName))
                .Where(id => id > 0)
                .ToList();
        }

        var eligibleWorkerIds = await GetEligibleWorkerIdsForLoanApplicationAsync(
            application.CreditId, 
            application.RegionId, 
            application.RequestedAmount, 
            targetBankIds);

        if (eligibleWorkerIds.Any())
        {
            var notificationPayload = new
            {
                Type = "NewLoanApplication",
                ApplicationId = application.Id,
                ApplicantName = application.ApplicantName,
                RequestedAmount = application.RequestedAmount,
                RegionName = application.RegionName,
                CreditName = application.CreditName,
                SubmittedToBanks = application.SelectedBankNames,
                SubmissionDate = DateTime.UtcNow,
                Message = $"New loan application from {application.ApplicantName} for {application.RequestedAmount:C}"
            };

            await hubContext.Clients.Users(eligibleWorkerIds.Select(id => id.ToString()))
                .SendAsync("ReceiveNewApplication", notificationPayload);
        }
    }

    public async Task NotifyEligibleWorkersForNewDepositApplicationAsync(DepositApplicationDto application)
    {
        // Get the deposit to find which bank it belongs to
        var deposit = await depositRepository.GetIdAsync(application.DepositId);
        if (deposit == null) return;

        List<int> targetBankIds;
        
        if (application.SendToAllBanks)
        {
            // Get all bank IDs
            var allBanks = await bankRepository.GetAllAsync();
            targetBankIds = allBanks.Select(b => b.Id).ToList();
        }
        else
        {
            // Get selected bank IDs from bank names
            targetBankIds = application.SelectedBankNames
                .Select(bankName => GetBankIdByName(bankName))
                .Where(id => id > 0)
                .ToList();
        }

        var eligibleWorkerIds = await GetEligibleWorkerIdsForDepositApplicationAsync(
            application.DepositId,
            application.RegionId,
            application.DepositAmount,
            targetBankIds);

        if (eligibleWorkerIds.Any())
        {
            var notificationPayload = new
            {
                Type = "NewDepositApplication",
                ApplicationId = application.Id,
                ApplicantName = application.ApplicantName,
                DepositAmount = application.DepositAmount,
                RegionName = application.RegionName,
                DepositName = application.DepositName,
                SubmittedToBanks = application.SelectedBankNames,
                SubmissionDate = DateTime.UtcNow,
                Message = $"New deposit application from {application.ApplicantName} for {application.DepositAmount:C}"
            };

            await hubContext.Clients.Users(eligibleWorkerIds.Select(id => id.ToString()))
                .SendAsync("ReceiveNewApplication", notificationPayload);
        }
    }

    public async Task<List<int>> GetEligibleWorkerIdsForLoanApplicationAsync(int creditId, int regionId, decimal amount, List<int> targetBankIds)
    {
        // Query workers based on the three criteria
        var eligibleWorkers = await userRepository.GetQuery()
            .Include(u => u.Role)
            .Include(u => u.Regions)
            .Include(u => u.Bank)
            .Where(u => u.Role != null && u.Role.Name.ToLower() == "worker") // Worker role
            .Where(u => u.MaxLoanAmount >= amount) // Monetary limit check
            .Where(u => u.Regions.Any(r => r.Id == regionId)) // Region match
            .Where(u => targetBankIds.Contains(u.BankId)) // Bank match - worker must belong to any of the target banks
            .Select(u => u.Id)
            .ToListAsync();

        return eligibleWorkers;
    }

    public async Task<List<int>> GetEligibleWorkerIdsForDepositApplicationAsync(int depositId, int regionId, decimal amount, List<int> targetBankIds)
    {
        // Query workers based on the three criteria
        var eligibleWorkers = await userRepository.GetQuery()
            .Include(u => u.Role)
            .Include(u => u.Regions)
            .Include(u => u.Bank)
            .Where(u => u.Role != null && u.Role.Name.ToLower() == "worker") // Worker role
            .Where(u => u.MaxLoanAmount >= amount) // Monetary limit check
            .Where(u => u.Regions.Any(r => r.Id == regionId)) // Region match
            .Where(u => targetBankIds.Contains(u.BankId)) // Bank match - worker must belong to any of the target banks
            .Select(u => u.Id)
            .ToListAsync();

        return eligibleWorkers;
    }

    private int GetBankIdByName(string bankName)
    {
        // This is a helper method - in production, you might want to cache this
        var banks = bankRepository.GetAllAsync().Result;
        var bank = banks.FirstOrDefault(b => b.Name == bankName);
        return bank?.Id ?? 0;
    }
}
