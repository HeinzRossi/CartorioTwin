namespace CartorioTwin.Domain.Identidades;

public readonly record struct ServentiaId
{
    private ServentiaId(Guid valor)
    {
        Valor = valor;
    }

    public Guid Valor { get; }

    public static ServentiaId Criar(Guid valor)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(valor, Guid.Empty);
        return new ServentiaId(valor);
    }

    public override string ToString()
    {
        return Valor.ToString("D");
    }
}
