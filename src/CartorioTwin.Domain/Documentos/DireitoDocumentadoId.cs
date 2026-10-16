namespace CartorioTwin.Domain.Documentos;

public readonly record struct DireitoDocumentadoId
{
    private DireitoDocumentadoId(Guid valor)
    {
        Valor = valor;
    }

    public Guid Valor { get; }

    public static DireitoDocumentadoId Criar(Guid valor)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(valor, Guid.Empty);
        return new DireitoDocumentadoId(valor);
    }

    public override string ToString()
    {
        return Valor.ToString("D");
    }
}
