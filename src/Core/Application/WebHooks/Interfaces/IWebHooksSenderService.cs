using MyReliableSite.Application.Common.Interfaces;

namespace MyReliableSite.Application.WebHooks.Interfaces;

public interface IWebHooksSenderService : ITransientService
{
    Task sendWebHook<T>(T model, string moduleName);
}
