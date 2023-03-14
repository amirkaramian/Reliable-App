using MyReliableSite.Shared.DTOs.Notifications.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Notifications;

public class UsersBasedOnConditionsRequest
{
    public ConditionBasedOn Property { get; set; } // Like for Bills, Orders, Tickets etc
    public string OperatorType { get; set; } // Like <=, >=, != etc
    public string Value { get; set; } // Like 100, ProductName etc
}
