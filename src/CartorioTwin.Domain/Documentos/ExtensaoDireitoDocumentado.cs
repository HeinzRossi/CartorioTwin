using CartorioTwin.Domain.Shared;

namespace CartorioTwin.Domain.Documentos;

public readonly record struct ExtensaoDireitoDocumentado
{
    private ExtensaoDireitoDocumentado(TipoExtensaoDireitoDocumentado tipo, Fracao? fracaoDoTotal)
    {
        Tipo = tipo;
        FracaoDoTotal = fracaoDoTotal;
    }

    public static ExtensaoDireitoDocumentado NaoIdentificada => default;

    public static ExtensaoDireitoDocumentado Total { get; } =
        new(TipoExtensaoDireitoDocumentado.Total, Fracao.Um);

    public TipoExtensaoDireitoDocumentado Tipo { get; }

    public Fracao? FracaoDoTotal { get; }

    public static ExtensaoDireitoDocumentado CriarFracaoDoTotal(Fracao fracao)
    {
        if (fracao <= Fracao.Zero || fracao > Fracao.Um)
        {
            throw new ArgumentOutOfRangeException(
                nameof(fracao),
                "A extensão fracionária deve ser maior que zero e menor ou igual ao total.");
        }

        return fracao == Fracao.Um
            ? Total
            : new ExtensaoDireitoDocumentado(TipoExtensaoDireitoDocumentado.FracaoDoTotal, fracao);
    }

    public override string ToString()
    {
        return Tipo switch
        {
            TipoExtensaoDireitoDocumentado.NaoIdentificada => "não identificada",
            TipoExtensaoDireitoDocumentado.Total => Fracao.Um.ToString(),
            TipoExtensaoDireitoDocumentado.FracaoDoTotal => FracaoDoTotal!.Value.ToString(),
            _ => throw new InvalidOperationException("Tipo de extensão inválido."),
        };
    }
}
