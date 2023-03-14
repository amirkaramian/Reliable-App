using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Notifications.Enums;

public enum ConditionBasedOn
{
    Bills, // e.g Client which have Total Bill >= 1000 USD
    Tickets, // e.g Admins which have Total Tickets >= 5 in Numbers
    Orders, // e.g Clients which have Total Order >= 5
    Products, // e.g Users having Products >= 5
    Refunds // e.g Users having Refunds >= 5
}