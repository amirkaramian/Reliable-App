﻿namespace MyReliableSite.Shared.DTOs.ManageModule;

public class AdminGroupModuleDto : IDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string PermissionDetail { get; set; }
    public string Tenant { get; set; }
    public bool IsActive { get; set; }
    public string AdminGroupId { get; set; }
}
