namespace CartorioTwin.Domain.Identidades;

public readonly record struct MatriculaId
{
    private MatriculaId(Guid valor)
    {
        Valor = valor;
    }

    public Guid Valor { get; }

    public static MatriculaId Criar(Guid valor)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(valor, Guid.Empty);
        return new MatriculaId(valor);
    }

    public override string ToString()
    {
        return Valor.ToString("D");
    }
}
