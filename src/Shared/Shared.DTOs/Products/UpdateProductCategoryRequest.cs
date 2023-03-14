using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Products;
public class UpdateProductCategoryRequest : IMustBeValid
{
    public Guid CategoryId { get; set; }
}
