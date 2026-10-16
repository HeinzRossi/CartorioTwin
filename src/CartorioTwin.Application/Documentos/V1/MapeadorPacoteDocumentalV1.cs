using System.Globalization;
using CartorioTwin.Contracts.Documentos.V1;
using CartorioTwin.Domain.Documentos;
using CartorioTwin.Domain.Identidades;
using CartorioTwin.Domain.Shared;

namespace CartorioTwin.Application.Documentos.V1;

internal static class MapeadorPacoteDocumentalV1
{
    public static PacoteDocumentalReidratadoV1 Reidratar(PacoteDocumentalV1 contrato)
    {
        ArgumentNullException.ThrowIfNull(contrato);
        ExigirColecoes(contrato);

        var fontes = ReidratarFontes(contrato.Fontes);
        var paginas = ReidratarPaginas(contrato.Paginas, fontes);
        var atos = ReidratarAtos(contrato.Atos, fontes, paginas);
        var direitos = ReidratarDireitos(contrato.Direitos, atos, fontes, paginas);
        var fatos = ReidratarFatos(contrato.Fatos, contrato.Revisoes, atos, fontes, paginas);

        return new PacoteDocumentalReidratadoV1(
            Array.AsReadOnly(fontes.Values.ToArray()),
            Array.AsReadOnly(paginas.Values.ToArray()),
            Array.AsReadOnly(atos.Values.ToArray()),
            Array.AsReadOnly(direitos),
            Array.AsReadOnly(fatos));
    }

    public static PacoteDocumentalV1 ParaContrato(PacoteDocumentalReidratadoV1 pacote)
    {
        ArgumentNullException.ThrowIfNull(pacote);

        var revisoes = pacote.Fatos
            .Select(fato => fato.RevisaoAplicada)
            .OfType<RevisaoFatoDocumental<ValorFatoContratoV1>>()
            .DistinctBy(revisao => revisao.Id)
            .Select(MapearRevisao)
            .ToArray();

        return new PacoteDocumentalV1(
            PacoteDocumentalV1.VersaoSuportada,
            pacote.Fontes.Select(MapearFonte).ToArray(),
            pacote.Paginas.Select(MapearPagina).ToArray(),
            pacote.Atos.Select(MapearAto).ToArray(),
            pacote.Direitos.Select(MapearDireito).ToArray(),
            pacote.Fatos.Select(MapearFato).ToArray(),
            revisoes);
    }

    private static Dictionary<(Guid Id, long Versao), FonteDocumental> ReidratarFontes(
        IReadOnlyList<FonteDocumentalContratoV1> contratos)
    {
        var fontes = new Dictionary<(Guid, long), FonteDocumental>();
        foreach (var contrato in contratos)
        {
            ArgumentNullException.ThrowIfNull(contrato);
            var id = LerGuid(contrato.Id, "fontes[].id");
            var fonte = FonteDocumental.Criar(
                FonteDocumentalId.Criar(id),
                VersaoFonte.Criar(contrato.Versao),
                HashSha256.CriarCanonico(contrato.HashSha256),
                contrato.NomeSanitizado,
                contrato.MimeType,
                contrato.TamanhoBytes);

            if (!fontes.TryAdd((id, contrato.Versao), fonte))
            {
                throw Referencia("fontes", "Fonte e versão duplicadas.");
            }
        }

        if (fontes.Count == 0)
        {
            throw Referencia("fontes", "O pacote exige ao menos uma fonte.");
        }

        return fontes;
    }

    private static Dictionary<(Guid FonteId, long Versao, int Pagina), PaginaTextoReidratadaV1> ReidratarPaginas(
        IReadOnlyList<PaginaTextoContratoV1> contratos,
        IReadOnlyDictionary<(Guid Id, long Versao), FonteDocumental> fontes)
    {
        var paginas = new Dictionary<(Guid, long, int), PaginaTextoReidratadaV1>();
        foreach (var contrato in contratos)
        {
            ArgumentNullException.ThrowIfNull(contrato);
            var fonteId = LerGuid(contrato.FonteId, "paginas[].fonteId");
            if (!fontes.TryGetValue((fonteId, contrato.FonteVersao), out var fonte))
            {
                throw Referencia("paginas[].fonteId", "A página referencia fonte ou versão inexistente.");
            }

            ArgumentNullException.ThrowIfNull(contrato.Texto);
            var pagina = Pagina.Criar(contrato.Numero);
            var hash = HashSha256.CriarCanonico(contrato.TextoHashSha256);
            if (hash != HashSha256.CalcularUtf8(contrato.Texto))
            {
                throw Referencia("paginas[].textoHashSha256", "O hash não corresponde ao texto integral da página.");
            }

            if (!paginas.TryAdd(
                    (fonteId, contrato.FonteVersao, contrato.Numero),
                    new PaginaTextoReidratadaV1(fonte, pagina, contrato.Texto, hash)))
            {
                throw Referencia("paginas", "A mesma página da fonte foi repetida.");
            }
        }

        return paginas;
    }

