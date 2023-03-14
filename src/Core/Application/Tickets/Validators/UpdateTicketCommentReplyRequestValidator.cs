using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.Tickets;

namespace MyReliableSite.Application.Tickets.Validators;

public class UpdateTicketCommentReplyRequestValidator : CustomValidator<UpdateTicketCommentReplyRequest>
{
    public UpdateTicketCommentReplyRequestValidator()
    {
        RuleFor(p => p.CommentText).NotNull().NotEmpty();
    }
}
