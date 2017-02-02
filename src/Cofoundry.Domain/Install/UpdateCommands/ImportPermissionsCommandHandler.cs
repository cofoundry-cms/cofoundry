using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.AutoUpdate;

namespace Cofoundry.Domain.Installation
{
    /// <summary>
    /// This import job is to add all the permissions defined in code in
    /// a single batch job, reducing the burden of doing this individually 
    /// as roles are created
    /// </summary>
    public class ImportPermissionsCommandHandler : IAsyncVersionedUpdateCommandHandler<ImportPermissionsCommand>
    {
        private readonly Database _db;
        private readonly IPermissionRepository _permissionRepository;

        public ImportPermissionsCommandHandler(
            Database db,
            IPermissionRepository permissionRepository
            )
        {
            _db = db;
            _permissionRepository = permissionRepository;
        }


        public Task ExecuteAsync(ImportPermissionsCommand command)
        {
            var sb = new StringBuilder();

            foreach (var permission in _permissionRepository.GetAll())
            {
                if (permission is IEntityPermission)
                {
                    var entityDefinition = ((IEntityPermission)permission).EntityDefinition;

                    sb.AppendLine(string.Format("if not exists (select * from Cofoundry.[EntityDefinition] where EntityDefinitionCode = '{0}')", entityDefinition.EntityDefinitionCode));
                    sb.AppendLine("begin");
                    sb.AppendLine(string.Format("insert into Cofoundry.[EntityDefinition] (EntityDefinitionCode, Name) values ('{0}', '{1}')", entityDefinition.EntityDefinitionCode, entityDefinition.Name));
                    sb.AppendLine("end");
                    sb.AppendLine(string.Format("insert into Cofoundry.[Permission] (EntityDefinitionCode, PermissionCode) values ('{0}', '{1}')", entityDefinition.EntityDefinitionCode, permission.PermissionType.Code));
                }
                else
                {
                    sb.AppendLine(string.Format("insert into Cofoundry.[Permission] (PermissionCode) values ('{0}')", permission.PermissionType.Code));
                }
            }

            var sql = sb.ToString();
            _db.Execute(sql);

            return Task.FromResult(true);
        }
    }
}
