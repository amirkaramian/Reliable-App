using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using MyReliableSite.Domain.Departments;
using MyReliableSite.Shared.DTOs.Tickets;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyReliableSite.Domain.Tickets;

public class Ticket : AuditableEntity, IMustHaveTenant
{
    // Clients ticket scenario in future. so assigned to , priority will be optional
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TicketNumber { get; set; }
    public string AssignedTo { get; set; } // Currently, we will only show all the users of admin in a tenant | Tickets assignment will be from admin in client case | And another case is admin can assign the tickets to other admins
    public TicketStatus TicketStatus { get; set; }
    public TicketPriority TicketPriority { get; set; }
    public string Description { get; set; }
    public string TicketTitle { get; set; }
    public TicketRelatedTo TicketRelatedTo { get; set; } // Knowledgebase, ClientProblem, TableName | Enum
    public string TicketRelatedToId { get; set; } // primary keys
    public string Tenant { get; set; }
    public string DepartmentId { get; set; }
    public virtual Department Department { get; set; }
    public virtual ICollection<TicketComment> TicketComments { get; set; }
    public string ClientEmail { get; set; }
    public string ClientFullName { get; set; }
    public string ClientId { get; set; }
    public string BrandId { get; set; }
    public virtual Brand Brand { get; set; }
    public string Duration { get; set; }
    public string IdleTime { get; set; }
    public bool PinTicket { get; set; }
    public DateTime? FollowUpOn { get; set; }
    public string Group { get; set; }
    public string AgentUser { get; set; }
    public PriorityFollowUp? PriorityFollowUp { get; set; }
    public string Notes { get; set; }
    public string TransferComments { get; set; }
    public DateTime? TransferOn { get; set; }
    public string FollowUpComment { get; set; }
    public bool IncomingFromClient { get; set; }

    public Ticket(
        string ticketTitle,
        string description,
        string assignedTo,
        TicketPriority ticketPriority,
        TicketRelatedTo ticketRelatedTo,
        string ticketRelatedToId,
        TicketStatus ticketStatus,
        string departmentId,
        string duration,
        string idleTime,
        string brandId,
        string clientFullName,
        bool pinTicket,
        string clientEmail,
        DateTime? followUpOn,
        string followUpComment,
        string group,
        string agentUser,
        PriorityFollowUp? priorityFollowUp,
        string notes,
        string transferComments,
        DateTime? transferOn,
        string clientId,
        bool incomingFromClient)
    {
        TicketTitle = ticketTitle;
        Description = description;
        AssignedTo = assignedTo;
        TicketPriority = ticketPriority;
        TicketRelatedTo = ticketRelatedTo;
        TicketRelatedToId = ticketRelatedToId;
        TicketStatus = ticketStatus;
        DepartmentId = departmentId;
        Duration = duration;
        IdleTime = idleTime;
        BrandId = brandId;
        ClientFullName = clientFullName;
        PinTicket = pinTicket;
        ClientEmail = clientEmail;
        FollowUpOn = followUpOn;
        Group = group;
        AgentUser = agentUser;
        PriorityFollowUp = priorityFollowUp;
        Notes = notes;
        TransferComments = transferComments;
        TransferOn = transferOn;
        FollowUpComment = followUpComment;
        ClientId = clientId;
        IncomingFromClient = incomingFromClient;
    }

    public Ticket Update(
        string ticketTitle,
        string description,
        string assignedTo,
        TicketPriority ticketPriority,
        TicketRelatedTo ticketRelatedTo,
        string ticketRelatedToId,
        TicketStatus ticketStatus,
        string departmentId,
        string duration,
        string idleTime,
        string brandId,
        bool pinTicket,
        DateTime? followUpOn,
        string followUpComment,
        string group,
        string agentUser,
        PriorityFollowUp? priorityFollowUp,
        string notes,
        string transferComments,
        DateTime? transferOn,
        bool? incomingFromClient)
    {
        if (ticketTitle != null && !TicketTitle.NullToString().Equals(ticketTitle)) TicketTitle = ticketTitle;
        if (description != null && !Description.NullToString().Equals(description)) Description = description;
        if (assignedTo != null && !AssignedTo.NullToString().Equals(assignedTo)) AssignedTo = assignedTo;
        if (ticketPriority != TicketPriority) TicketPriority = ticketPriority;
        if (ticketRelatedTo != TicketRelatedTo) TicketRelatedTo = ticketRelatedTo;
        if (ticketRelatedToId != null && !TicketRelatedToId.NullToString().Equals(ticketRelatedToId)) TicketRelatedToId = ticketRelatedToId;
        if (departmentId != null && !DepartmentId.NullToString().Equals(departmentId)) DepartmentId = departmentId;
        if (ticketStatus != TicketStatus) TicketStatus = ticketStatus;
        if (departmentId != DepartmentId) DepartmentId = departmentId;

        if (duration != null && !Duration.NullToString().Equals(duration)) Duration = duration;
        if (idleTime != null && !IdleTime.NullToString().Equals(idleTime)) IdleTime = idleTime;
        if (brandId != null && !BrandId.NullToString().Equals(brandId)) BrandId = brandId;
        if (pinTicket != PinTicket) PinTicket = pinTicket;
        if (followUpOn != FollowUpOn) FollowUpOn = followUpOn;
        if (followUpComment != null && !FollowUpComment.NullToString().Equals(followUpComment)) FollowUpComment = followUpComment;

        if (group != null && !Group.NullToString().Equals(group)) Group = group;
        if (agentUser != null && !AgentUser.NullToString().Equals(agentUser)) AgentUser = agentUser;
        if (priorityFollowUp != null && !PriorityFollowUp.NullToString().Equals(priorityFollowUp)) PriorityFollowUp = priorityFollowUp;
        if (transferComments != null && !TransferComments.NullToString().Equals(transferComments)) TransferComments = transferComments;
        if (notes != null && !Notes.NullToString().Equals(notes)) Notes = notes;
        if (transferOn != TransferOn) TransferOn = transferOn;
        if (incomingFromClient != null && incomingFromClient != IncomingFromClient) IncomingFromClient = incomingFromClient.Value;

        return this;
    }
}

