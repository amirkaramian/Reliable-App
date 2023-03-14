using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.Tickets;

namespace MyReliableSite.Application.Tickets.Validators;

public class CreateTicketCommentRequestValidator : CustomValidator<CreateTicketCommentRequest>
{
    public CreateTicketCommentRequestValidator()
    {
        RuleFor(p => p.TicketId).NotNull().NotEmpty();
        RuleFor(p => p.CommentText).NotNull().NotEmpty();
        RuleFor(p => p.IsSticky).Must(x => x == true || x == false);
        RuleFor(p => p.TicketCommentAction).IsInEnum();
        RuleFor(p => p.TicketCommentType).IsInEnum();
    }
}
