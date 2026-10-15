namespace CartorioTwin.Domain.Documentos;

public readonly record struct FonteDocumentalId
{
    private FonteDocumentalId(Guid valor)
    {
        Valor = valor;
    }

    public Guid Valor { get; }

    public static FonteDocumentalId Criar(Guid valor)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(valor, Guid.Empty);
        return new FonteDocumentalId(valor);
    }

    public override string ToString()
    {
        return Valor.ToString("D");
    }
}
