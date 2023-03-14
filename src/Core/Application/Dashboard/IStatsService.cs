using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs;

namespace MyReliableSite.Application.Dashboard;

public interface IStatsService : ITransientService
{
    Task<IResult<StatsDto>> GetDataAsync();
}