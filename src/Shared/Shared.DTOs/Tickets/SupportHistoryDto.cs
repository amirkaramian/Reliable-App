using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Tickets;
public class SupportStatisticsDto
{
    public string StarDate { get; set; }
    public string EndDate { get; set; }
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ReportSupportDto> Support { get; set; }
}

public class ReportSupportDto
{
    public string ReportBy { get; set; }
    public string FilterBy { get; set; }
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Status { get; set; }
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Department { get; set; }
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<SupportDetailDto> Details { get; set; }
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<SupportReplyDetailDto> SupportReplyDetails { get; set; }
}

public class SupportDetailDto
{
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Hourly { get; set; }
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Day { get; set; }
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Month { get; set; }
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Year { get; set; }
    public int LongTime { get; set; }
}

public class SupportReplyDetailDto
{
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Hourly { get; set; }
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Day { get; set; }
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Month { get; set; }
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Year { get; set; }
    public int Count { get; set; }
}
