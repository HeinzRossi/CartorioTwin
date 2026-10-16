using CartorioTwin.Contracts.Documentos.V1;
using CartorioTwin.Domain.Documentos;

namespace CartorioTwin.Application.Documentos.V1;

public enum CodigoErroPacoteV1
{
    JsonInvalido,
    VersaoNaoSuportada,
    ReferenciaInvalida,
    InvarianteDominio,
}

public sealed record ErroPacoteV1(CodigoErroPacoteV1 Codigo, string Caminho, string Mensagem);

public sealed record ResultadoLeituraPacoteV1
{
    private ResultadoLeituraPacoteV1(
        PacoteDocumentalReidratadoV1? pacote,
        IReadOnlyList<ErroPacoteV1> erros)
    {
        Pacote = pacote;
        Erros = erros;
    }

    public bool Sucesso => Pacote is not null;

    public PacoteDocumentalReidratadoV1? Pacote { get; }

    public IReadOnlyList<ErroPacoteV1> Erros { get; }

    internal static ResultadoLeituraPacoteV1 ComSucesso(PacoteDocumentalReidratadoV1 pacote) =>
        new(pacote, Array.Empty<ErroPacoteV1>());

    internal static ResultadoLeituraPacoteV1 ComErro(ErroPacoteV1 erro) =>
        new(null, Array.AsReadOnly([erro]));
}

public sealed record PaginaTextoReidratadaV1(
    FonteDocumental Fonte,
    Pagina Pagina,
    string Texto,
    HashSha256 HashTexto);

public sealed record PacoteDocumentalReidratadoV1(
    IReadOnlyList<FonteDocumental> Fontes,
    IReadOnlyList<PaginaTextoReidratadaV1> Paginas,
    IReadOnlyList<AtoRegistral> Atos,
    IReadOnlyList<DireitoDocumentado> Direitos,
    IReadOnlyList<FatoDocumentalRevisavel<ValorFatoContratoV1>> Fatos);
