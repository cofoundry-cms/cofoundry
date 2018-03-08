using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetNestedDataModelSchemaByNameQuery : IQuery<NestedDataModelSchema>
    {
        public GetNestedDataModelSchemaByNameQuery()
        {
        }

        public GetNestedDataModelSchemaByNameQuery(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
