using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.CreditManagement;
public class CreditInfoRequest : IMustBeValid
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public CreditInfoRequest(Guid id, Guid clientId)
    {
        this.Id = id;
        this.ClientId = clientId;
    }

    public CreditInfoRequest(Guid clientId)
    {
        this.ClientId = clientId;
    }

    public string Tenant { get; set; }
}