    private static Dictionary<Guid, AtoRegistral> ReidratarAtos(
        IReadOnlyList<AtoRegistralContratoV1> contratos,
        IReadOnlyDictionary<(Guid Id, long Versao), FonteDocumental> fontes,
        IReadOnlyDictionary<(Guid FonteId, long Versao, int Pagina), PaginaTextoReidratadaV1> paginas)
    {
        if (contratos.Any(ato => ato is null))
        {
            throw Referencia("atos", "A coleção de atos não aceita valor nulo.");
        }

        var ids = contratos.Select(ato => LerGuid(ato.Id, "atos[].id")).ToArray();
        if (ids.Distinct().Count() != ids.Length)
        {
            throw Referencia("atos", "Identificador de ato duplicado.");
        }

        var conjuntoIds = ids.ToHashSet();
        var atos = new Dictionary<Guid, AtoRegistral>();
        for (var indice = 0; indice < contratos.Count; indice++)
        {
            var contrato = contratos[indice];
            ArgumentNullException.ThrowIfNull(contrato);
            var id = ids[indice];
            var fonteId = LerGuid(contrato.FonteId, "atos[].fonteId");
            if (!fontes.ContainsKey((fonteId, contrato.FonteVersao)))
            {
                throw Referencia("atos[].fonteId", "O ato referencia fonte ou versão inexistente.");
            }

            ArgumentNullException.ThrowIfNull(contrato.Paginas);
            if (contrato.Paginas.Any(numero => !paginas.ContainsKey((fonteId, contrato.FonteVersao, numero))))
            {
                throw Referencia("atos[].paginas", "O ato referencia página inexistente na fonte.");
            }

            ArgumentNullException.ThrowIfNull(contrato.Partes);
            ArgumentNullException.ThrowIfNull(contrato.Relacoes);
            var relacoes = contrato.Relacoes.Select(relacao =>
            {
                ArgumentNullException.ThrowIfNull(relacao);
                var alvo = LerGuid(relacao.AtoRelacionadoId, "atos[].relacoes[].atoRelacionadoId");
                if (!conjuntoIds.Contains(alvo))
                {
                    throw Referencia("atos[].relacoes[].atoRelacionadoId", "A relação aponta para ato inexistente.");
                }

                return RelacaoAtoRegistral.Criar(
                    AtoRegistralId.Criar(alvo),
                    MapearTipoRelacao(relacao.Tipo));
            }).ToArray();

            var ato = AtoRegistral.Criar(
                AtoRegistralId.Criar(id),
                MatriculaId.Criar(LerGuid(contrato.MatriculaId, "atos[].matriculaId")),
                FonteDocumentalId.Criar(fonteId),
                VersaoFonte.Criar(contrato.FonteVersao),
                ReidratarIdentificador(contrato.Identificador),
                OrdemAtoRegistral.Criar(contrato.OrdemNaFonte),
                ReidratarDatas(contrato.Datas),
                contrato.Paginas.Select(Pagina.Criar),
                contrato.Partes.Select(ReidratarParte),
                relacoes);
            atos.Add(id, ato);
        }

        return atos;
    }

    private static DireitoDocumentado[] ReidratarDireitos(
        IReadOnlyList<DireitoDocumentadoContratoV1> contratos,
        IReadOnlyDictionary<Guid, AtoRegistral> atos,
        IReadOnlyDictionary<(Guid Id, long Versao), FonteDocumental> fontes,
        IReadOnlyDictionary<(Guid FonteId, long Versao, int Pagina), PaginaTextoReidratadaV1> paginas)
    {
        var ids = new HashSet<Guid>();
        return contratos.Select(contrato =>
        {
            ArgumentNullException.ThrowIfNull(contrato);
            var id = LerGuid(contrato.Id, "direitos[].id");
            if (!ids.Add(id))
            {
                throw Referencia("direitos", "Identificador de direito duplicado.");
            }

            var atoId = LerGuid(contrato.AtoOrigemId, "direitos[].atoOrigemId");
            if (!atos.TryGetValue(atoId, out var ato))
            {
                throw Referencia("direitos[].atoOrigemId", "O direito referencia ato inexistente.");
            }

            ArgumentNullException.ThrowIfNull(contrato.Titulares);
            ArgumentNullException.ThrowIfNull(contrato.Evidencias);
            return DireitoDocumentado.Criar(
                DireitoDocumentadoId.Criar(id),
                ato,
                MapearTipoDireito(contrato.Tipo),
                ReidratarExtensao(contrato.Extensao),
                MapearConhecimentoEfeito(contrato.ConhecimentoEfeito),
                LerDataOpcional(contrato.InicioDocumentado, "direitos[].inicioDocumentado"),
                LerDataOpcional(contrato.FimDocumentado, "direitos[].fimDocumentado"),
                contrato.Titulares.Select(idTitular =>
                    PessoaId.Criar(LerGuid(idTitular, "direitos[].titulares[]"))),
                contrato.Evidencias.Select(evidencia => ReidratarEvidencia(evidencia, fontes, paginas)));
        }).ToArray();
    }

