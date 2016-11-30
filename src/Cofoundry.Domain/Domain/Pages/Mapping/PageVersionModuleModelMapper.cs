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
    /// Class for mapping page version module data from an unstructured db source to a model instance.
    /// </summary>
    public class PageVersionModuleModelMapper : IPageVersionModuleModelMapper
    {
        private static readonly MethodInfo _mapGenericMethod = typeof(PageVersionModuleModelMapper).GetMethod("MapGeneric", BindingFlags.NonPublic | BindingFlags.Instance);

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

        public IPageModuleDisplayModel MapDisplayModel(string typeName, IEntityVersionPageModule pageModule, WorkFlowStatusQuery workflowStatus)
        {
            return MapDisplayModel(typeName, new IEntityVersionPageModule[] { pageModule }, workflowStatus)
                .Select(m => m.DisplayModel)
                .SingleOrDefault();
        }

        public List<PageModuleDisplayModelMapperOutput> MapDisplayModel(string typeName, IEnumerable<IEntityVersionPageModule> versionModules, WorkFlowStatusQuery workflowStatus)
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
                // We have to use a mapping class to do some custom mapping
                var displayModels = (List<PageModuleDisplayModelMapperOutput>)_mapGenericMethod
                    .MakeGenericMethod(modelType)
                    .Invoke(this, new object[] { versionModules, workflowStatus });

                return displayModels;
            }
        }

        public IPageModuleDataModel MapDataModel(string typeName, IEntityVersionPageModule versionModule)
        {
            Type modelType = _moduleDataModelTypeFactory.CreateByPageModuleTypeFileName(typeName);
            var model = (IPageModuleDataModel)_dbUnstructuredDataSerializer.Deserialize(versionModule.SerializedData, modelType);

            return model;
        }

        #endregion

        #region privates

        private List<PageModuleDisplayModelMapperOutput> MapGeneric<T>(IEnumerable<IEntityVersionPageModule> versionModules, WorkFlowStatusQuery workflowStatus) where T : IPageModuleDataModel
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

            return mapper.Map(dataModels, workflowStatus).ToList();
        }

        #endregion
    }
}
