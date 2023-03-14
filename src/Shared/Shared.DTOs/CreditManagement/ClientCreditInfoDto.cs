using MyReliableSite.Shared.DTOs.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.CreditManagement;
public class ClientCreditInfoDto : IMustBeValid
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public decimal Balance { get; set; }

    public TransactionStatus TransactionStatus { get; set; }

    public ClientCreditInfoDto(Guid id, Guid clientId, decimal balance)
    {
        Id = id;
        this.ClientId = clientId;
        this.Balance = balance;
    }

    public ClientCreditInfoDto(Guid clientId, decimal balance)
    {
        this.ClientId = clientId;
        this.Balance = balance;
    }
}