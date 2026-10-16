namespace CartorioTwin.Domain.Identidades;

public readonly record struct UsuarioId
{
    private UsuarioId(Guid valor)
    {
        Valor = valor;
    }

    public Guid Valor { get; }

    public static UsuarioId Criar(Guid valor)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(valor, Guid.Empty);
        return new UsuarioId(valor);
    }

    public override string ToString()
    {
        return Valor.ToString("D");
    }
}
