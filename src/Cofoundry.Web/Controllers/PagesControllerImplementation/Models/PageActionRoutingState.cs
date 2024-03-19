﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Cofoundry.Web;

/// <summary>
/// Keeps track of any variables that are required through the
/// Page routing pipeline.
/// </summary>
public class PageActionRoutingState
{
    /// <summary>
    /// Locale of the page, or null if locales aren't supported.
    /// </summary>
    public ActiveLocale? Locale { get; set; }

    /// <summary>
    /// Represents the input parameters of the Page action.
    /// </summary>
    public required PageActionInputParameters InputParameters { get; set; }

    /// <summary>
    /// True if the currently logged in user is authenticated
    /// under the Cofoundry admin schema.
    /// </summary>
    [MemberNotNullWhen(true, nameof(CofoundryAdminUserContext))]
    [MemberNotNullWhen(true, nameof(CofoundryAdminExecutionContext))]
    public bool IsCofoundryAdminUser { get; set; }

    /// <summary>
    /// The current user context for the request using the ambient
    /// auth schema. The default schema is the Cofoundry
    /// admin user area, but this could be changed.
    /// </summary>
    public IUserContext? AmbientUserContext { get; set; }

    /// <summary>
    /// User context representing the logged in Cofoundry admin user, or
    /// null if the user is not logged into the admin auth schema.
    /// </summary>
    public IUserContext? CofoundryAdminUserContext { get; set; }

    /// <summary>
    /// An execution context wrapping the CofoundryUserContext.
    /// </summary>
    public IExecutionContext? CofoundryAdminExecutionContext { get; set; }

    /// <summary>
    /// The requested visual editor state
    /// </summary>
    public VisualEditorState? VisualEditorState { get; set; }

    /// <summary>
    /// A PageRoute if one is found during the pipeline process.
    /// </summary>
    public PageRoutingInfo? PageRoutingInfo { get; set; }

    /// <summary>
    /// The resulting page data if one is found during the pipeline process.
    /// </summary>
    public PageRenderDetails? PageData { get; set; }

    /// <summary>
    /// An action result to return from the Page action. Set this at any point in the
    /// pipline and it will be returned after the current method has finished executing.
    /// </summary>
    public IActionResult? Result { get; set; }
}
