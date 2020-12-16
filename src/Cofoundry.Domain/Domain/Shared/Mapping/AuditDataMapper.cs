using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to entity audit objects.
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

        /// <summary>
        /// Maps an EF model that inherits from ICreateAuditable into a CreateAuditData object. If the
        /// db record is null then an ArgumentNullException is thrown.
        /// </summary>
        /// <param name="model">ICreateAuditable EF database record.</param>
        public virtual CreateAuditData MapCreateAuditData(ICreateAuditable model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            ValidateUserProperty(model.Creator, nameof(model.Creator));

            var auditData = new CreateAuditData();
            auditData.CreateDate = DbDateTimeMapper.AsUtc(model.CreateDate);
            auditData.Creator = _userMicroSummaryMapper.Map(model.Creator);

            return auditData;
        }

        /// <summary>
        /// Maps an EF model that inherits from IUpdateAuditable into a UpdateAuditData object. If the
        /// db record is null then an ArgumentNullException is thrown.
        /// </summary>
        /// <param name="model">IUpdateAuditable EF database record.</param>
        public virtual UpdateAuditData MapUpdateAuditData(IUpdateAuditable model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            ValidateUserProperty(model.Creator, nameof(model.Creator));
            ValidateUserProperty(model.Updater, nameof(model.Updater));

            var auditData = new UpdateAuditData();
            auditData.CreateDate = DbDateTimeMapper.AsUtc(model.CreateDate);
            auditData.UpdateDate = DbDateTimeMapper.AsUtc(model.UpdateDate);
            auditData.Creator = _userMicroSummaryMapper.Map(model.Creator);
            auditData.Updater = _userMicroSummaryMapper.Map(model.Updater);

            return auditData;
        }

        /// <summary>
        /// Maps an EF model that inherits from ICreateAuditable into a UpdateAuditData object
        /// using only the creator information from the model. Useful when you are mapping audit
        /// information from two different objects. If the db record is null then an ArgumentNullException is thrown.
        /// </summary>
        /// <param name="model">ICreateAuditable EF database record to map create data from.</param>
        public virtual UpdateAuditData MapUpdateAuditDataCreatorData(ICreateAuditable model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            ValidateUserProperty(model.Creator, nameof(model.Creator));

            var auditData = new UpdateAuditData();
            auditData.CreateDate = DbDateTimeMapper.AsUtc(model.CreateDate);
            auditData.Creator = _userMicroSummaryMapper.Map(model.Creator);

            return auditData;
        }

        /// <summary>
        /// Updates an UpdateAuditData object  using the creator information from the model as the updater 
        /// information. Useful when you are mapping audit information from two different objects. If the 
        /// db record is null then an ArgumentNullException is thrown.
        /// </summary>
        /// <param name="model">ICreateAuditable EF database record to map create data from.</param>
        public virtual void MapUpdateAuditDataUpdaterData(UpdateAuditData updateAuditDatra, ICreateAuditable model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            ValidateUserProperty(model.Creator, nameof(model.Creator));

            updateAuditDatra.UpdateDate = DbDateTimeMapper.AsUtc(model.CreateDate);
            updateAuditDatra.Updater = _userMicroSummaryMapper.Map(model.Creator);
        }

        protected void ValidateUserProperty(User user, string name)
        {
            if (user == null)
            {
                throw new ArgumentException($"Entity has a null {name} property. Ensure it has been included in the query.", name);
            }
        }
    }
}
