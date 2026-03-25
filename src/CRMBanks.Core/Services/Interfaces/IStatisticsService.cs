using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface IStatisticsService
{
    Task<GlobalStatisticsDto> GetGlobalStatisticsAsync();
    Task<PersonalStatisticsDto> GetPersonalStatisticsAsync(int userId);
}