    private static FatoDocumentalRevisavel<ValorFatoContratoV1>[] ReidratarFatos(
        IReadOnlyList<FatoDocumentalContratoV1> contratos,
        IReadOnlyList<RevisaoFatoContratoV1> contratosRevisoes,
        IReadOnlyDictionary<Guid, AtoRegistral> atos,
        IReadOnlyDictionary<(Guid Id, long Versao), FonteDocumental> fontes,
        IReadOnlyDictionary<(Guid FonteId, long Versao, int Pagina), PaginaTextoReidratadaV1> paginas)
    {
        if (contratos.Any(fato => fato is null) || contratosRevisoes.Any(revisao => revisao is null))
        {
            throw Referencia("fatos", "Fatos e revisões não aceitam valor nulo.");
        }

        var revisoes = new Dictionary<Guid, RevisaoFatoContratoV1>();
        foreach (var revisao in contratosRevisoes)
        {
            var id = LerGuid(revisao.Id, "revisoes[].id");
            if (!revisoes.TryAdd(id, revisao))
            {
                throw Referencia("revisoes", "Identificador de revisão duplicado.");
            }
        }

        var idsContratos = contratos.Select(fato => LerGuid(fato.Id, "fatos[].id")).ToArray();
        if (idsContratos.Distinct().Count() != idsContratos.Length)
        {
            throw Referencia("fatos", "Identificador de fato duplicado.");
        }

        var pendentes = contratos.ToList();
        var fatos = new Dictionary<Guid, FatoDocumentalRevisavel<ValorFatoContratoV1>>();
        var revisoesUsadas = new HashSet<Guid>();

        while (pendentes.Count > 0)
        {
            var processados = 0;
            foreach (var contrato in pendentes.ToArray())
            {
                var anteriorId = contrato.FatoAnteriorId is null
                    ? (Guid?)null
                    : LerGuid(contrato.FatoAnteriorId, "fatos[].fatoAnteriorId");
                if (anteriorId is not null && !fatos.ContainsKey(anteriorId.Value))
                {
                    continue;
                }

                var fato = ReidratarFato(
                    contrato,
                    anteriorId is null ? null : fatos[anteriorId.Value],
                    revisoes,
                    revisoesUsadas,
                    atos,
                    fontes,
                    paginas);
                fatos.Add(fato.Id.Valor, fato);
                pendentes.Remove(contrato);
                processados++;
            }

            if (processados == 0)
            {
                throw Referencia("fatos[].fatoAnteriorId", "A cadeia possui predecessor ausente ou ciclo.");
            }
        }

        if (revisoesUsadas.Count != revisoes.Count)
        {
            throw Referencia("revisoes", "Existe revisão sem fato terminal correspondente.");
        }

        return contratos.Select(contrato => fatos[LerGuid(contrato.Id, "fatos[].id")]).ToArray();
    }

