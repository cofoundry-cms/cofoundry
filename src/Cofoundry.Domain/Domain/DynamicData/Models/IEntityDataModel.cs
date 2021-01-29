using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// <para>
    /// Marker interface to denote a data model that
    /// can be used to extend the properties of an entity.
    /// This data is serialized as unstructured data in the
    /// database alongside the entity. Examples include
    /// ICustomEntityDataModel and IPageBlockTypeDataModel.
    /// </para>
    /// <para>
    /// This marker interface is important for classifying which
    /// classes can be serialized/deserialized into the database.
    /// for security it's important to be able to restrict which
    /// classes can be deserialized to.
    /// </para>
    /// </summary>
    public interface IEntityDataModel
    {
    }
}
