using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Products;
public class UpdateProductDepartmentRequest : IMustBeValid
{
    public Guid DepartmentId { get; set; }
}
