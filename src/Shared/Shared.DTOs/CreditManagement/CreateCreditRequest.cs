using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.CreditManagement;
public class CreateCreditRequest : CreditDto, IMustBeValid
{
    public Guid ClientId { get; set; }
    public string Notes { get; set; }

}
