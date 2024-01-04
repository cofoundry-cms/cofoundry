using Cofoundry.Domain.Data;

namespace Cofoundry.Domain;

/// <summary>
/// Simple mapper for mapping to entity audit objects.
/// </summary>
public interface IAuditDataMapper
{
    /// <summary>
    /// Maps an EF model that inherits from ICreateAuditable into a CreateAuditData object. If the
    /// db record is null then an ArgumentNullException is thrown.
    /// </summary>
    /// <param name="model">ICreateAuditable EF database record.</param>
    CreateAuditData MapCreateAuditData(ICreateAuditable model);

    /// <summary>
    /// Maps an EF model that inherits from IUpdateAuditable into a UpdateAuditData object. If the
    /// db record is null then an ArgumentNullException is thrown.
    /// </summary>
    /// <param name="model">IUpdateAuditable EF database record.</param>
    UpdateAuditData MapUpdateAuditData(IUpdateAuditable model);

    /// <summary>
    /// Maps update audit information based on two EF models. Typically this is 
    /// used where the create model is the root entity and the update model is a 
    /// version record. If the db record is null then an <see cref="ArgumentNullException"/> is thrown.
    /// </summary>
    /// <param name="createModel">
    /// The <see cref="ICreateAuditable"/> EF database record containing the create audit data.
    /// </param>
    /// <param name="updateModel">
    /// The <see cref="ICreateAuditable"/> EF database record to take update audit information 
    /// from. As this is expected to be a versioned record The creator and create date fields will be used for
    /// </param>
    UpdateAuditData MapUpdateAuditDataFromVersion<TVersionModel>(ICreateAuditable createModel, TVersionModel versionModel)
        where TVersionModel : IEntityVersion, ICreateAuditable;
}
