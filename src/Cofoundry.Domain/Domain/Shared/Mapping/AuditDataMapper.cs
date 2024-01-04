using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default iumplementation of <see cref="IAuditDataMapper"/>.
/// </summary>
public class AuditDataMapper : IAuditDataMapper
{
    private readonly IUserMicroSummaryMapper _userMicroSummaryMapper;

    public AuditDataMapper(
        IUserMicroSummaryMapper userMicroSummaryMapper
        )
    {
        _userMicroSummaryMapper = userMicroSummaryMapper;
    }

    /// <inheritdoc/>
    public virtual CreateAuditData MapCreateAuditData(ICreateAuditable model)
    {
        ArgumentNullException.ThrowIfNull(model);
        ValidateUserProperty(model.Creator, nameof(model.Creator));

        var auditData = new CreateAuditData
        {
            CreateDate = model.CreateDate,
            Creator = _userMicroSummaryMapper.Map(model.Creator)
        };

        return auditData;
    }

    /// <inheritdoc/>
    public virtual UpdateAuditData MapUpdateAuditData(IUpdateAuditable model)
    {
        ArgumentNullException.ThrowIfNull(model);
        ValidateUserProperty(model.Creator, nameof(model.Creator));
        ValidateUserProperty(model.Updater, nameof(model.Updater));

        var auditData = new UpdateAuditData
        {
            CreateDate = model.CreateDate,
            UpdateDate = model.UpdateDate,
            Creator = _userMicroSummaryMapper.Map(model.Creator),
            Updater = _userMicroSummaryMapper.Map(model.Updater)
        };

        return auditData;
    }

    /// <inheritdoc/>
    public virtual UpdateAuditData MapUpdateAuditDataFromVersion<TVersionModel>(ICreateAuditable createModel, TVersionModel versionModel)
        where TVersionModel : IEntityVersion, ICreateAuditable
    {
        ArgumentNullException.ThrowIfNull(createModel);
        ArgumentNullException.ThrowIfNull(versionModel);
        ValidateUserProperty(createModel.Creator, nameof(createModel.Creator));
        ValidateUserProperty(versionModel.Creator, nameof(versionModel.Creator));

        var auditData = new UpdateAuditData
        {
            CreateDate = createModel.CreateDate,
            UpdateDate = versionModel.CreateDate,
            Creator = _userMicroSummaryMapper.Map(createModel.Creator),
            Updater = _userMicroSummaryMapper.Map(versionModel.Creator)
        };

        return auditData;
    }

    protected static void ValidateUserProperty(User user, string name)
    {
        if (user == null)
        {
            throw new ArgumentException($"Entity has a null {name} property. Ensure it has been included in the query.", name);
        }
    }
}
