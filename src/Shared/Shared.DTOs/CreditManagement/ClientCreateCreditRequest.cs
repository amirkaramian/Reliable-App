using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.CreditManagement;
public class ClientCreateCreditRequest : IMustBeValid
{
    public PaymentMehods PaymentMehod { get; set; }
    public string Tenant { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
}

public enum PaymentMehods
{
    CreditCard,
    PayPal,
    Crypto,
    AirPay,
    WriteTransfer,
    GiftCode,
}