using Cofoundry.Core.BackgroundTasks;
using Hangfire;

namespace Cofoundry.Plugins.BackgroundTasks.Hangfire;

public class HangfireBackgroundTaskScheduler : IBackgroundTaskScheduler
{
    public IBackgroundTaskScheduler RegisterRecurringTask<TTask>(int days, int atHour, int atMinute) where TTask : IRecurringBackgroundTask
    {
        ValidateDailyTaskParameters(days, atHour, atMinute);

        var cronExpression = $"{atMinute} {atHour} */{days} * *";
        RegisterRecurringTask<TTask>(cronExpression);

        return this;
    }

    public IBackgroundTaskScheduler RegisterRecurringTask<TTask>(int hours = 0, int minute = 0) where TTask : IRecurringBackgroundTask
    {
        ValidateHourlyTaskParameters(hours, minute);

        if (hours == 0 && minute > 0)
        {
            return RegisterRecurringTask<TTask>(minute);
        }

        var cronExpression = $"{minute} */{hours} * * *";
        RegisterRecurringTask<TTask>(cronExpression);

        return this;
    }

    public IBackgroundTaskScheduler RegisterRecurringTask<TTask>(int minutes) where TTask : IRecurringBackgroundTask
    {
        ValidateMinuteTaskParameters(minutes);

        if (minutes > 59)
        {
            var hours = minutes / 60;
            minutes = minutes % 60;
            return RegisterRecurringTask<TTask>(hours, minutes);
        }

        var cronExpression = $"*/{minutes} * * * *";
        RegisterRecurringTask<TTask>(cronExpression);

        return this;
    }

    public IBackgroundTaskScheduler DeregisterRecurringTask<TTask>() where TTask : IRecurringBackgroundTask
    {
        RecurringJob.RemoveIfExists(GetJobId<TTask>());

        return this;
    }

    public IBackgroundTaskScheduler RegisterAsyncRecurringTask<TTask>(int days, int atHour = 0, int atMinute = 0) where TTask : IAsyncRecurringBackgroundTask
    {
        ValidateDailyTaskParameters(days, atHour, atMinute);

        var cronExpression = $"{atMinute} {atHour} */{days} * *";
        RegisterAsyncRecurringTask<TTask>(cronExpression);

        return this;
    }

    public IBackgroundTaskScheduler RegisterAsyncRecurringTask<TTask>(int hours = 0, int minute = 0) where TTask : IAsyncRecurringBackgroundTask
    {
        ValidateHourlyTaskParameters(hours, minute);

        if (hours == 0 && minute > 0)
        {
            return RegisterAsyncRecurringTask<TTask>(minute);
        }

        var cronExpression = $"{minute} */{hours} * * *";
        RegisterAsyncRecurringTask<TTask>(cronExpression);

        return this;
    }

    public IBackgroundTaskScheduler RegisterAsyncRecurringTask<TTask>(int minutes) where TTask : IAsyncRecurringBackgroundTask
    {
        ValidateMinuteTaskParameters(minutes);

        if (minutes > 59)
        {
            var hours = minutes / 60;
            minutes = minutes % 60;
            return RegisterAsyncRecurringTask<TTask>(hours, minutes);
        }

        var cronExpression = $"*/{minutes} * * * *";
        RegisterAsyncRecurringTask<TTask>(cronExpression);

        return this;
    }

    public IBackgroundTaskScheduler DeregisterAsyncRecurringTask<TTask>() where TTask : IAsyncRecurringBackgroundTask
    {
        RecurringJob.RemoveIfExists(GetJobId<TTask>());

        return this;
    }

    private static string GetJobId<TTask>()
    {
        var typeName = typeof(TTask).FullName;
        if (typeName == null)
        {
            throw new InvalidOperationException("Job type does not have a type name.");
        }

        return typeName;
    }

    private static void RegisterRecurringTask<TTask>(string cronExpression) where TTask : IRecurringBackgroundTask
    {
        RecurringJob.AddOrUpdate<TTask>(GetJobId<TTask>(), t => t.Execute(), cronExpression);
    }

    private static void RegisterAsyncRecurringTask<TTask>(string cronExpression) where TTask : IAsyncRecurringBackgroundTask
    {
        RecurringJob.AddOrUpdate<TTask>(GetJobId<TTask>(), t => t.ExecuteAsync(), cronExpression);
    }

    private static void ValidateDailyTaskParameters(int days, int atHour, int atMinute)
    {
        ValidateNotNegative(days, nameof(days));
        ValidateNotNegative(atHour, nameof(atHour));
        ValidateNotNegative(atMinute, nameof(atMinute));

        if (days == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(days), "Daily recurring tasks need to have an interval of at least 1 day.");
        }
    }

    private static void ValidateHourlyTaskParameters(int hours, int minute)
    {
        ValidateNotNegative(hours, nameof(hours));
        ValidateNotNegative(minute, nameof(minute));

        if (hours == 0 && minute == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(hours), "Recurring tasks need to have an interval of at least 1 minute.");
        }

        if (hours > 23)
        {
            throw new ArgumentOutOfRangeException(nameof(hours), "Recurring tasks with an interval of 24 hours or more should be scheduled using a daily interval instead.");
        }

        if (minute > 59)
        {
            throw new ArgumentOutOfRangeException(nameof(hours), "Recurring tasks with an interval measured in hours and minutes cannot have an minute component greater than 59 minutes.");
        }
    }

    private static void ValidateMinuteTaskParameters(int minutes)
    {
        ValidateNotNegative(minutes, nameof(minutes));
        if (minutes == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minutes), "Recurring tasks need to have an interval of at least 1 minute.");
        }
    }

    private static void ValidateNotNegative(int number, string argumentName)
    {
        if (number < 0)
        {
            throw new ArgumentOutOfRangeException(argumentName, "Recurring tasks cannot set the interval to negative numbers.");
        }
    }
}
