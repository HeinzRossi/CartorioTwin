using System.Security.Cryptography;
using System.Text;
using System.IO;
using CartorioTwin.Desktop.Client.Autenticacao;

namespace CartorioTwin.Desktop.Wpf.Autenticacao;

public sealed class ArmazenamentoRefreshTokenDpapi : IArmazenamentoRefreshToken
{
    private static readonly byte[] Entropia = Encoding.UTF8.GetBytes("CartorioTwin.RefreshToken.v1");
    private readonly string caminho;

    public ArmazenamentoRefreshTokenDpapi(string diretorio)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(diretorio);
        caminho = Path.Combine(Path.GetFullPath(diretorio), "refresh-token.bin");
    }

    public static ArmazenamentoRefreshTokenDpapi CriarPadrao() => new(Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "CartorioTwin",
        "seguranca"));

    public async Task SalvarAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);
        var protegido = ProtectedData.Protect(
            Encoding.UTF8.GetBytes(refreshToken),
            Entropia,
            DataProtectionScope.CurrentUser);
        var diretorio = Path.GetDirectoryName(caminho)!;
        Directory.CreateDirectory(diretorio);
        var temporario = Path.Combine(diretorio, $".{Guid.NewGuid():N}.tmp");
        try
        {
            await File.WriteAllBytesAsync(temporario, protegido, cancellationToken);
            File.Move(temporario, caminho, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporario))
            {
                File.Delete(temporario);
            }
        }
    }

    public async Task<string?> LerAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(caminho))
        {
            return null;
        }

        var protegido = await File.ReadAllBytesAsync(caminho, cancellationToken);
        var aberto = ProtectedData.Unprotect(protegido, Entropia, DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(aberto);
    }

    public Task LimparAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (File.Exists(caminho))
        {
            File.Delete(caminho);
        }

        return Task.CompletedTask;
    }
}
