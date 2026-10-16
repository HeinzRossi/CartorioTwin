namespace CartorioTwin.Domain.Documentos;

public readonly record struct AutorizacaoRevisaoId
{
    private AutorizacaoRevisaoId(Guid valor)
    {
        Valor = valor;
    }

    public Guid Valor { get; }

    public static AutorizacaoRevisaoId Criar(Guid valor)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(valor, Guid.Empty);
        return new AutorizacaoRevisaoId(valor);
    }

    public override string ToString()
    {
        return Valor.ToString("D");
    }
}