    private static FatoDocumentalRevisavel<ValorFatoContratoV1> ReidratarFato(
        FatoDocumentalContratoV1 contrato,
        FatoDocumentalRevisavel<ValorFatoContratoV1>? anterior,
        IReadOnlyDictionary<Guid, RevisaoFatoContratoV1> revisoes,
        ISet<Guid> revisoesUsadas,
        IReadOnlyDictionary<Guid, AtoRegistral> atos,
        IReadOnlyDictionary<(Guid Id, long Versao), FonteDocumental> fontes,
        IReadOnlyDictionary<(Guid FonteId, long Versao, int Pagina), PaginaTextoReidratadaV1> paginas)
    {
        var id = FatoDocumentalId.Criar(LerGuid(contrato.Id, "fatos[].id"));
        var atoId = LerGuid(contrato.AtoOrigemId, "fatos[].atoOrigemId");
        if (!atos.TryGetValue(atoId, out var ato))
        {
            throw Referencia("fatos[].atoOrigemId", "O fato referencia ato inexistente.");
        }

        ArgumentNullException.ThrowIfNull(contrato.Evidencias);
        var evidencias = contrato.Evidencias
            .Select(evidencia => ReidratarEvidencia(evidencia, fontes, paginas))
            .ToArray();
        ValidarValor(contrato.ValorSugerido, "fatos[].valorSugerido");

        FatoDocumentalRevisavel<ValorFatoContratoV1> fato;
        if (anterior is null)
        {
            if (contrato.Versao != 1 || contrato.FatoAnteriorId is not null)
            {
                throw Referencia("fatos[].versao", "O primeiro fato deve usar versão 1 e não possuir predecessor.");
            }

            fato = contrato.ValorSugerido is null && evidencias.Length == 0
                ? FatoDocumentalRevisavel<ValorFatoContratoV1>.CriarNaoLocalizado(id, ato, contrato.CaminhoCampo)
                : FatoDocumentalRevisavel<ValorFatoContratoV1>.CriarExtraido(
                    id,
                    ato,
                    contrato.CaminhoCampo,
                    contrato.ValorSugerido ?? throw new ArgumentException("Fato com evidência exige valor sugerido."),
                    evidencias);
        }
        else
        {
            if (contrato.Versao != checked(anterior.Versao.Numero + 1) ||
                ato.Id != anterior.AtoOrigemId ||
                contrato.CaminhoCampo != anterior.CaminhoCampo)
            {
                throw Referencia("fatos[].versao", "A sucessora deve incrementar uma versão e preservar ato e campo.");
            }

            fato = contrato.ValorSugerido is null && evidencias.Length == 0
                ? FatoDocumentalRevisavel<ValorFatoContratoV1>.CriarNovaVersaoNaoLocalizada(id, anterior)
                : FatoDocumentalRevisavel<ValorFatoContratoV1>.CriarNovaVersaoExtraida(
                    id,
                    anterior,
                    contrato.ValorSugerido ?? throw new ArgumentException("Fato com evidência exige valor sugerido."),
                    evidencias);
        }

        if (contrato.Estado == EstadoRevisaoFatoContratoV1.EmRevisao)
        {
            fato = fato.IniciarRevisao();
        }
        else if (contrato.Estado is EstadoRevisaoFatoContratoV1.Confirmado or
                 EstadoRevisaoFatoContratoV1.Contestado or
                 EstadoRevisaoFatoContratoV1.NaoSuportado)
        {
            if (contrato.RevisaoId is null)
            {
                throw Referencia("fatos[].revisaoId", "Estado terminal exige revisão vinculada.");
            }

            var revisaoId = LerGuid(contrato.RevisaoId, "fatos[].revisaoId");
            if (!revisoes.TryGetValue(revisaoId, out var contratoRevisao) || !revisoesUsadas.Add(revisaoId))
            {
                throw Referencia("fatos[].revisaoId", "A revisão não existe ou já foi usada.");
            }

            fato = fato.IniciarRevisao().AplicarRevisao(ReidratarRevisao(contratoRevisao));
        }
        else if (contrato.RevisaoId is not null)
        {
            throw Referencia("fatos[].revisaoId", "Estado não terminal não aceita revisão aplicada.");
        }

        if ((EstadoRevisaoFatoContratoV1)fato.Estado != contrato.Estado ||
            (SituacaoEvidenciaFatoContratoV1)fato.SituacaoEvidencia != contrato.SituacaoEvidencia)
        {
            throw Referencia("fatos[].estado", "Estado ou proveniência não correspondem à transição derivada.");
        }

        return fato;
    }

    private static RevisaoFatoDocumental<ValorFatoContratoV1> ReidratarRevisao(
        RevisaoFatoContratoV1 contrato)
    {
        var id = RevisaoFatoDocumentalId.Criar(LerGuid(contrato.Id, "revisoes[].id"));
        var fatoId = FatoDocumentalId.Criar(LerGuid(contrato.FatoId, "revisoes[].fatoId"));
        var versao = VersaoFatoDocumental.Criar(contrato.VersaoFato);
        var usuario = UsuarioId.Criar(LerGuid(contrato.UsuarioId, "revisoes[].usuarioId"));
        var autorizacao = AutorizacaoRevisaoId.Criar(LerGuid(contrato.AutorizacaoId, "revisoes[].autorizacaoId"));
        var instante = LerInstanteUtc(contrato.EmUtc, "revisoes[].emUtc");
        var decisao = MapearDecisao(contrato.Decisao);
        ValidarValor(contrato.ValorFinal, "revisoes[].valorFinal");

        return contrato.ValorFinal is null
            ? RevisaoFatoDocumental<ValorFatoContratoV1>.CriarSemValorFinal(
                id, fatoId, versao, decisao, contrato.Justificativa, usuario, autorizacao, instante)
            : RevisaoFatoDocumental<ValorFatoContratoV1>.CriarComValorFinal(
                id, fatoId, versao, decisao, contrato.ValorFinal, contrato.Justificativa, usuario, autorizacao, instante);
    }

