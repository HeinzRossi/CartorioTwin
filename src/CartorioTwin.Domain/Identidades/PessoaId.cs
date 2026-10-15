namespace CartorioTwin.Domain.Identidades;

public readonly record struct PessoaId
{
    private PessoaId(Guid valor)
    {
        Valor = valor;
    }

    public Guid Valor { get; }

    public static PessoaId Criar(Guid valor)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(valor, Guid.Empty);
        return new PessoaId(valor);
    }

    public override string ToString()
    {
        return Valor.ToString("D");
    }
}
