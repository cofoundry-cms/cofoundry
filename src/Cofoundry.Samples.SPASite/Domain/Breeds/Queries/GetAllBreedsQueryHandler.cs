using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite.Domain
{
    /// <summary>
    /// This query uses IIgnorePermissionCheckHandler because the permissions
    /// are already checked by the underlying custom entity query.
    /// </summary>
    public class GetAllBreedsQueryHandler
        : IQueryHandler<GetAllBreedsQuery, IEnumerable<Breed>>
        , IIgnorePermissionCheckHandler
    {
        private readonly IContentRepository _contentRepository;

        public GetAllBreedsQueryHandler(
            IContentRepository contentRepository
            )
        {
            _contentRepository = contentRepository;
        }

        public async Task<IEnumerable<Breed>> ExecuteAsync(GetAllBreedsQuery query, IExecutionContext executionContext)
        {
            var breeds = await _contentRepository
                .CustomEntities()
                .GetByDefinition<BreedCustomEntityDefinition>()
                .AsRenderSummary()
                .MapItem(MapBreed)
                .ExecuteAsync();

            return breeds;
        }

        /// <summary>
        /// For simplicity this logic is just repeated between handlers, but to 
        /// reduce repetition you could use a library like AutoMapper or break out
        /// the logic into a seperate mapper class and inject it in.
        /// </summary>
        private Breed MapBreed(CustomEntityRenderSummary customEntity)
        {
            var breed = new Breed();

            breed.BreedId = customEntity.CustomEntityId;
            breed.Title = customEntity.Title;

            return breed;
        }
    }
}
