﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.CreditManagement;
public class CreditDto : IDto
{
    public Guid Id { get; set; }
    [Range(typeof(decimal), "1", "79228162514264337593543950335")]
    public decimal Amount { get; set; }
    public string Tenant { get; set; }
    public string Description { get; set; }
}

