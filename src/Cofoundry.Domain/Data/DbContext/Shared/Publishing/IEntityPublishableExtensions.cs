namespace Cofoundry.Domain.Data.Internal;

public static class IEntityPublishableExtensions
{
    /// <summary>
    /// Updates the <see cref="IEntityPublishable.PublishStatusCode"/> and 
    /// publish date properties to mark the entity as published.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity to act on.</typeparam>
    /// <param name="entity">Entity to act on.</param>
    /// <param name="currentDate">The current date and time, typically passed from the execution context in the domain layer.</param>
    /// <param name="publishDate">
    /// Optional time that the entity should be published and made public. If
    /// this is left <see langword="null"/> then the publish date is set to the current 
    /// date and the page is made immediately available.
    /// </param>
    /// <returns>
    /// Returns <see langword="true"/> if the entity status has changed. If the entity was already published and
    /// no change needed to be made, then <see langword="false"/> is returned.
    /// </returns>
    public static bool SetPublished<TEntity>(this TEntity entity, DateTime currentDate, DateTime? publishDate = null)
        where TEntity : IEntityPublishable
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentEmptyException.ThrowIfDefault(currentDate);

        bool hasPublishStatusChanged = false;

        if (entity.PublishStatusCode != PublishStatusCode.Published)
        {
            hasPublishStatusChanged = true;
            entity.PublishStatusCode = PublishStatusCode.Published;
        }

        UpdatePublishDate(entity, currentDate, publishDate);

        return hasPublishStatusChanged;
    }

    private static void UpdatePublishDate<TEntity>(TEntity entity, DateTime currentDate, DateTime? publishDate = null)
        where TEntity : IEntityPublishable
    {
        if (publishDate.HasValue)
        {
            // If a specific date has been requested, use it
            entity.PublishDate = publishDate;
        }
        else if (!entity.PublishDate.HasValue)
        {
            // Otheriwse only set it if it's not been published yet
            entity.PublishDate = currentDate;
        }

        // Always update the last publish date
        entity.LastPublishDate = currentDate;

        // but if the publish date has been requested for the future, reflect that in LastPublishDate
        if (entity.PublishDate > entity.LastPublishDate)
        {
            entity.LastPublishDate = entity.PublishDate;
        }
    }
}
