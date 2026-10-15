using System.Numerics;
using CartorioTwin.Domain.Shared;

namespace CartorioTwin.Domain.Tests;

public sealed class FracaoTests
{
    [Fact]
    public void Criar_NormalizaPorMaiorDivisorComum()
    {
        Assert.Equal(Fracao.Criar(1, 2), Fracao.Criar(2, 4));
    }

    [Fact]
    public void Criar_NormalizaZeroEUnidade()
    {
        Assert.Equal(Fracao.Zero, Fracao.Criar(0, 37));
        Assert.Equal(Fracao.Zero, default);
        Assert.Equal(Fracao.Um, Fracao.Criar(37, 37));
        Assert.Equal("0/1", Fracao.Zero.ToString());
        Assert.Equal("1/1", Fracao.Um.ToString());
    }

    [Fact]
    public void Somar_TercosPermaneceExato()
    {
        var resultado = Fracao.Criar(1, 3) + Fracao.Criar(1, 3);

        Assert.Equal(Fracao.Criar(2, 3), resultado);
        Assert.Equal("2/3", resultado.ToString());
    }

    [Fact]
    public void Operacoes_AplicamAritmeticaExata()
    {
        var metade = Fracao.Criar(1, 2);
        var quarto = Fracao.Criar(1, 4);

        Assert.Equal(Fracao.Criar(3, 4), metade + quarto);
        Assert.Equal(quarto, metade - quarto);
        Assert.Equal(Fracao.Criar(1, 8), metade * quarto);
        Assert.Equal(Fracao.Criar(2, 1), metade / quarto);
    }

    [Fact]
    public void Comparar_NaoSofreOverflowIntermediario()
    {
        var menor = Fracao.Criar(long.MaxValue, long.MaxValue - 1);
        var maior = Fracao.Criar(long.MaxValue - 1, long.MaxValue - 2);
        var menorEquivalente = Fracao.Criar(long.MaxValue, long.MaxValue - 1);
        var maiorEquivalente = Fracao.Criar(long.MaxValue - 1, long.MaxValue - 2);

        Assert.True(menor < maior);
        Assert.True(maior > menor);
        Assert.True(menor <= menorEquivalente);
        Assert.True(maior >= maiorEquivalente);
    }

    [Fact]
    public void Criar_AceitaLimitesQueNormalizamDentroDeLong()
    {
        Assert.Equal(Fracao.Um, Fracao.Criar(long.MaxValue, long.MaxValue));
        Assert.Equal(Fracao.Um, Fracao.Criar(long.MinValue, long.MinValue));
    }

    [Fact]
    public void Criar_RejeitaDenominadorZeroEValorNegativo()
    {
        Assert.Throws<DivideByZeroException>(() => Fracao.Criar(1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => Fracao.Criar(-1, 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => Fracao.Criar(1, -2));
        Assert.Equal(Fracao.Criar(1, 2), Fracao.Criar(-1, -2));
    }

    [Fact]
    public void Operacoes_RejeitamResultadoNegativoEDivisaoPorZero()
    {
        Assert.Throws<InvalidOperationException>(() => Fracao.Criar(1, 3) - Fracao.Criar(1, 2));
        Assert.Throws<DivideByZeroException>(() => Fracao.Um / Fracao.Zero);
    }

    [Fact]
    public void Operacoes_RejeitamOverflowSemWrapAround()
    {
        var maximo = Fracao.Criar(long.MaxValue, 1);

        Assert.Throws<OverflowException>(() => maximo + Fracao.Um);
        Assert.Throws<OverflowException>(() => maximo * maximo);
        Assert.Throws<OverflowException>(() => Fracao.Criar(long.MinValue, -1));
    }

    [Fact]
    public void Criar_MantemInvariantesEmAmostraDeterministica()
    {
        for (var numerador = 0L; numerador <= 20; numerador++)
        {
            for (var denominador = 1L; denominador <= 20; denominador++)
            {
                var fracao = Fracao.Criar(numerador, denominador);

                Assert.True(fracao.Numerador >= 0);
                Assert.True(fracao.Denominador > 0);
                Assert.Equal(BigInteger.One, BigInteger.GreatestCommonDivisor(fracao.Numerador, fracao.Denominador));
                Assert.Equal(
                    new BigInteger(numerador) * fracao.Denominador,
                    new BigInteger(fracao.Numerador) * denominador);
            }
        }
    }
}
