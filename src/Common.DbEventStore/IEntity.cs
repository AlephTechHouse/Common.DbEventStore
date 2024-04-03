namespace Common.DbEventStore
{
    public interface IEntity
    {
        Guid TenantId { get; set; }
        Guid Id { get; set; }
    }
}

