using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Identity;
using MyReliableSite.Shared.DTOs.WHMCS;

namespace MyReliableSite.Application.WHMCS;

public interface IWHMCSImportService : ITransientService
{
    Task<ImportWHMSCResponse> ValidateTheData(ImportWHMSCRequest importWHMSCRequest);

    Task<List<IResult>> ImportData(ImportWHMSCRequest importWHMSCRequest, string origin);

}