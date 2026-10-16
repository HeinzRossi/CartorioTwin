using System.Text.Json;
using System.Text.Json.Serialization;
using CartorioTwin.Contracts.Documentos.V1;

namespace CartorioTwin.Application.Documentos.V1;

public static class SerializadorPacoteDocumentalV1
{
    private static readonly JsonSerializerOptions Opcoes = CriarOpcoes();

    public static string Serializar(PacoteDocumentalReidratadoV1 pacote)
    {
        var contrato = MapeadorPacoteDocumentalV1.ParaContrato(pacote);
        _ = MapeadorPacoteDocumentalV1.Reidratar(contrato);
        return JsonSerializer.Serialize(contrato, Opcoes);
    }

    public static ResultadoLeituraPacoteV1 Desserializar(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return Erro(CodigoErroPacoteV1.JsonInvalido, "$", "O JSON não pode ser vazio.");
        }

        PacoteDocumentalV1? contrato;
        try
        {
            contrato = JsonSerializer.Deserialize<PacoteDocumentalV1>(json, Opcoes);
        }
        catch (JsonException excecao)
        {
            return Erro(
                CodigoErroPacoteV1.JsonInvalido,
                excecao.Path ?? "$",
                "O JSON não corresponde ao contrato v1.");
        }

        if (contrato is null)
        {
            return Erro(CodigoErroPacoteV1.JsonInvalido, "$", "O pacote JSON não pode ser nulo.");
        }

        if (!string.Equals(
                contrato.SchemaVersion,
                PacoteDocumentalV1.VersaoSuportada,
                StringComparison.Ordinal))
        {
            return Erro(
                CodigoErroPacoteV1.VersaoNaoSuportada,
                "$.schemaVersion",
                $"Somente a versão {PacoteDocumentalV1.VersaoSuportada} é aceita.");
        }

        try
        {
            return ResultadoLeituraPacoteV1.ComSucesso(MapeadorPacoteDocumentalV1.Reidratar(contrato));
        }
        catch (InvalidDataException excecao)
        {
            var separador = excecao.Message.IndexOf(':', StringComparison.Ordinal);
            var caminho = separador > 0 ? excecao.Message[..separador] : "$";
            var mensagem = separador > 0 ? excecao.Message[(separador + 1)..].Trim() : excecao.Message;
            return Erro(CodigoErroPacoteV1.ReferenciaInvalida, caminho, mensagem);
        }
        catch (Exception excecao) when (excecao is ArgumentException or InvalidOperationException or OverflowException)
        {
            return Erro(CodigoErroPacoteV1.InvarianteDominio, "$", excecao.Message);
        }
    }

    private static ResultadoLeituraPacoteV1 Erro(
        CodigoErroPacoteV1 codigo,
        string caminho,
        string mensagem) =>
        ResultadoLeituraPacoteV1.ComErro(new ErroPacoteV1(codigo, caminho, mensagem));

    private static JsonSerializerOptions CriarOpcoes()
    {
        var opcoes = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = false,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
            WriteIndented = true,
        };
        opcoes.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower, allowIntegerValues: false));
        return opcoes;
    }
}