    private static TrechoEvidencia ReidratarEvidencia(
        EvidenciaContratoV1 contrato,
        IReadOnlyDictionary<(Guid Id, long Versao), FonteDocumental> fontes,
        IReadOnlyDictionary<(Guid FonteId, long Versao, int Pagina), PaginaTextoReidratadaV1> paginas)
    {
        ArgumentNullException.ThrowIfNull(contrato);
        var fonteId = LerGuid(contrato.FonteId, "evidencias[].fonteId");
        if (!fontes.TryGetValue((fonteId, contrato.FonteVersao), out var fonte) ||
            !paginas.TryGetValue((fonteId, contrato.FonteVersao, contrato.Pagina), out var pagina))
        {
            throw Referencia("evidencias", "A evidência referencia fonte, versão ou página inexistente.");
        }

        var evidencia = TrechoEvidencia.Criar(
            fonte,
            pagina.Pagina,
            pagina.Texto,
            contrato.InicioUtf16,
            contrato.FimUtf16);
        if (evidencia.HashFonte.ValorCanonico != contrato.FonteHashSha256 ||
            evidencia.HashTextoPagina.ValorCanonico != contrato.TextoHashSha256 ||
            evidencia.Trecho != contrato.Trecho)
        {
            throw Referencia("evidencias", "Hashes, intervalo ou trecho não correspondem ao conteúdo da fonte.");
        }

        return evidencia;
    }

    private static void ValidarValor(ValorFatoContratoV1? valor, string caminho)
    {
        if (valor is null)
        {
            return;
        }

        var preenchidos = new object?[] { valor.Texto, valor.Inteiro, valor.Booleano, valor.Fracao, valor.Identidade }
            .Count(item => item is not null);
        if (preenchidos != 1)
        {
            throw Referencia(caminho, "O valor deve preencher exatamente a variante indicada.");
        }

        switch (valor.Tipo)
        {
            case TipoValorFatoContratoV1.Texto when valor.Texto is not null:
                break;
            case TipoValorFatoContratoV1.Inteiro when valor.Inteiro is not null:
                break;
            case TipoValorFatoContratoV1.Booleano when valor.Booleano is not null:
                break;
            case TipoValorFatoContratoV1.Fracao when valor.Fracao is not null:
                _ = Fracao.Criar(valor.Fracao.Numerador, valor.Fracao.Denominador);
                break;
            case TipoValorFatoContratoV1.Identidade when valor.Identidade is not null:
                _ = LerGuid(valor.Identidade, $"{caminho}.identidade");
                break;
            default:
                throw Referencia(caminho, "O conteúdo não corresponde ao discriminador do valor.");
        }
    }

    private static FonteDocumentalContratoV1 MapearFonte(FonteDocumental fonte) =>
        new(
            GuidCanonico(fonte.Id.Valor),
            fonte.Versao.Numero,
            fonte.Hash.ValorCanonico,
            fonte.NomeOriginalSanitizado,
            fonte.MimeTypeValidado,
            fonte.TamanhoBytes);

    private static PaginaTextoContratoV1 MapearPagina(PaginaTextoReidratadaV1 pagina) =>
        new(
            GuidCanonico(pagina.Fonte.Id.Valor),
            pagina.Fonte.Versao.Numero,
            pagina.Pagina.Numero,
            pagina.Texto,
            pagina.HashTexto.ValorCanonico);

    private static AtoRegistralContratoV1 MapearAto(AtoRegistral ato) =>
        new(
            GuidCanonico(ato.Id.Valor),
            GuidCanonico(ato.MatriculaId.Valor),
            GuidCanonico(ato.FonteId.Valor),
            ato.VersaoFonte.Numero,
            new IdentificadorAtoContratoV1(
                MapearTipoIdentificadorContrato(ato.Identificador.Tipo),
                ato.Identificador.Numero,
                ato.Identificador.TextoOriginal),
            ato.OrdemNaFonte.Numero,
            new DatasDocumentaisAtoContratoV1(
                FormatarData(ato.Datas.DataDoTitulo),
                FormatarData(ato.Datas.DataDoAto),
                FormatarData(ato.Datas.DataDaApresentacao)),
            ato.Paginas.Select(pagina => pagina.Numero).ToArray(),
            ato.Partes.Select(parte => new ParteAtoContratoV1(
                parte.RotuloDocumental,
                parte.PessoaId is null ? null : GuidCanonico(parte.PessoaId.Value.Valor))).ToArray(),
            ato.Relacoes.Select(relacao => new RelacaoAtoContratoV1(
                GuidCanonico(relacao.AtoRelacionadoId.Valor),
                MapearTipoRelacaoContrato(relacao.Tipo))).ToArray());

