namespace CartorioTwin.Domain.Documentos;

public readonly record struct AtoRegistralId
{
    private AtoRegistralId(Guid valor)
    {
        Valor = valor;
    }

    public Guid Valor { get; }

    public static AtoRegistralId Criar(Guid valor)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(valor, Guid.Empty);
        return new AtoRegistralId(valor);
    }

    public override string ToString()
    {
        return Valor.ToString("D");
    }
}
