using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.Billing;

public class UserAppSetting : AuditableEntity, IMustHaveTenant
{
    public bool ClientStatus { get; set; }
    public int RequestPerIPOverride { get; set; }
    public int IPRestrictionIntervalOverrideInSeconds { get; set; }
    public int APIKeyLimitOverride { get; set; }
    public int APIKeyIntervalOverrideInSeconds { get; set; }
    public DateTime ExtendSuspensionDate { get; set; }
    public string UserId { get; set; }
    public string Tenant { get; set; }
    public bool AutoBill { get; set; }
    public bool IsActiveOrPendingProduct { get; set; }
    public decimal MaxCreditAmount { get; set; }
    public bool CanTakeOrders { get; set; }
    public bool AvaillableForOrders { get; set; }
    public bool AutoAssignOrders { get; set; }
    public UserAppSetting()
    {
    }

    public UserAppSetting(bool clientStatus, int requestPerIPOverride, int iPRestrictionIntervalOverrideInSeconds, int aPIKeyLimitOverride, int aPIKeyIntervalOverrideInSeconds, DateTime extendSuspensionDate, string userId, bool isActiveOrPendingProduct, bool? autoBill, decimal maxCreditAmount, bool canTakeOrders, bool availlableForOrders, bool autoAssignOrders)
    {
        ClientStatus = clientStatus;
        RequestPerIPOverride = requestPerIPOverride;
        IPRestrictionIntervalOverrideInSeconds = iPRestrictionIntervalOverrideInSeconds;
        APIKeyLimitOverride = aPIKeyLimitOverride;
        APIKeyIntervalOverrideInSeconds = aPIKeyIntervalOverrideInSeconds;
        ExtendSuspensionDate = extendSuspensionDate;
        UserId = userId;
        AutoBill = autoBill ?? true;
        this.IsActiveOrPendingProduct = isActiveOrPendingProduct;
        MaxCreditAmount = maxCreditAmount;
        CanTakeOrders = canTakeOrders;
        AvaillableForOrders = availlableForOrders;
        AutoAssignOrders = autoAssignOrders;
    }

    public UserAppSetting Update(bool clientStatus, int requestPerIPOverride, int iPRestrictionIntervalOverrideInSeconds, int aPIKeyLimitOverride, int aPIKeyIntervalOverrideInSeconds, DateTime extendSuspensionDate, string userId, bool isActiveOrPendingProduct, bool? autoBill, decimal maxCreditAmount, bool canTakeOrders, bool availlableForOrders, bool autoAssignOrders)
    {
        if (ClientStatus != clientStatus) { ClientStatus = clientStatus; }
        if (RequestPerIPOverride != requestPerIPOverride) { RequestPerIPOverride = requestPerIPOverride; }
        if (IPRestrictionIntervalOverrideInSeconds != iPRestrictionIntervalOverrideInSeconds) { IPRestrictionIntervalOverrideInSeconds = iPRestrictionIntervalOverrideInSeconds; }
        if (APIKeyLimitOverride != aPIKeyLimitOverride) { APIKeyLimitOverride = aPIKeyLimitOverride; }
        if (APIKeyIntervalOverrideInSeconds != aPIKeyIntervalOverrideInSeconds) { APIKeyIntervalOverrideInSeconds = aPIKeyIntervalOverrideInSeconds; }
        if (ExtendSuspensionDate != extendSuspensionDate) { ExtendSuspensionDate = extendSuspensionDate; }
        if (UserId != userId) { UserId = userId; }
        if (AutoBill != autoBill) { AutoBill = autoBill ?? true; }
        if (this.IsActiveOrPendingProduct != isActiveOrPendingProduct) { IsActiveOrPendingProduct = isActiveOrPendingProduct; }
        if (MaxCreditAmount != maxCreditAmount) { MaxCreditAmount = maxCreditAmount; }
        if (CanTakeOrders != canTakeOrders) { CanTakeOrders = canTakeOrders; }
        if (AvaillableForOrders != availlableForOrders) { AvaillableForOrders = availlableForOrders; }
        if (AutoAssignOrders != autoAssignOrders) { AutoAssignOrders = autoAssignOrders; }
        return this;
    }

}

public class UserRestrictedIp : BaseEntity
{
    public string UserId { get; set; }
    public string RestrictAccessIPAddress { get; set; }
}