    private static DireitoDocumentadoContratoV1 MapearDireito(DireitoDocumentado direito) =>
        new(
            GuidCanonico(direito.Id.Valor),
            GuidCanonico(direito.AtoOrigemId.Valor),
            MapearTipoDireitoContrato(direito.Tipo),
            new ExtensaoDireitoContratoV1(
                MapearTipoExtensaoContrato(direito.Extensao.Tipo),
                direito.Extensao.FracaoDoTotal is null
                    ? null
                    : new FracaoContratoV1(
                        direito.Extensao.FracaoDoTotal.Value.Numerador,
                        direito.Extensao.FracaoDoTotal.Value.Denominador)),
            MapearConhecimentoEfeitoContrato(direito.ConhecimentoEfeito),
            FormatarData(direito.InicioDocumentado),
            FormatarData(direito.FimDocumentado),
            direito.Titulares.Select(id => GuidCanonico(id.Valor)).ToArray(),
            direito.Evidencias.Select(MapearEvidencia).ToArray());

    private static FatoDocumentalContratoV1 MapearFato(
        FatoDocumentalRevisavel<ValorFatoContratoV1> fato) =>
        new(
            GuidCanonico(fato.Id.Valor),
            fato.Versao.Numero,
            fato.FatoAnteriorId is null ? null : GuidCanonico(fato.FatoAnteriorId.Value.Valor),
            GuidCanonico(fato.AtoOrigemId.Valor),
            fato.CaminhoCampo,
            fato.ValorSugerido.PossuiValor ? fato.ValorSugerido.Valor : null,
            fato.Evidencias.Select(MapearEvidencia).ToArray(),
            (EstadoRevisaoFatoContratoV1)fato.Estado,
            (SituacaoEvidenciaFatoContratoV1)fato.SituacaoEvidencia,
            fato.RevisaoAplicada is null ? null : GuidCanonico(fato.RevisaoAplicada.Id.Valor));

    private static RevisaoFatoContratoV1 MapearRevisao(
        RevisaoFatoDocumental<ValorFatoContratoV1> revisao) =>
        new(
            GuidCanonico(revisao.Id.Valor),
            GuidCanonico(revisao.FatoId.Valor),
            revisao.VersaoFato.Numero,
            (DecisaoRevisaoFatoContratoV1)revisao.Decisao,
            revisao.ValorFinal.PossuiValor ? revisao.ValorFinal.Valor : null,
            revisao.Justificativa,
            GuidCanonico(revisao.UsuarioId.Valor),
            GuidCanonico(revisao.AutorizacaoId.Valor),
            revisao.EmUtc.ToString("O", CultureInfo.InvariantCulture));

    private static EvidenciaContratoV1 MapearEvidencia(TrechoEvidencia evidencia) =>
        new(
            GuidCanonico(evidencia.FonteId.Valor),
            evidencia.VersaoFonte.Numero,
            evidencia.HashFonte.ValorCanonico,
            evidencia.Pagina.Numero,
            evidencia.HashTextoPagina.ValorCanonico,
            evidencia.InicioUtf16,
            evidencia.FimUtf16,
            evidencia.Trecho);

    private static IdentificadorAtoRegistral ReidratarIdentificador(IdentificadorAtoContratoV1 contrato)
    {
        ArgumentNullException.ThrowIfNull(contrato);
        return contrato.Tipo switch
        {
            TipoIdentificadorAtoContratoV1.Registro =>
                IdentificadorAtoRegistral.CriarRegistro(contrato.Numero ?? 0, contrato.TextoOriginal),
            TipoIdentificadorAtoContratoV1.Averbacao =>
                IdentificadorAtoRegistral.CriarAverbacao(contrato.Numero ?? 0, contrato.TextoOriginal),
            TipoIdentificadorAtoContratoV1.NaoIdentificado when contrato.Numero is null =>
                IdentificadorAtoRegistral.CriarNaoIdentificado(contrato.TextoOriginal),
            _ => throw Referencia("atos[].identificador", "Identificador de ato inconsistente."),
        };
    }

    private static DatasDocumentaisAto ReidratarDatas(DatasDocumentaisAtoContratoV1 contrato)
    {
        ArgumentNullException.ThrowIfNull(contrato);
        return new DatasDocumentaisAto(
            LerDataOpcional(contrato.DataDoTitulo, "atos[].datas.dataDoTitulo"),
            LerDataOpcional(contrato.DataDoAto, "atos[].datas.dataDoAto"),
            LerDataOpcional(contrato.DataDaApresentacao, "atos[].datas.dataDaApresentacao"));
    }

    private static ParteAtoRegistral ReidratarParte(ParteAtoContratoV1 contrato)
    {
        ArgumentNullException.ThrowIfNull(contrato);
        return contrato.PessoaId is null
            ? ParteAtoRegistral.CriarNaoIdentificada(contrato.RotuloDocumental)
            : ParteAtoRegistral.CriarIdentificada(
                PessoaId.Criar(LerGuid(contrato.PessoaId, "atos[].partes[].pessoaId")),
                contrato.RotuloDocumental);
    }

