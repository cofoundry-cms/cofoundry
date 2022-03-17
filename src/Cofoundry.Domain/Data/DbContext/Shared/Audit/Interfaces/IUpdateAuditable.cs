namespace Cofoundry.Domain.Data;

public interface IUpdateAuditable : ICreateAuditable
{
    User Updater { get; set; }
    DateTime UpdateDate { get; set; }
    int UpdaterId { get; set; }
}
