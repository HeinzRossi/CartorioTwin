namespace CartorioTwin.Domain.Documentos;

public readonly record struct RevisaoFatoDocumentalId
{
    private RevisaoFatoDocumentalId(Guid valor)
    {
        Valor = valor;
    }

    public Guid Valor { get; }

    public static RevisaoFatoDocumentalId Criar(Guid valor)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(valor, Guid.Empty);
        return new RevisaoFatoDocumentalId(valor);
    }

    public override string ToString()
    {
        return Valor.ToString("D");
    }
}
