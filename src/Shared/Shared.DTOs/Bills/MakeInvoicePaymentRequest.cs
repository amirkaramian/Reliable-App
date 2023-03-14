﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Bills;
public class MakeInvoicePaymentRequest : IMustBeValid
{
    public string Tenant { get; set; }
    public decimal TotalAmount { get; set; }
    public string Notes { get; set; }
    public string InvoiceNumber { get; set; }

}
