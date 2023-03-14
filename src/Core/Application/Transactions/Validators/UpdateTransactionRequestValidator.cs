using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.Transaction;

namespace MyReliableSite.Application.Transactions.Validators;
public class UpdateTransactionRequestValidator : CustomValidator<UpdateTransactionRequest>
{
    public UpdateTransactionRequestValidator()
    {
        RuleFor(p => p.Tenant).NotEmpty().NotNull();
        RuleFor(p => p.TransactionStatus).NotEmpty().NotNull();
    }
}
