using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.Tickets;

namespace MyReliableSite.Application.Tickets.Validators;

public class CreateTicketCommentReplyRequestValidator : CustomValidator<CreateTicketCommentReplyRequest>
{
    public CreateTicketCommentReplyRequestValidator()
    {
        RuleFor(p => p.CommentText).NotNull().NotEmpty();
        RuleFor(p => p.TicketCommentId).NotNull().NotEmpty();
    }
}
