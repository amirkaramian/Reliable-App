using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.Tickets;

namespace MyReliableSite.Application.Tickets.Validators;

public class UpdateTicketRequestValidator : CustomValidator<UpdateTicketRequest>
{
    public UpdateTicketRequestValidator()
    {
        RuleFor(p => p.Description).NotNull().NotEmpty();
        RuleFor(p => p.TicketRelatedTo).IsInEnum();
        RuleFor(p => p.TicketStatus).IsInEnum();
        RuleFor(p => p.TicketPriority).IsInEnum();
        RuleFor(p => p.TicketRelatedToId).NotNull().NotEmpty();
        RuleFor(p => p.DepartmentId).NotNull().NotEmpty();
        RuleFor(p => p.PinTicket).Must(x => x == true || x == false);
    }
}
