using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Core.Services;

public class SelectListService(
    IRepository<Role> roleRepository,
    IRepository<Bank> bankRepository,
    IRepository<Region> regionRepository,
    IRepository<TypeCredit> typeCreditRepository,
    IRepository<TypeDeposit> typeDepositRepository,
    IRepository<User> userRepository,
    IRepository<Credit> creditRepository,
    IRepository<Deposit> depositRepository,
    IRepository<Card> cardRepository,
    IRepository<Request> requestRepository) : ISelectListService
{
    public async Task<IEnumerable<SelectItemDto>> GetRolesAsync() =>
        await roleRepository.GetQuery().Select(r => new SelectItemDto { Value = r.Id, Text = r.Name }).ToListAsync();

    public async Task<IEnumerable<SelectItemDto>> GetBanksAsync() =>
        await bankRepository.GetQuery().Select(b => new SelectItemDto { Value = b.Id, Text = b.Name }).ToListAsync();

    public async Task<IEnumerable<SelectItemDto>> GetRegionsAsync() =>
        await regionRepository.GetQuery().Select(r => new SelectItemDto { Value = r.Id, Text = r.Name }).ToListAsync();

    public async Task<IEnumerable<SelectItemDto>> GetTypeCreditsAsync() =>
        await typeCreditRepository.GetQuery().Select(t => new SelectItemDto { Value = t.Id, Text = t.Name }).ToListAsync();

    public async Task<IEnumerable<SelectItemDto>> GetTypeDepositsAsync() =>
        await typeDepositRepository.GetQuery().Select(t => new SelectItemDto { Value = t.Id, Text = t.Name }).ToListAsync();

    public async Task<IEnumerable<SelectItemDto>> GetUsersAsync(int bankId) =>
        await userRepository.GetQuery().Where(u => u.BankId == bankId)
            .Select(u => new SelectItemDto { Value = u.Id, Text = u.Name }).ToListAsync();

    public async Task<IEnumerable<SelectItemDto>> GetCreditsAsync(int bankId) =>
        await creditRepository.GetQuery().Where(c => c.BankId == bankId)
            .Select(c => new SelectItemDto { Value = c.Id, Text = c.Name }).ToListAsync();

    public async Task<IEnumerable<SelectItemDto>> GetDepositsAsync(int bankId) =>
        await depositRepository.GetQuery().Where(d => d.BankId == bankId)
            .Select(d => new SelectItemDto { Value = d.Id, Text = d.Name }).ToListAsync();

    public async Task<IEnumerable<SelectItemDto>> GetCardsAsync(int bankId) =>
        await cardRepository.GetQuery().Select(c => new SelectItemDto { Value = c.Id, Text = c.Name }).ToListAsync();

    public async Task<IEnumerable<SelectItemDto>> GetRequestsAsync(int bankId) =>
        await requestRepository.GetQuery()
            .Include(r => r.Banks)
            .Where(r => r.Banks != null && r.Banks.Any(b => b.Id == bankId))
            .Select(r => new SelectItemDto { Value = r.Id, Text = r.Name }).ToListAsync();
}
