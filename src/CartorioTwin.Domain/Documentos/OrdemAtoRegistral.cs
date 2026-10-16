using System.Globalization;

namespace CartorioTwin.Domain.Documentos;

public readonly record struct OrdemAtoRegistral
{
    private OrdemAtoRegistral(long numero)
    {
        Numero = numero;
    }

    public long Numero { get; }

    public static OrdemAtoRegistral Criar(long numero)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(numero);
        return new OrdemAtoRegistral(numero);
    }

    public override string ToString()
    {
        return Numero.ToString(CultureInfo.InvariantCulture);
    }
}
