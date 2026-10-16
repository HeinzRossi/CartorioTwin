namespace CartorioTwin.Domain.Documentos;

public readonly record struct RelacaoAtoRegistral
{
    private RelacaoAtoRegistral(AtoRegistralId atoRelacionadoId, TipoRelacaoAto tipo)
    {
        AtoRelacionadoId = atoRelacionadoId;
        Tipo = tipo;
    }

    public AtoRegistralId AtoRelacionadoId { get; }

    public TipoRelacaoAto Tipo { get; }

    public static RelacaoAtoRegistral Criar(AtoRegistralId atoRelacionadoId, TipoRelacaoAto tipo)
    {
        if (atoRelacionadoId == default)
        {
            throw new ArgumentOutOfRangeException(nameof(atoRelacionadoId), "O ato relacionado não pode ser vazio.");
        }

        if (!Enum.IsDefined(tipo))
        {
            throw new ArgumentOutOfRangeException(nameof(tipo), "O tipo da relação deve ser conhecido pelo contrato.");
        }

        return new RelacaoAtoRegistral(atoRelacionadoId, tipo);
    }
}
