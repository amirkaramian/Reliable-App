using Hangfire;
using Hangfire.Console.Extensions;
using Hangfire.Console.Progress;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Categories.Interfaces;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Domain.Categories;
using MyReliableSite.Domain.Dashboard;
using MyReliableSite.Shared.DTOs.Notifications;
using MyReliableSite.Shared.DTOs.Notifications.Enums;

namespace MyReliableSite.Infrastructure.Categories;

public class CategoryGeneratorJob : ICategoryGeneratorJob
{
    private readonly IEventService _eventService;
    private readonly ILogger<CategoryGeneratorJob> _logger;
    private readonly IRepositoryAsync _repository;
    private readonly IProgressBarFactory _progressBar;
    private readonly PerformingContext _performingContext;
    private readonly INotificationService _notificationService;
    private readonly ICurrentUser _currentUser;
    private readonly IProgressBar _progress;

    public CategoryGeneratorJob(
        ILogger<CategoryGeneratorJob> logger,
        IRepositoryAsync repository,
        IProgressBarFactory progressBar,
        PerformingContext performingContext,
        INotificationService notificationService,
        ICurrentUser currentUser,
        IEventService eventService)
    {
        _logger = logger;
        _repository = repository;
        _progressBar = progressBar;
        _performingContext = performingContext;
        _notificationService = notificationService;
        _currentUser = currentUser;
        _progress = _progressBar.Create();
        _eventService = eventService;
    }

    [Queue("notdefault")]
    public async Task GenerateAsync(int nSeed)
    {
        await Notify("Your job processing has started");
        foreach (int index in Enumerable.Range(1, nSeed))
        {
            await _repository.CreateAsync(new Category(name: $"Category Random - {Guid.NewGuid()}", "Funny description", null, CategoryType.Articles, null));
            await Notify("Progress: ", nSeed > 0 ? (index * 100 / nSeed) : 0);
        }

        await _repository.SaveChangesAsync();
        await _eventService.PublishAsync(new StatsChangedEvent());
        await Notify("Job successfully completed");
    }

    [Queue("notdefault")]
    [AutomaticRetry(Attempts = 5)]
    public async Task CleanAsync()
    {
        _logger.LogInformation("Initializing Job with Id: {JobId}", _performingContext.BackgroundJob.Id);
        var items = await _repository.GetListAsync<Category>(x => x.Name.Contains("Category Random"), true);
        _logger.LogInformation("Categories Random: {CategoriesCount} ", items.Count.ToString());

        foreach (var item in items)
        {
            await _repository.RemoveAsync(item);
        }

        int rows = await _repository.SaveChangesAsync();
        await _eventService.PublishAsync(new StatsChangedEvent());
        _logger.LogInformation("Rows affected: {rows} ", rows.ToString());
    }

    private async Task Notify(string message, int progress = 0)
    {
        _progress.SetValue(progress);
        await _notificationService.SendMessageToUserAsync(
            _currentUser.GetUserId().ToString(),
            new JobNotification()
            {
                JobId = _performingContext.BackgroundJob.Id,
                Message = message,
                Progress = progress,
                NotificationType = NotificationType.CATEGORY_GENERATOR,
                TargetUserTypes = NotificationTargetUserTypes.Admins,
                NotifyModelId = Guid.Parse(_performingContext.BackgroundJob.Id)
            });
    }
}