public class TicketHistory : BaseEntity
{
    public int TicketNumber { get; set; }
    public string AssignedTo { get; set; } // Currently, we will only show all the users of admin in a tenant | Tickets assignment will be from admin in client case | And another case is admin can assign the tickets to other admins
    public TicketStatus TicketStatus { get; set; }
    public TicketPriority TicketPriority { get; set; }
    public string Description { get; set; }
    public string TicketTitle { get; set; }
    public TicketRelatedTo TicketRelatedTo { get; set; } // Knowledgebase, ClientProblem, TableName | Enum
    public string TicketRelatedToId { get; set; } // primary keys
    public string DepartmentId { get; set; }
    public string ClientEmail { get; set; }
    public string ClientFullName { get; set; }
    public string ClientId { get; set; }
    public string BrandId { get; set; }
    public virtual Brand Brand { get; set; }
    public string Duration { get; set; }
    public string IdleTime { get; set; }
    public bool PinTicket { get; set; }
    public DateTime? FollowUpOn { get; set; }
    public string Group { get; set; }
    public string AgentUser { get; set; }
    public PriorityFollowUp? PriorityFollowUp { get; set; }
    public string Notes { get; set; }
    public string TransferComments { get; set; }
    public DateTime? TransferOn { get; set; }
    public string FollowUpComment { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid ActionBy { get; set; }
    public Guid TicketId { get; set; }
    public string ActionByName { get; set; }
    public string AssignedToFullName { get; set; }
    public Guid? TicketCommentId { get; set; }
    public Guid? TicketCommentReplyId { get; set; }
    public Guid? TicketCommentHistoryId { get; set; }
    public Guid? TicketCommentReplyHistoryId { get; set; }
    public bool IncomingFromClient { get; set; }

    public TicketHistory(
       string ticketTitle,
       string description,
       string assignedTo,
       TicketPriority ticketPriority,
       TicketRelatedTo ticketRelatedTo,
       string ticketRelatedToId,
       TicketStatus ticketStatus,
       string departmentId,
       string duration,
       string idleTime,
       string brandId,
       string clientFullName,
       bool pinTicket,
       string clientEmail,
       DateTime? followUpOn,
       string followUpComment,
       string group,
       string agentUser,
       PriorityFollowUp? priorityFollowUp,
       string notes,
       string transferComments,
       DateTime? transferOn,
       string clientId,
       Guid actionBy,
       Guid ticketId,
       Guid? ticketCommentId,
       Guid? ticketCommentReplyId,
       Guid? ticketCommentHistoryId,
       Guid? ticketCommentReplyHistoryId,
       bool incomingFromClient)
    {
        TicketTitle = ticketTitle;
        Description = description;
        AssignedTo = assignedTo;
        TicketPriority = ticketPriority;
        TicketRelatedTo = ticketRelatedTo;
        TicketRelatedToId = ticketRelatedToId;
        TicketStatus = ticketStatus;
        DepartmentId = departmentId;
        Duration = duration;
        IdleTime = idleTime;
        BrandId = brandId;
        ClientFullName = clientFullName;
        PinTicket = pinTicket;
        ClientEmail = clientEmail;
        FollowUpOn = followUpOn;
        Group = group;
        AgentUser = agentUser;
        PriorityFollowUp = priorityFollowUp;
        Notes = notes;
        TransferComments = transferComments;
        TransferOn = transferOn;
        FollowUpComment = followUpComment;
        ClientId = clientId;
        CreatedOn = DateTime.UtcNow;
        ActionBy = actionBy;
        TicketId = ticketId;
        TicketCommentId = ticketCommentId;
        TicketCommentReplyId = ticketCommentReplyId;
        TicketCommentHistoryId = ticketCommentHistoryId;
        TicketCommentReplyHistoryId = ticketCommentReplyHistoryId;
        IncomingFromClient = incomingFromClient;
    }
}