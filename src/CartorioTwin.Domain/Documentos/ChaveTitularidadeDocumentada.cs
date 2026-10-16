using CartorioTwin.Domain.Identidades;

namespace CartorioTwin.Domain.Documentos;

public readonly record struct ChaveTitularidadeDocumentada
{
    private ChaveTitularidadeDocumentada(
        MatriculaId matriculaId,
        PessoaId pessoaId,
        TipoDireitoDocumentado tipoDireito)
    {
        MatriculaId = matriculaId;
        PessoaId = pessoaId;
        TipoDireito = tipoDireito;
    }

    public MatriculaId MatriculaId { get; }

    public PessoaId PessoaId { get; }

    public TipoDireitoDocumentado TipoDireito { get; }

    internal static ChaveTitularidadeDocumentada Criar(
        MatriculaId matriculaId,
        PessoaId pessoaId,
        TipoDireitoDocumentado tipoDireito)
    {
        return new ChaveTitularidadeDocumentada(matriculaId, pessoaId, tipoDireito);
    }
}
