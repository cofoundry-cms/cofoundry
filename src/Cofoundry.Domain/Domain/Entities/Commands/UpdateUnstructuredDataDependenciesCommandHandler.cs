using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class UpdateUnstructuredDataDependenciesCommandHandler 
        : ICommandHandler<UpdateUnstructuredDataDependenciesCommand>
        , IAsyncCommandHandler<UpdateUnstructuredDataDependenciesCommand>
        , IPermissionRestrictedCommandHandler<UpdateUnstructuredDataDependenciesCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IEntityDefinitionRepository _entityDefinitionRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly ICommandExecutor _commandExecutor;

        public UpdateUnstructuredDataDependenciesCommandHandler(
            CofoundryDbContext dbContext,
            IEntityDefinitionRepository entityDefinitionRepository,
            IPermissionRepository permissionRepository,
            ICommandExecutor commandExecutor
            )
        {
            _dbContext = dbContext;
            _entityDefinitionRepository = entityDefinitionRepository;
            _permissionRepository = permissionRepository;
            _commandExecutor = commandExecutor;
        }

        #endregion

        #region Execute

        public async Task ExecuteAsync(UpdateUnstructuredDataDependenciesCommand command, IExecutionContext executionContext)
        {
            var existingDependencies = await QueryDepenencies(command).ToListAsync();
            var relations = GetDistinctRelations(command.Model);
            var ensureEntityDefinitionExistsCommands = GetEntityCheckCommands(command, existingDependencies, relations);

            foreach (var ensureEntityDefinitionExistsCommand in ensureEntityDefinitionExistsCommands)
            {
                await _commandExecutor.ExecuteAsync(ensureEntityDefinitionExistsCommand, executionContext);
            }

            ApplyChanges(command, existingDependencies, relations);
            await _dbContext.SaveChangesAsync();
        }

        public void Execute(UpdateUnstructuredDataDependenciesCommand command, IExecutionContext executionContext)
        {
            var existingDependencies = QueryDepenencies(command).ToList();
            var relations = GetDistinctRelations(command.Model);
            var ensureEntityDefinitionExistsCommands = GetEntityCheckCommands(command, existingDependencies, relations);

            foreach (var ensureEntityDefinitionExistsCommand in ensureEntityDefinitionExistsCommands)
            {
                _commandExecutor.Execute(ensureEntityDefinitionExistsCommand, executionContext);
            }
            ApplyChanges(command, existingDependencies, relations);
            _dbContext.SaveChanges();
        }

        private IQueryable<UnstructuredDataDependency> QueryDepenencies(UpdateUnstructuredDataDependenciesCommand command)
        {
            return _dbContext
                .UnstructuredDataDependencies
                .Where(d => d.RootEntityDefinitionCode == command.RootEntityDefinitionCode && d.RootEntityId == command.RootEntityId);
        }

        private IEnumerable<EntityDependency> GetDistinctRelations(object model)
        {
            var relations = GetRelations(model);

            foreach (var relationGroup in relations.GroupBy(r => new { r.EntityDefinitionCode, r.EntityId }))
            {
                if (relationGroup.Count() == 1)
                {
                    yield return relationGroup.First();
                }
                else
                {
                    // In the case of multiple relations of the same entity we take the most harmful first, prefering a warning.
                    var relation = relationGroup
                        //.OrderByDescending(r => r.RootEntityCascadeAction == RootEntityCascadeAction.WarnAndCascade)
                        //.ThenByDescending(r => r.RootEntityCascadeAction == RootEntityCascadeAction.Cascade)
                        .First();

                    // Here we take the most restrictive first, which is to prevent deletion.
                    relation.RelatedEntityCascadeAction = relationGroup
                        .Select(r => r.RelatedEntityCascadeAction)
                        .OrderByDescending(r => r == RelatedEntityCascadeAction.None)
                        .ThenByDescending(r => r == RelatedEntityCascadeAction.CascadeProperty)
                        //.OrderByDescending(r => r == RelatedEntityCascadeAction.WarnAndCascadeEntity)
                        .First();

                    yield return relation;
                }
            }
        }

        private IEnumerable<EntityDependency> GetRelations(object model)
        {
            var relationProperties = model.GetType()
                .GetProperties()
                .Select(p => new { Property = p, Attribute = p.GetCustomAttributes(typeof(IEntityRelationAttribute), false).FirstOrDefault() })
                .Where(p => p.Attribute != null);

            foreach (var relationProperty in relationProperties)
            {
                var attribtue = (IEntityRelationAttribute)relationProperty.Attribute;
                foreach (var relation in attribtue.GetRelations(model, relationProperty.Property))
                {
                    yield return relation;
                }
            }
        }

        private IEnumerable<EnsureEntityDefinitionExistsCommand> GetEntityCheckCommands(UpdateUnstructuredDataDependenciesCommand command, List<UnstructuredDataDependency> existingDependencies, IEnumerable<EntityDependency> relations)
        {
            var commands = relations
                .Select(r => r.EntityDefinitionCode)
                .Union(new string[] { command.RootEntityDefinitionCode })
                .Except(existingDependencies.Select(r => r.RelatedEntityDefinitionCode))
                .Distinct()
                .Select(c => new EnsureEntityDefinitionExistsCommand(c));

            return commands;
        }

        private void ApplyChanges(UpdateUnstructuredDataDependenciesCommand command, List<UnstructuredDataDependency> existingDependencies, IEnumerable<EntityDependency> relations)
        {
            var relationsList = relations.ToList();

            foreach (var existingDependency in existingDependencies)
            {
                var updatedRelation = relationsList.SingleOrDefault(r => r.EntityDefinitionCode == existingDependency.RelatedEntityDefinitionCode && r.EntityId == existingDependency.RelatedEntityId);

                if (updatedRelation == null)
                {
                    _dbContext.UnstructuredDataDependencies.Remove(existingDependency);
                }
                else
                {
                    existingDependency.RelatedEntityCascadeActionId = (int)updatedRelation.RelatedEntityCascadeAction;
                }
                relationsList.Remove(updatedRelation);
            }

            foreach (var newRelation in relationsList)
            {
                _dbContext.UnstructuredDataDependencies.Add(new UnstructuredDataDependency()
                {
                    RootEntityDefinitionCode = command.RootEntityDefinitionCode,
                    RootEntityId = command.RootEntityId,
                    RelatedEntityDefinitionCode = newRelation.EntityDefinitionCode,
                    RelatedEntityId = newRelation.EntityId,
                    RelatedEntityCascadeActionId = (int)newRelation.RelatedEntityCascadeAction
                });
            }
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(UpdateUnstructuredDataDependenciesCommand command)
        {
            var entityDefinition = _entityDefinitionRepository.GetByCode(command.RootEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(entityDefinition, command.RootEntityDefinitionCode);

            // Try and get a delete permission for the root entity.
            var permission = _permissionRepository.GetByEntityAndPermissionType(entityDefinition, CommonPermissionTypes.Update("Entity"));

            if (permission != null)
            {
                yield return permission;
            }
        }

        #endregion
    }
}
