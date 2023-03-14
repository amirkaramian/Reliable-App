using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Brands.Services;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Specifications;
using MyReliableSite.Application.Storage;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Billing.Events.Brands;
using MyReliableSite.Domain.Billing.Events.Notifications.Templates;
using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Dashboard;
using MyReliableSite.Domain.Tickets;
using MyReliableSite.Infrastructure.Identity.Models;
using MyReliableSite.Shared.DTOs.Brands;
using MyReliableSite.Shared.DTOs.Filters;
using MyReliableSite.Shared.DTOs.Identity;
using MyReliableSite.Shared.DTOs.Notifications;
using MyReliableSite.Shared.DTOs.Notifications.Enums;
using MyReliableSite.Shared.DTOs.Notifications.Templates;
using MyReliableSite.Shared.DTOs.Tickets;

namespace MyReliableSite.Infrastructure.Common.Services;

public class NotificationTemplateService : INotificationTemplateService
{
    private readonly IRepositoryAsync _repository;
    private readonly IStringLocalizer<NotificationTemplateService> _localizer;
    private readonly ICurrentUser _currentUser;
    private IUserService _userService;

    public NotificationTemplateService(IRepositoryAsync repository, IStringLocalizer<NotificationTemplateService> localizer, IFileStorageService fileStorageService, ICurrentUser currentUser, IUserService userService)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result<NotificationTemplateDetailsDto>> GetAsync(Guid id)
    {
        var toReturn = await _repository.GetByIdAsync<NotificationTemplate>(id);

        if (toReturn == null)
            throw new EntityNotFoundException(string.Format(_localizer["notificationtemplate.notfound"], id));

        return await Result<NotificationTemplateDetailsDto>.SuccessAsync(toReturn.Adapt<NotificationTemplateDetailsDto>());
    }

    public async Task<PaginatedResult<NotificationTemplateDto>> SearchAsync(NotificatoinTemplatesListFilter filter)
    {
        PaginatedResult<NotificationTemplateDto> templates = null;

        if (filter.AdvancedSearch != null && filter.AdvancedSearch.Fields.FirstOrDefault(x => x == "status") != null)
        {
            string status = filter.AdvancedSearch?.Keyword;
            var statusEnum = (NotificationTemplateStatus)Convert.ToInt32(status);

            templates = await _repository.GetSearchResultsAsync<NotificationTemplate, NotificationTemplateDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, null, filter.Keyword, x => x.Status == statusEnum && ((filter.StartDate == null || x.CreatedOn >= filter.StartDate) && (filter.EndDate == null || x.CreatedOn <= filter.EndDate)));
        }
        else
        {
            templates = await _repository.GetSearchResultsAsync<NotificationTemplate, NotificationTemplateDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, x => (filter.StartDate == null || x.CreatedOn >= filter.StartDate) && (filter.EndDate == null || x.CreatedOn <= filter.EndDate));
        }

        foreach (var notificationTemplate in templates.Data)
        {
            var totalUsers = await _userService.GetUsersBasedOnConditionsForNotificationTemplates(new UsersBasedOnConditionsRequest()
            {
                OperatorType = notificationTemplate.OperatorType,
                Property = notificationTemplate.Property,
                Value = notificationTemplate.Value
            });

            if(totalUsers.Data != null)
            {
                notificationTemplate.TotalUsers = totalUsers.Data.Count();
            }
        }

        return templates;
    }

    public async Task<Result<NotificationTemplateDto>> CreateAsync(CreateNotificationTemplateRequest request)
    {
        if (await _repository.ExistsAsync<NotificationTemplate>(a => a.Title.ToLower() == request.Title.ToLower()))
            throw new EntityAlreadyExistsException(string.Format(_localizer["notificationtemplate.alreadyexists"], request.Title));

        var toAdd = new NotificationTemplate(request.Title, request.Body, request.StartDate, request.EndDate, request.Status, request.TargetUserType, request.Property, request.OperatorType, request.Value);

        toAdd.DomainEvents.Add(new NotificationTemplateCreatedEvent(toAdd));
        toAdd.DomainEvents.Add(new StatsChangedEvent());

        _ = await _repository.CreateAsync(toAdd);
        _ = await _repository.SaveChangesAsync();
        return await Result<NotificationTemplateDto>.SuccessAsync(toAdd.Adapt<NotificationTemplateDto>());
    }

    public async Task<Result<NotificationTemplateDetailsDto>> UpdateAsync(Guid id, UpdateNotificationTemplateRequest request)
    {
        var toUpdate = await _repository.GetByIdAsync<NotificationTemplate>(id);

        if (toUpdate == null)
            throw new EntityNotFoundException(string.Format(_localizer["notificationtemplate.notfound"], id));

        var updatedNotificationTemplate = toUpdate.Update(request.Title, request.Body, request.StartDate, request.EndDate, request.Status, request.TargetUserType, request.Property, request.OperatorType, request.Value);

        toUpdate.DomainEvents.Add(new NotificationTemplateUpdatedEvent(toUpdate));
        toUpdate.DomainEvents.Add(new StatsChangedEvent());

        await _repository.UpdateAsync(updatedNotificationTemplate);
        _ = await _repository.SaveChangesAsync();
        return await Result<NotificationTemplateDetailsDto>.SuccessAsync(updatedNotificationTemplate.Adapt<NotificationTemplateDetailsDto>());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var toDelete = await _repository.GetByIdAsync<NotificationTemplate>(id);
        if(toDelete == null) throw new EntityNotFoundException(string.Format(_localizer["notificationtemplate.notfound"], id));

        toDelete.DeletedOn = DateTime.UtcNow;
        toDelete.DeletedBy = _currentUser.GetUserId();

        toDelete.DomainEvents.Add(new NotificationTemplateDeletedEvent(toDelete));

        await _repository.SaveChangesAsync();
        return await Result<bool>.SuccessAsync(await _repository.SaveChangesAsync() > 0);
    }
}
