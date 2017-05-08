using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Helper for mapping page and custom entity version module data from an 
    /// unstructured db source to a display model instance.
    /// </summary>
    public class PageVersionModuleModelMapper : IPageVersionModuleModelMapper
    {
        private static readonly MethodInfo _mapGenericMethod = typeof(PageVersionModuleModelMapper).GetMethod(nameof(MapGeneric), BindingFlags.NonPublic | BindingFlags.Instance);

        #region constructor

        private readonly IResolutionContext _resolutionContext;
        private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;
        private readonly IPageModuleDataModelTypeFactory _moduleDataModelTypeFactory;

        public PageVersionModuleModelMapper(
            IResolutionContext resolutionContext,
            IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
            IPageModuleDataModelTypeFactory moduleDataModelTypeFactory
            )
        {
            _resolutionContext = resolutionContext;
            _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
            _moduleDataModelTypeFactory = moduleDataModelTypeFactory;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Maps a batch of the same type of page module data to a collection
        /// of display models ready for rendering.
        /// </summary>
        /// <param name="typeName">The module type name e.g. 'PlainText', 'RawHtml'.</param>
        /// <param name="versionModule">The version data to get the serialized model from.</param>
        /// <param name="workflowStatus">
        /// The workflow status of the parent page or custom entity 
        /// being mapped. This is provided so dependent entities can use
        /// the same workflow status.
        /// </param>
        /// <returns>
        /// Collection of mapped display models, wrapped in an output class that
        /// can be used to identify them.
        /// </returns>
        public async Task<List<PageModuleDisplayModelMapperOutput>> MapDisplayModelAsync(
            string typeName, 
            IEnumerable<IEntityVersionPageModule> versionModules, 
            WorkFlowStatusQuery workflowStatus
            )
        {
            // Find the data-provider class for this type of module
            Type modelType = _moduleDataModelTypeFactory.CreateByPageModuleTypeFileName(typeName);
            
            if (typeof(IPageModuleDisplayModel).IsAssignableFrom(modelType))
            {
                // We can serialize directly to the display model
                var displayModels = new List<PageModuleDisplayModelMapperOutput>();
                foreach (var pageModule in versionModules)
                {
                    var mapperModel = new PageModuleDisplayModelMapperOutput();
                    mapperModel.DisplayModel = (IPageModuleDisplayModel)_dbUnstructuredDataSerializer.Deserialize(pageModule.SerializedData, modelType);
                    mapperModel.VersionModuleId = pageModule.GetVersionModuleId();
                    displayModels.Add(mapperModel);
                }

                return displayModels;
            }
            else
            {
                var moduleWorkflowStatus = TranslateWorkFlowStatusForModules(workflowStatus);

                // We have to use a mapping class to do some custom mapping
                var displayModels = (Task<List<PageModuleDisplayModelMapperOutput>>)_mapGenericMethod
                    .MakeGenericMethod(modelType)
                    .Invoke(this, new object[] { versionModules, moduleWorkflowStatus });

                return await displayModels;
            }
        }

        /// <summary>
        /// Maps a single page module data model to a concrete
        /// display model.
        /// </summary>
        /// <param name="typeName">The module type name e.g. 'PlainText', 'RawHtml'.</param>
        /// <param name="versionModule">The version data to get the serialized model from.</param>
        /// <param name="workflowStatus">
        /// The workflow status of the parent page or custom entity 
        /// being mapped. This is provided so dependent entities can use
        /// the same workflow status.
        /// </param>
        /// <returns>Mapped display model.</returns>
        public async Task<IPageModuleDisplayModel> MapDisplayModelAsync(
            string typeName,
            IEntityVersionPageModule versionModule, 
            WorkFlowStatusQuery workflowStatus
            )
        {
            var mapped = await MapDisplayModelAsync(typeName, new IEntityVersionPageModule[] { versionModule }, workflowStatus);
            return mapped
                .Select(m => m.DisplayModel)
                .SingleOrDefault();
        }

        /// <summary>
        /// Deserialized a module data model to a stongly typed model.
        /// </summary>
        /// <param name="typeName">The module type name e.g. 'PlainText', 'RawHtml'.</param>
        /// <param name="versionModule">The version data to get the serialized model from.</param>
        /// <returns>Strongly typed data model including deserialized data.</returns>
        public IPageModuleDataModel MapDataModel(string typeName, IEntityVersionPageModule versionModule)
        {
            Type modelType = _moduleDataModelTypeFactory.CreateByPageModuleTypeFileName(typeName);
            var model = (IPageModuleDataModel)_dbUnstructuredDataSerializer.Deserialize(versionModule.SerializedData, modelType);

            return model;
        }

        #endregion

        #region privates

        /// <summary>
        /// When working with child entities, the WorkFlosStatus we apply to
        /// them is not neccessarily the status used to query the parent. If we are 
        /// loading a page using the Draft status, then we cannot expect that all 
        /// dependencies should have a draft version, so we re-write it to Latest.
        /// The same applies if we're loading a specific version.
        /// </summary>
        /// <param name="workflowStatus">The original workflow status of the parent entity.</param>
        private WorkFlowStatusQuery TranslateWorkFlowStatusForModules(WorkFlowStatusQuery workflowStatus)
        {
            if (workflowStatus == WorkFlowStatusQuery.Draft || workflowStatus == WorkFlowStatusQuery.SpecificVersion)
            {
                workflowStatus = WorkFlowStatusQuery.Latest;
            }

            return workflowStatus;
        }

        private async Task<List<PageModuleDisplayModelMapperOutput>> MapGeneric<T>(IEnumerable<IEntityVersionPageModule> versionModules, WorkFlowStatusQuery workflowStatus) where T : IPageModuleDataModel
        {
            var mapperType = typeof(IPageModuleDisplayModelMapper<T>);
            if (!_resolutionContext.IsRegistered(mapperType))
            {
                string msg = @"{0} does not implement IPageModuleDisplayModel and no custom mapper could be found. You must create 
                               a class that implements IPageModuleDisplayModelMapper<{0}> if you are using a custom display model. Full type name: {1}";
                throw new ApplicationException(string.Format(msg, typeof(T).Name, typeof(T).FullName));
            }

            var mapper = (IPageModuleDisplayModelMapper<T>)_resolutionContext.Resolve(typeof(IPageModuleDisplayModelMapper<T>));
            var dataModels = new List<PageModuleDisplayModelMapperInput<T>>();

            foreach (var pageModule in versionModules)
            {
                var mapperModel = new PageModuleDisplayModelMapperInput<T>();
                mapperModel.DataModel = (T)_dbUnstructuredDataSerializer.Deserialize(pageModule.SerializedData, typeof(T));
                mapperModel.VersionModuleId = pageModule.GetVersionModuleId();
                dataModels.Add(mapperModel);
            }

            var results = await mapper.MapAsync(dataModels, workflowStatus);

            return results.ToList();
        }

        #endregion
    }
}
