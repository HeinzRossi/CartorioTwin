namespace CartorioTwin.Domain.Documentos;

public readonly record struct ValorOpcional<TValor>
    where TValor : notnull
{
    private readonly TValor? _valor;

    private ValorOpcional(TValor valor)
    {
        _valor = valor;
        PossuiValor = true;
    }

    public static ValorOpcional<TValor> Ausente => default;

    public bool PossuiValor { get; }

    public TValor Valor => PossuiValor
        ? _valor!
        : throw new InvalidOperationException("O valor opcional está ausente.");

    public static ValorOpcional<TValor> De(TValor valor)
    {
        ArgumentNullException.ThrowIfNull(valor);
        return new ValorOpcional<TValor>(valor);
    }
}
