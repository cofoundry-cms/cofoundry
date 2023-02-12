namespace Cofoundry.Domain.Data;

public interface IDataExtendable
{
    /// <summary>
    /// Serialized data from <see cref="IEntityExtensionDataModel"/> used to entend this
    /// entity. Data models are serialized into string data by <see cref="IDbUnstructuredDataSerializer"/>, 
    /// which used JSON serlialization by default.
    /// </summary>
    string SerializedExtensionData { get; set; }
}
