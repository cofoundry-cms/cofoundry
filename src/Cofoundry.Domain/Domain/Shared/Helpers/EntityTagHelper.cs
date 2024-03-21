﻿using Cofoundry.Domain.Data;
using System.Text.RegularExpressions;

namespace Cofoundry.Domain;

/// <summary>
/// A helper for working entities which can be tagged.
/// </summary>
public partial class EntityTagHelper
{
    private readonly CofoundryDbContext _dbContext;
    private readonly EntityAuditHelper _entityAuditHelper;

    public EntityTagHelper(
        CofoundryDbContext dbContext,
        EntityAuditHelper entityAuditHelper
        )
    {
        _dbContext = dbContext;
        _entityAuditHelper = entityAuditHelper;
    }

    /// <summary>
    /// Updates a collection of entity tags with tags parsed from a user
    /// entered string.
    /// </summary>
    /// <typeparam name="TEntityTag">type of tags to update e.g. PageTag. You will need to mark up the Data class with IEntityTag</typeparam>
    /// <param name="existingTagCollection">Collection of existing tags to update (can be empty for a new entity)</param>
    /// <param name="updatedTags">Collection of updated tags to merge</param>
    /// <param name="executionContext">The execution context that captures the state of the command being executed.</param>
    public void UpdateTags<TEntityTag>(ICollection<TEntityTag> existingTagCollection, IEnumerable<string?>? updatedTags, IExecutionContext executionContext)
        where TEntityTag : class, IEntityTag, new()
    {
        ArgumentNullException.ThrowIfNull(existingTagCollection);

        var cleanedTags = CleanTags(updatedTags ?? Enumerable.Empty<string>());

        var entitySet = _dbContext.Set<TEntityTag>();
        var exitingTags = existingTagCollection
            .Select(t => new
            {
                EntityTag = t,
                t.Tag.TagText
            })
            .ToList();

        // Delete removed tags
        var entityTagsToRemove = exitingTags
            .Where(et => !cleanedTags.Any(pt => pt.Equals(et.TagText, StringComparison.OrdinalIgnoreCase)));

        foreach (var tagToRemove in entityTagsToRemove)
        {
            entitySet.Remove(tagToRemove.EntityTag);
        }

        // Add any new tags
        var entityTagsToAdd = cleanedTags
            .Where(nt => !exitingTags.Any(et => nt.Equals(et.TagText, StringComparison.OrdinalIgnoreCase)));

        var tags = _dbContext
            .Tags
            .Where(t => entityTagsToAdd.Contains(t.TagText))
            .ToArray();

        foreach (var tagText in entityTagsToAdd)
        {
            var entityTag = new TEntityTag();
            _entityAuditHelper.SetCreated(entityTag, executionContext);

            var existingTag = tags.FirstOrDefault(t => t.TagText.Equals(tagText, StringComparison.OrdinalIgnoreCase));

            if (existingTag != null)
            {
                entityTag.Tag = existingTag;
            }
            else
            {
                entityTag.Tag = new Tag
                {
                    CreateDate = executionContext.ExecutionDate,
                    TagText = tagText
                };
            }

            existingTagCollection.Add(entityTag);
        }

    }

    /// <summary>
    /// Updates a collection of entity tags with tags parsed from a user
    /// entered string.
    /// </summary>
    /// <typeparam name="TEntityTag">type of tags to update e.g. PageTag. You will need to mark up the Data class with IEntityTag</typeparam>
    /// <param name="existingTagCollection">Collection of existing tags to update (can be empty for a new entity)</param>
    /// <param name="unparsedTags">Unparsed tag text string e.g. Waffle, Dog, Bannana, "Danish Pastry"</param>
    /// <param name="executionContext">The execution context that captures the state of the command being executed.</param>
    public void UpdateTags<TEntityTag>(ICollection<TEntityTag> existingTagCollection, string unparsedTags, IExecutionContext executionContext)
        where TEntityTag : class, IEntityTag, new()
    {
        UpdateTags(existingTagCollection, TagParser.Split(unparsedTags), executionContext);
    }

    /// <summary>
    /// Renders tags into a single string, comma delimited and multiple words escaped in quotes.
    /// </summary>
    /// <param name="tags">String tags to parse</param>
    public string ToDelimitedString(IEnumerable<string> tags)
    {
        var output = string.Join(",", tags.Select(FormatTag));
        return output;
    }

    private static string FormatTag(string tag)
    {
        if (tag.Contains(' '))
        {
            return "\"" + tag + "\"";
        }

        return tag;
    }


    private List<string> CleanTags(IEnumerable<string?> tags)
    {
        var cleanedTags = tags
            .WhereNotNullOrEmpty()
            .Select(t => CleanTagsRegex().Replace(t, string.Empty)
                .Trim()
                .TrimStart(['&', '-', ')'])
                .TrimEnd(['&', '-', '(']))
            .Select(TextFormatter.FirstLetterToUpperCase)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .ToList();

        return cleanedTags;
    }

    [GeneratedRegex(@"[^&\w\s'()-]+")]
    private static partial Regex CleanTagsRegex();
}