    private static ExtensaoDireitoDocumentado ReidratarExtensao(ExtensaoDireitoContratoV1 contrato)
    {
        ArgumentNullException.ThrowIfNull(contrato);
        return contrato.Tipo switch
        {
            TipoExtensaoDireitoContratoV1.NaoIdentificada when contrato.FracaoDoTotal is null =>
                ExtensaoDireitoDocumentado.NaoIdentificada,
            TipoExtensaoDireitoContratoV1.Total when contrato.FracaoDoTotal is null or { Numerador: 1, Denominador: 1 } =>
                ExtensaoDireitoDocumentado.Total,
            TipoExtensaoDireitoContratoV1.FracaoDoTotal when contrato.FracaoDoTotal is not null =>
                ExtensaoDireitoDocumentado.CriarFracaoDoTotal(
                    Fracao.Criar(contrato.FracaoDoTotal.Numerador, contrato.FracaoDoTotal.Denominador)),
            _ => throw Referencia("direitos[].extensao", "Extensão incompatível com sua fração."),
        };
    }

    private static TipoRelacaoAto MapearTipoRelacao(TipoRelacaoAtoContratoV1 tipo) => tipo switch
    {
        TipoRelacaoAtoContratoV1.Referencia => TipoRelacaoAto.Referencia,
        TipoRelacaoAtoContratoV1.Retificacao => TipoRelacaoAto.Retificacao,
        TipoRelacaoAtoContratoV1.Cancelamento => TipoRelacaoAto.Cancelamento,
        TipoRelacaoAtoContratoV1.Correspondencia => TipoRelacaoAto.Correspondencia,
        TipoRelacaoAtoContratoV1.Desconhecida => TipoRelacaoAto.Desconhecida,
        _ => throw Referencia("atos[].relacoes[].tipo", "Tipo de relação inválido."),
    };

    private static TipoDireitoDocumentado MapearTipoDireito(TipoDireitoDocumentadoContratoV1 tipo) =>
        tipo switch
        {
            TipoDireitoDocumentadoContratoV1.PropriedadePlena => TipoDireitoDocumentado.PropriedadePlena,
            TipoDireitoDocumentadoContratoV1.NuaPropriedade => TipoDireitoDocumentado.NuaPropriedade,
            TipoDireitoDocumentadoContratoV1.Usufruto => TipoDireitoDocumentado.Usufruto,
            TipoDireitoDocumentadoContratoV1.Onus => TipoDireitoDocumentado.Onus,
            TipoDireitoDocumentadoContratoV1.Garantia => TipoDireitoDocumentado.Garantia,
            TipoDireitoDocumentadoContratoV1.NaoIdentificado => TipoDireitoDocumentado.NaoIdentificado,
            _ => throw Referencia("direitos[].tipo", "Tipo de direito inválido."),
        };

    private static ConhecimentoEfeitoDireito MapearConhecimentoEfeito(
        ConhecimentoEfeitoDireitoContratoV1 conhecimento) =>
        conhecimento switch
        {
            ConhecimentoEfeitoDireitoContratoV1.NaoAvaliado => ConhecimentoEfeitoDireito.NaoAvaliado,
            ConhecimentoEfeitoDireitoContratoV1.Conhecido => ConhecimentoEfeitoDireito.Conhecido,
            ConhecimentoEfeitoDireitoContratoV1.Desconhecido => ConhecimentoEfeitoDireito.Desconhecido,
            ConhecimentoEfeitoDireitoContratoV1.NaoSuportado => ConhecimentoEfeitoDireito.NaoSuportado,
            _ => throw Referencia("direitos[].conhecimentoEfeito", "Conhecimento do efeito inválido."),
        };

    private static TipoIdentificadorAtoContratoV1 MapearTipoIdentificadorContrato(TipoIdentificadorAto tipo) =>
        tipo switch
        {
            TipoIdentificadorAto.Registro => TipoIdentificadorAtoContratoV1.Registro,
            TipoIdentificadorAto.Averbacao => TipoIdentificadorAtoContratoV1.Averbacao,
            TipoIdentificadorAto.NaoIdentificado => TipoIdentificadorAtoContratoV1.NaoIdentificado,
            _ => throw new InvalidOperationException("Tipo de identificador inválido."),
        };

    private static TipoRelacaoAtoContratoV1 MapearTipoRelacaoContrato(TipoRelacaoAto tipo) => tipo switch
    {
        TipoRelacaoAto.Referencia => TipoRelacaoAtoContratoV1.Referencia,
        TipoRelacaoAto.Retificacao => TipoRelacaoAtoContratoV1.Retificacao,
        TipoRelacaoAto.Cancelamento => TipoRelacaoAtoContratoV1.Cancelamento,
        TipoRelacaoAto.Correspondencia => TipoRelacaoAtoContratoV1.Correspondencia,
        TipoRelacaoAto.Desconhecida => TipoRelacaoAtoContratoV1.Desconhecida,
        _ => throw new InvalidOperationException("Tipo de relação inválido."),
    };

