namespace CartorioTwin.Domain.Identidades;

public readonly record struct ImovelId
{
    private ImovelId(Guid valor)
    {
        Valor = valor;
    }

    public Guid Valor { get; }

    public static ImovelId Criar(Guid valor)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(valor, Guid.Empty);
        return new ImovelId(valor);
    }

    public override string ToString()
    {
        return Valor.ToString("D");
    }
}
