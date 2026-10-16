namespace CartorioTwin.Domain.Documentos;

public readonly record struct FatoDocumentalId
{
    private FatoDocumentalId(Guid valor)
    {
        Valor = valor;
    }

    public Guid Valor { get; }

    public static FatoDocumentalId Criar(Guid valor)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(valor, Guid.Empty);
        return new FatoDocumentalId(valor);
    }

    public override string ToString()
    {
        return Valor.ToString("D");
    }
}
