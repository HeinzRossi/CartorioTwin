using System.Globalization;

namespace CartorioTwin.Domain.Documentos;

public readonly record struct Pagina
{
    private Pagina(int numero)
    {
        Numero = numero;
    }

    public int Numero { get; }

    public static Pagina Criar(int numero)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(numero);
        return new Pagina(numero);
    }

    public override string ToString()
    {
        return Numero.ToString(CultureInfo.InvariantCulture);
    }
}
