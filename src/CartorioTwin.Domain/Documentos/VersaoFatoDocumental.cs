using System.Globalization;

namespace CartorioTwin.Domain.Documentos;

public readonly record struct VersaoFatoDocumental
{
    private VersaoFatoDocumental(long numero)
    {
        Numero = numero;
    }

    public long Numero { get; }

    public static VersaoFatoDocumental Criar(long numero)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(numero);
        return new VersaoFatoDocumental(numero);
    }

    public VersaoFatoDocumental Incrementar()
    {
        if (Numero == long.MaxValue)
        {
            throw new OverflowException("A versão do fato excedeu a capacidade de inteiros de 64 bits.");
        }

        return Criar(Numero + 1);
    }

    public override string ToString()
    {
        return Numero.ToString(CultureInfo.InvariantCulture);
    }
}
