using Cofoundry.Core.Validation;
using System.ComponentModel.DataAnnotations;

namespace SPASite.Domain;

/// <summary>
/// Here we can use ILoggableCommand to mark this class as
/// loggable. The default logger only logs to the debugger,
/// but you can use a plugin to extend this functionality
/// </summary>
public class SetCatLikedCommand : ICommand, ILoggableCommand
{
    [PositiveInteger]
    [Required]
    public int CatId { get; set; }

    public bool IsLiked { get; set; }
}