using Cofoundry.Core;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.BasicTestSite
{
    public class SidebarCategoriesViewComponent : ViewComponent
    {
        private readonly ICustomEntityRepository _customEntityRepository;

        public SidebarCategoriesViewComponent(
            ICustomEntityRepository customEntityRepository
            )
        {
            _customEntityRepository = customEntityRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var query = new SearchCustomEntityRenderSummariesQuery();
            query.CustomEntityDefinitionCode = CategoryCustomEntityDefinition.DefinitionCode;
            query.PageSize = 20;
            query.SortBy = CustomEntityQuerySortType.Title;
            query.PublishStatus = PublishStatusQuery.Published;

            var entities = await _customEntityRepository.SearchCustomEntityRenderSummariesAsync(query);
            var viewModel = MapCategories(entities);

            return View(viewModel);
        }

        private ICollection<CategorySummary> MapCategories(PagedQueryResult<CustomEntityRenderSummary> customEntityResult)
        {
            var categories = new List<CategorySummary>(customEntityResult.Items.Count());

            foreach (var customEntity in customEntityResult.Items)
            {
                var model = (CategoryDataModel)customEntity.Model;

                var category = new CategorySummary();
                category.CategoryId = customEntity.CustomEntityId;
                category.Title = customEntity.Title;
                category.ShortDescription = model.ShortDescription;

                categories.Add(category);
            }

            return categories;
        }
    }
}
