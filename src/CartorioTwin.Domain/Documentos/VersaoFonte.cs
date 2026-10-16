using System.Globalization;

namespace CartorioTwin.Domain.Documentos;

public readonly record struct VersaoFonte
{
    private VersaoFonte(long numero)
    {
        Numero = numero;
    }

    public long Numero { get; }

    public static VersaoFonte Criar(long numero)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(numero);
        return new VersaoFonte(numero);
    }

    public override string ToString()
    {
        return Numero.ToString(CultureInfo.InvariantCulture);
    }
}
