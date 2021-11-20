using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite.Domain
{
    public class GetBreedByIdQueryHandler
        : IQueryHandler<GetBreedByIdQuery, Breed>
        , IIgnorePermissionCheckHandler
    {
        private readonly IContentRepository _contentRepository;

        public GetBreedByIdQueryHandler(
            IContentRepository contentRepository
            )
        {
            _contentRepository = contentRepository;
        }

        public async Task<Breed> ExecuteAsync(GetBreedByIdQuery query, IExecutionContext executionContext)
        {
            var breed = await _contentRepository
                .CustomEntities()
                .GetById(query.BreedId)
                .AsRenderSummary()
                .Map(MapBreed)
                .ExecuteAsync();

            return breed;
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
