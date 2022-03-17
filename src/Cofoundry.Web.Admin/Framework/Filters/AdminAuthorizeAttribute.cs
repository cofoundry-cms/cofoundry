namespace Cofoundry.Web.Admin;

public class AdminAuthorizeAttribute : AuthorizeUserAreaAttribute
{
    public AdminAuthorizeAttribute()
        : base(CofoundryAdminUserArea.Code)
    {
    }
}
