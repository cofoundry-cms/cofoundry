using Cofoundry.Core.AutoUpdate;

namespace Cofoundry.Domain.Installation;

public class RegisterPageTemplatesAndPageBlockTypesCommand : IAlwaysRunUpdateCommand
{
    public string Description
    {
        get
        {
            return "Update page templates, page block types & block type templates";
        }
    }
}
