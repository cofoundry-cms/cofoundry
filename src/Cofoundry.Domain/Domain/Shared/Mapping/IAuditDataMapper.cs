using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
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
        /// Maps an EF model that inherits from ICreateAuditable into a UpdateAuditData object
        /// using only the creator information from the model. Useful when you are mapping audit
        /// information from two different objects. If the db record is null then an ArgumentNullException is thrown.
        /// </summary>
        /// <param name="model">ICreateAuditable EF database record to map create data from.</param>
        UpdateAuditData MapUpdateAuditDataCreatorData(ICreateAuditable model);

        /// <summary>
        /// Updates an UpdateAuditData object  using the creator information from the model as the updater 
        /// information. Useful when you are mapping audit information from two different objects. If the 
        /// db record is null then an ArgumentNullException is thrown.
        /// </summary>
        /// <param name="model">ICreateAuditable EF database record to map create data from.</param>
        void MapUpdateAuditDataUpdaterData(UpdateAuditData updateAuditDatra, ICreateAuditable model);
    }
}
