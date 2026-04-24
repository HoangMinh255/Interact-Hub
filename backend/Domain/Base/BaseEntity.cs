namespace InteractHub.Domain.Base;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}