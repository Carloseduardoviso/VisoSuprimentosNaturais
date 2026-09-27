namespace VisoERP.Domain.Entities;

public abstract class EntidadeBase
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
}
