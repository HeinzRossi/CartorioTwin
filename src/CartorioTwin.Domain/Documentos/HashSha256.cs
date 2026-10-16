using System.Security.Cryptography;
using System.Text;

namespace CartorioTwin.Domain.Documentos;

public sealed record HashSha256
{
    private const int ComprimentoHexadecimal = 64;
    private const string PrefixoCanonico = "sha256:";

    private HashSha256(string valor)
    {
        Valor = valor;
    }

    public string Valor { get; }

    public string ValorCanonico => $"sha256:{Valor}";

    public static HashSha256 Criar(string valor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(valor);

        if (valor.Length != ComprimentoHexadecimal || !valor.All(Uri.IsHexDigit))
        {
            throw new FormatException("O hash SHA-256 deve conter exatamente 64 dígitos hexadecimais.");
        }

        return new HashSha256(valor.ToLowerInvariant());
    }

    public static HashSha256 CriarCanonico(string valorCanonico)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(valorCanonico);

        if (valorCanonico.Length != PrefixoCanonico.Length + ComprimentoHexadecimal ||
            !valorCanonico.StartsWith(PrefixoCanonico, StringComparison.Ordinal) ||
            !EhHexadecimalCanonico(valorCanonico.AsSpan(PrefixoCanonico.Length)))
        {
            throw new FormatException(
                "O hash SHA-256 canônico deve usar 'sha256:' seguido por 64 dígitos hexadecimais minúsculos.");
        }

        return new HashSha256(valorCanonico[PrefixoCanonico.Length..]);
    }

    public static HashSha256 Calcular(ReadOnlySpan<byte> conteudo)
    {
        var hash = SHA256.HashData(conteudo);
        return new HashSha256(Convert.ToHexString(hash).ToLowerInvariant());
    }

    public static HashSha256 CalcularUtf8(string texto)
    {
        ArgumentNullException.ThrowIfNull(texto);
        return Calcular(Encoding.UTF8.GetBytes(texto));
    }

    public override string ToString()
    {
        return ValorCanonico;
    }

    private static bool EhHexadecimalCanonico(ReadOnlySpan<char> valor)
    {
        foreach (var caractere in valor)
        {
            if (caractere is not (>= '0' and <= '9' or >= 'a' and <= 'f'))
            {
                return false;
            }
        }

        return true;
    }
}
