using System.Globalization;

namespace CartorioTwin.Domain.Documentos;

public readonly record struct IdentificadorAtoRegistral
{
    private IdentificadorAtoRegistral(
        TipoIdentificadorAto tipo,
        long? numero,
        string textoOriginal)
    {
        Tipo = tipo;
        Numero = numero;
        TextoOriginal = textoOriginal;
    }

    public TipoIdentificadorAto Tipo { get; }

    public long? Numero { get; }

    public string TextoOriginal { get; }

    public string? ValorCanonico => Tipo switch
    {
        TipoIdentificadorAto.Registro => $"R-{Numero!.Value.ToString(CultureInfo.InvariantCulture)}",
        TipoIdentificadorAto.Averbacao => $"AV-{Numero!.Value.ToString(CultureInfo.InvariantCulture)}",
        _ => null,
    };

    public static IdentificadorAtoRegistral CriarRegistro(long numero, string? textoOriginal = null)
    {
        return CriarConhecido(TipoIdentificadorAto.Registro, numero, textoOriginal);
    }

    public static IdentificadorAtoRegistral CriarAverbacao(long numero, string? textoOriginal = null)
    {
        return CriarConhecido(TipoIdentificadorAto.Averbacao, numero, textoOriginal);
    }

    public static IdentificadorAtoRegistral CriarNaoIdentificado(string textoOriginal)
    {
        return new IdentificadorAtoRegistral(
            TipoIdentificadorAto.NaoIdentificado,
            null,
            SanitizarTextoOriginal(textoOriginal));
    }

    public override string ToString()
    {
        return TextoOriginal;
    }

    private static IdentificadorAtoRegistral CriarConhecido(
        TipoIdentificadorAto tipo,
        long numero,
        string? textoOriginal)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(numero);
        var numeroInvariante = numero.ToString(CultureInfo.InvariantCulture);
        var valorCanonico = tipo == TipoIdentificadorAto.Registro ? $"R-{numeroInvariante}" : $"AV-{numeroInvariante}";

        return new IdentificadorAtoRegistral(
            tipo,
            numero,
            textoOriginal is null ? valorCanonico : SanitizarTextoOriginal(textoOriginal));
    }

    private static string SanitizarTextoOriginal(string textoOriginal)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(textoOriginal);
        var textoSanitizado = textoOriginal.Trim();
        if (textoSanitizado.Any(char.IsControl))
        {
            throw new ArgumentException(
                "O identificador documental não pode conter caracteres de controle.",
                nameof(textoOriginal));
        }

        return textoSanitizado;
    }
}
