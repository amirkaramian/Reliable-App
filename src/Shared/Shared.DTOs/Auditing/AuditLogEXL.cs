using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Audit;
public class AuditLogEXL
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; }
    public string Type { get; set; }
    public string TableName { get; set; }
    public string DateTime { get; set; }
    public string OldValues { get; set; }
    public string NewValues { get; set; }
    public string AffectedColumns { get; set; }
    public string PrimaryKey { get; set; }
    public string Text { get; set; }
    public double CreatedDaysAgo { get; set; }
}