    private static TipoDireitoDocumentadoContratoV1 MapearTipoDireitoContrato(TipoDireitoDocumentado tipo) =>
        tipo switch
        {
            TipoDireitoDocumentado.PropriedadePlena => TipoDireitoDocumentadoContratoV1.PropriedadePlena,
            TipoDireitoDocumentado.NuaPropriedade => TipoDireitoDocumentadoContratoV1.NuaPropriedade,
            TipoDireitoDocumentado.Usufruto => TipoDireitoDocumentadoContratoV1.Usufruto,
            TipoDireitoDocumentado.Onus => TipoDireitoDocumentadoContratoV1.Onus,
            TipoDireitoDocumentado.Garantia => TipoDireitoDocumentadoContratoV1.Garantia,
            TipoDireitoDocumentado.NaoIdentificado => TipoDireitoDocumentadoContratoV1.NaoIdentificado,
            _ => throw new InvalidOperationException("Tipo de direito inválido."),
        };

    private static TipoExtensaoDireitoContratoV1 MapearTipoExtensaoContrato(
        TipoExtensaoDireitoDocumentado tipo) => tipo switch
        {
            TipoExtensaoDireitoDocumentado.NaoIdentificada => TipoExtensaoDireitoContratoV1.NaoIdentificada,
            TipoExtensaoDireitoDocumentado.Total => TipoExtensaoDireitoContratoV1.Total,
            TipoExtensaoDireitoDocumentado.FracaoDoTotal => TipoExtensaoDireitoContratoV1.FracaoDoTotal,
            _ => throw new InvalidOperationException("Tipo de extensão inválido."),
        };

    private static ConhecimentoEfeitoDireitoContratoV1 MapearConhecimentoEfeitoContrato(
        ConhecimentoEfeitoDireito conhecimento) => conhecimento switch
        {
            ConhecimentoEfeitoDireito.NaoAvaliado => ConhecimentoEfeitoDireitoContratoV1.NaoAvaliado,
            ConhecimentoEfeitoDireito.Conhecido => ConhecimentoEfeitoDireitoContratoV1.Conhecido,
            ConhecimentoEfeitoDireito.Desconhecido => ConhecimentoEfeitoDireitoContratoV1.Desconhecido,
            ConhecimentoEfeitoDireito.NaoSuportado => ConhecimentoEfeitoDireitoContratoV1.NaoSuportado,
            _ => throw new InvalidOperationException("Conhecimento do efeito inválido."),
        };

    private static DecisaoRevisaoFato MapearDecisao(DecisaoRevisaoFatoContratoV1 decisao) =>
        (DecisaoRevisaoFato)decisao;

    private static Guid LerGuid(string valor, string caminho)
    {
        if (!Guid.TryParseExact(valor, "D", out var guid) || guid == Guid.Empty || valor != GuidCanonico(guid))
        {
            throw Referencia(caminho, "O UUID deve ser não vazio, canônico e minúsculo.");
        }

        return guid;
    }

    private static DateOnly? LerDataOpcional(string? valor, string caminho)
    {
        if (valor is null)
        {
            return null;
        }

        if (!DateOnly.TryParseExact(valor, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var data))
        {
            throw Referencia(caminho, "A data documental deve usar yyyy-MM-dd.");
        }

        return data;
    }

    private static DateTimeOffset LerInstanteUtc(string valor, string caminho)
    {
        if (!DateTimeOffset.TryParseExact(valor, "O", CultureInfo.InvariantCulture, DateTimeStyles.None, out var instante) ||
            instante.Offset != TimeSpan.Zero)
        {
            throw Referencia(caminho, "O instante técnico deve usar formato round-trip com offset UTC.");
        }

        return instante;
    }

    private static string? FormatarData(DateOnly? data) =>
        data?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    private static string GuidCanonico(Guid guid) => guid.ToString("D", CultureInfo.InvariantCulture);

    private static InvalidDataException Referencia(string caminho, string mensagem) =>
        new($"{caminho}: {mensagem}");

    private static void ExigirColecoes(PacoteDocumentalV1 contrato)
    {
        ArgumentNullException.ThrowIfNull(contrato.SchemaVersion);
        ArgumentNullException.ThrowIfNull(contrato.Fontes);
        ArgumentNullException.ThrowIfNull(contrato.Paginas);
        ArgumentNullException.ThrowIfNull(contrato.Atos);
        ArgumentNullException.ThrowIfNull(contrato.Direitos);
        ArgumentNullException.ThrowIfNull(contrato.Fatos);
        ArgumentNullException.ThrowIfNull(contrato.Revisoes);
    }
}
