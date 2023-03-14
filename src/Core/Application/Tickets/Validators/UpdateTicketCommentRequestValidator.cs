using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.Tickets;

namespace MyReliableSite.Application.Tickets.Validators;

public class UpdateTicketCommentRequestValidator : CustomValidator<UpdateTicketCommentRequest>
{
    public UpdateTicketCommentRequestValidator()
    {
        RuleFor(p => p.CommentText).NotNull().NotEmpty();
        RuleFor(p => p.IsSticky).Must(x => x == true || x == false);
        RuleFor(p => p.TicketCommentAction).IsInEnum();
        RuleFor(p => p.TicketCommentType).IsInEnum();
    }
}
