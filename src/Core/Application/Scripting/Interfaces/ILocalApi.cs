using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Products.Interfaces;

namespace MyReliableSite.Application.Scripting.Interfaces;

public interface ILocalApi : ITransientService
{
    IRepositoryAsync Repository { get; }
    IMailService MailService { get; }
    IUserService UserService { get; }
    ICurrentUser CurrentUser { get; }
    IProductService ProductService { get; }

    string Test(string param);
}
