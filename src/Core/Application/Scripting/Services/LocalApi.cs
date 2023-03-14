using Microsoft.Extensions.Options;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Products.Interfaces;
using MyReliableSite.Application.Scripting.Interfaces;

namespace MyReliableSite.Application.Scripting.Services;

public class LocalApi : ILocalApi
{
    public LocalApi(IRepositoryAsync repository, IMailService mailService, IUserService userService, ICurrentUser user, IProductService productService)
    {
        Repository = repository;
        MailService = mailService;
        UserService = userService;
        CurrentUser = user;
        ProductService = productService;
    }

    public IRepositoryAsync Repository { get; }
    public IMailService MailService { get; }
    public IUserService UserService { get; }
    public ICurrentUser CurrentUser { get; }
    public IProductService ProductService { get; }

    public string Test(string param)
    {
        return param;
    }
}
