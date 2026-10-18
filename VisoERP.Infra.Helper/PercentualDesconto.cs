namespace VisoERP.Infra.Helper;

public static class PercentualDesconto
{
    public static decimal Exibir(decimal precoCatalogo, decimal precoComDesconto)
    {
        if (precoCatalogo == 0) return 0;

        var percentual = Math.Round((1 - precoComDesconto / precoCatalogo) * 100, 2);
        var inteiroMaisProximo = Math.Round(percentual, 0, MidpointRounding.AwayFromZero);
        return Math.Abs(percentual - inteiroMaisProximo) <= 0.01m
            ? inteiroMaisProximo
            : percentual;
    }
}
