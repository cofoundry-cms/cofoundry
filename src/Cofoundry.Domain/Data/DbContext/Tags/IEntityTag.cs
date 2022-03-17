namespace Cofoundry.Domain.Data;

public interface IEntityTag : ICreateAuditable
{
    int TagId { get; set; }
    Tag Tag { get; set; }
}
