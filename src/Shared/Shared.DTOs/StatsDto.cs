using MyReliableSite.Shared.DTOs.Dashboard;
using MyReliableSite.Shared.DTOs.Tickets;

namespace MyReliableSite.Shared.DTOs;

public class StatsDto
{
    public int ArticleCount { get; set; }
    public int CategoryCount { get; set; }
    public int UserCount { get; set; }
    public int RoleCount { get; set; }
}