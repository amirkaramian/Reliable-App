using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Application.Dashboard.Interfaces;
public interface IDashboardService : ITransientService
{
    Task<Result<DashboardDto>> GetAsync();
    Task<Result<DataCountDto>> GetDataCountsAsync();
    Task<Result<ClientDashboardDto>> GetCleintDataAsync();
}
