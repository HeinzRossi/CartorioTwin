using System.Numerics;

namespace CartorioTwin.Domain.Shared;

public readonly record struct Fracao : IComparable<Fracao>
{
    private readonly long _numerador;
    private readonly long _denominadorMenosUm;

    private Fracao(long numerador, long denominador)
    {
        _numerador = numerador;
        _denominadorMenosUm = denominador - 1;
    }

    public static Fracao Zero => default;

    public static Fracao Um { get; } = new(1, 1);

    public long Numerador => _numerador;

    public long Denominador => _denominadorMenosUm + 1;

    public static Fracao Criar(long numerador, long denominador)
    {
        return CriarNormalizada(numerador, denominador);
    }

    public static Fracao operator +(Fracao esquerda, Fracao direita)
    {
        var numerador =
            (new BigInteger(esquerda.Numerador) * direita.Denominador) +
            (new BigInteger(direita.Numerador) * esquerda.Denominador);
        var denominador = new BigInteger(esquerda.Denominador) * direita.Denominador;

        return CriarNormalizada(numerador, denominador);
    }

    public static Fracao operator -(Fracao esquerda, Fracao direita)
    {
        var numerador =
            (new BigInteger(esquerda.Numerador) * direita.Denominador) -
            (new BigInteger(direita.Numerador) * esquerda.Denominador);

        if (numerador.Sign < 0)
        {
            throw new InvalidOperationException("Uma fração do domínio não pode resultar em valor negativo.");
        }

        var denominador = new BigInteger(esquerda.Denominador) * direita.Denominador;
        return CriarNormalizada(numerador, denominador);
    }

    public static Fracao operator *(Fracao esquerda, Fracao direita)
    {
        var numerador = new BigInteger(esquerda.Numerador) * direita.Numerador;
        var denominador = new BigInteger(esquerda.Denominador) * direita.Denominador;

        return CriarNormalizada(numerador, denominador);
    }

    public static Fracao operator /(Fracao esquerda, Fracao direita)
    {
        if (direita == Zero)
        {
            throw new DivideByZeroException("Não é possível dividir uma fração por zero.");
        }

        var numerador = new BigInteger(esquerda.Numerador) * direita.Denominador;
        var denominador = new BigInteger(esquerda.Denominador) * direita.Numerador;

        return CriarNormalizada(numerador, denominador);
    }

    public static bool operator <(Fracao esquerda, Fracao direita)
    {
        return esquerda.CompareTo(direita) < 0;
    }

    public static bool operator <=(Fracao esquerda, Fracao direita)
    {
        return esquerda.CompareTo(direita) <= 0;
    }

    public static bool operator >(Fracao esquerda, Fracao direita)
    {
        return esquerda.CompareTo(direita) > 0;
    }

    public static bool operator >=(Fracao esquerda, Fracao direita)
    {
        return esquerda.CompareTo(direita) >= 0;
    }

    public int CompareTo(Fracao other)
    {
        var esquerda = new BigInteger(Numerador) * other.Denominador;
        var direita = new BigInteger(other.Numerador) * Denominador;

        return esquerda.CompareTo(direita);
    }

    public override string ToString()
    {
        return FormattableString.Invariant($"{Numerador}/{Denominador}");
    }

    private static Fracao CriarNormalizada(BigInteger numerador, BigInteger denominador)
    {
        if (denominador.IsZero)
        {
            throw new DivideByZeroException("O denominador da fração não pode ser zero.");
        }

        if (denominador.Sign < 0)
        {
            numerador = BigInteger.Negate(numerador);
            denominador = BigInteger.Negate(denominador);
        }

        if (numerador.Sign < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(numerador), "Uma fração do domínio não pode ser negativa.");
        }

        if (numerador.IsZero)
        {
            return Zero;
        }

        var divisorComum = BigInteger.GreatestCommonDivisor(numerador, denominador);
        numerador /= divisorComum;
        denominador /= divisorComum;

        if (numerador > long.MaxValue || denominador > long.MaxValue)
        {
            throw new OverflowException("A fração normalizada excede a capacidade de inteiros de 64 bits.");
        }

        return new Fracao((long)numerador, (long)denominador);
    }
}
