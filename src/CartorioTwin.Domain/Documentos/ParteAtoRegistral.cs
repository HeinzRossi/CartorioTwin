using CartorioTwin.Domain.Identidades;

namespace CartorioTwin.Domain.Documentos;

public sealed record ParteAtoRegistral
{
    private ParteAtoRegistral(PessoaId? pessoaId, string rotuloDocumental)
    {
        PessoaId = pessoaId;
        RotuloDocumental = rotuloDocumental;
    }

    public PessoaId? PessoaId { get; }

    public string RotuloDocumental { get; }

    public static ParteAtoRegistral CriarIdentificada(PessoaId pessoaId, string rotuloDocumental)
    {
        if (pessoaId == default)
        {
            throw new ArgumentOutOfRangeException(nameof(pessoaId), "A identidade revisada não pode ser vazia.");
        }

        return new ParteAtoRegistral(pessoaId, SanitizarRotulo(rotuloDocumental));
    }

    public static ParteAtoRegistral CriarNaoIdentificada(string rotuloDocumental)
    {
        return new ParteAtoRegistral(null, SanitizarRotulo(rotuloDocumental));
    }

    private static string SanitizarRotulo(string rotuloDocumental)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rotuloDocumental);
        var rotuloSanitizado = rotuloDocumental.Trim();
        if (rotuloSanitizado.Any(char.IsControl))
        {
            throw new ArgumentException(
                "O rótulo documental não pode conter caracteres de controle.",
                nameof(rotuloDocumental));
        }

        return rotuloSanitizado;
    }
}
