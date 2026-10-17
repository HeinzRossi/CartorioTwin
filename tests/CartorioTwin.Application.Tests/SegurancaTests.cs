using CartorioTwin.Application.Seguranca;
using CartorioTwin.Domain.Documentos;
using CartorioTwin.Domain.Identidades;

namespace CartorioTwin.Application.Tests;

public sealed class SegurancaTests
{
    private static readonly ServentiaId Serventia = ServentiaId.Criar(Guid.Parse("10000000-0000-0000-0000-000000000001"));
    private static readonly UsuarioId Usuario = UsuarioId.Criar(Guid.Parse("20000000-0000-0000-0000-000000000001"));

    [Fact]
    public void Matriz_NaoConcedeRevisaoDeFatoAoAdministrador()
    {
        var administrador = ContextoAcesso.Usuario("id", Serventia, Usuario, PerfilAcesso.Administrador);
        var conferente = ContextoAcesso.Usuario("id", Serventia, Usuario, PerfilAcesso.Conferente);

        Assert.False(administrador.Possui(CapacidadesAcesso.FatosRevisar));
        Assert.True(administrador.Possui(CapacidadesAcesso.UsuariosAdministrar));
        Assert.True(conferente.Possui(CapacidadesAcesso.FatosRevisar));
        Assert.False(conferente.Possui(CapacidadesAcesso.UsuariosAdministrar));
    }

    [Theory]
    [InlineData(PerfilAcesso.Operador, 2)]
    [InlineData(PerfilAcesso.Conferente, 6)]
    [InlineData(PerfilAcesso.ResponsavelJuridico, 8)]
    [InlineData(PerfilAcesso.Administrador, 2)]
    [InlineData(PerfilAcesso.Auditor, 2)]
    public void Matriz_TemCapacidadesDeterministicas(PerfilAcesso perfil, int quantidade)
    {
        Assert.Equal(quantidade, MatrizCapacidadesAcesso.Obter(perfil).Count);
    }

    [Fact]
    public async Task Resolvedor_UsaVinculoAtivoDoRepositorio()
    {
        var repositorio = new RepositorioVinculoFake(new(
            Usuario,
            Serventia,
            PerfilAcesso.Conferente,
            UsuarioAtivo: true,
            ServentiaAtiva: true,
            VinculoAtivo: true));

        var resultado = await new ResolvedorAcesso(repositorio)
            .ResolverUsuarioAsync("https://idp.test", "subject-1", Serventia);

        Assert.True(resultado.Sucesso);
        Assert.Equal(PerfilAcesso.Conferente, resultado.Contexto!.Perfil);
        Assert.True(resultado.Contexto.Possui(CapacidadesAcesso.FatosRevisar));
    }

    [Theory]
    [InlineData(false, true, true)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    public async Task Resolvedor_RejeitaQualquerComponenteInativo(
        bool usuarioAtivo,
        bool serventiaAtiva,
        bool vinculoAtivo)
    {
        var repositorio = new RepositorioVinculoFake(new(
            Usuario,
            Serventia,
            PerfilAcesso.Conferente,
            usuarioAtivo,
            serventiaAtiva,
            vinculoAtivo));

        var resultado = await new ResolvedorAcesso(repositorio)
            .ResolverUsuarioAsync("https://idp.test", "subject-1", Serventia);

        Assert.False(resultado.Sucesso);
        Assert.Equal(FalhaResolucaoAcesso.VinculoInativo, resultado.Falha);
    }

    [Fact]
    public void Worker_PossuiSomenteCapacidadeTecnica()
    {
        var worker = ContextoAcesso.Servico("worker", workerAutorizado: true);

        Assert.True(worker.Possui(CapacidadesAcesso.JobsProcessar));
        Assert.False(worker.Possui(CapacidadesAcesso.FatosRevisar));
        Assert.False(worker.Possui(CapacidadesAcesso.UsuariosAdministrar));
    }

    [Fact]
    public async Task AutorizacaoRevisao_RegistraDecisaoHumanaAutorizada()
    {
        var repositorio = new RepositorioAutorizacaoFake();
        var contexto = ContextoAcesso.Usuario("id", Serventia, Usuario, PerfilAcesso.Conferente);
        var id = AutorizacaoRevisaoId.Criar(Guid.Parse("30000000-0000-0000-0000-000000000001"));
        var operacao = Guid.Parse("40000000-0000-0000-0000-000000000001");
        var recurso = Guid.Parse("50000000-0000-0000-0000-000000000001");
        var correlacao = Guid.Parse("60000000-0000-0000-0000-000000000001");

        var registro = await new EmissorAutorizacaoRevisao(repositorio).EmitirAsync(
            contexto,
            id,
            operacao,
            "FatoDocumental",
            recurso,
            2,
            new DateTimeOffset(2026, 10, 16, 22, 0, 0, TimeSpan.Zero),
            correlacao);

        Assert.Same(registro, repositorio.Registro);
        Assert.Equal(CapacidadesAcesso.FatosRevisar, registro.Capacidade);
        Assert.Equal(PerfilAcesso.Conferente, registro.PerfilEfetivo);
    }

    [Fact]
    public async Task AutorizacaoRevisao_RejeitaWorkerEAdministrador()
    {
        var emissor = new EmissorAutorizacaoRevisao(new RepositorioAutorizacaoFake());
        var argumentos = new Func<ContextoAcesso, Task<RegistroAutorizacaoRevisao>>(contexto => emissor.EmitirAsync(
            contexto,
            AutorizacaoRevisaoId.Criar(Guid.NewGuid()),
            Guid.NewGuid(),
            "FatoDocumental",
            Guid.NewGuid(),
            1,
            DateTimeOffset.UtcNow,
            Guid.NewGuid()));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => argumentos(ContextoAcesso.Servico("worker", true)));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => argumentos(
            ContextoAcesso.Usuario("admin", Serventia, Usuario, PerfilAcesso.Administrador)));
    }

    [Fact]
    public async Task AutorizacaoRevisao_ExigeInstanteUtcEContextoValido()
    {
        var emissor = new EmissorAutorizacaoRevisao(new RepositorioAutorizacaoFake());
        var contexto = ContextoAcesso.Usuario("id", Serventia, Usuario, PerfilAcesso.Conferente);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => emissor.EmitirAsync(
            contexto,
            AutorizacaoRevisaoId.Criar(Guid.NewGuid()),
            Guid.NewGuid(),
            "FatoDocumental",
            Guid.NewGuid(),
            1,
            new DateTimeOffset(2026, 10, 16, 22, 0, 0, TimeSpan.FromHours(-3)),
            Guid.NewGuid()));
    }

    [Fact]
    public async Task Storage_NaoExecutaIoQuandoContextoDiverge()
    {
        var storage = new StorageFake();
        var servico = new StorageDocumentalAutorizado(storage);
        var referencia = ReferenciaStorage.Gerar(Serventia, Guid.NewGuid(), Guid.NewGuid(), "staging");
        var outroContexto = ContextoAcesso.Usuario(
            "id",
            ServentiaId.Criar(Guid.NewGuid()),
            Usuario,
            PerfilAcesso.Conferente);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            servico.AbrirLeituraAsync(outroContexto, referencia));
        Assert.Equal(0, storage.Chamadas);
    }

    [Fact]
    public async Task Storage_NaoExecutaIoSemCapacidade()
    {
        var storage = new StorageFake();
        var servico = new StorageDocumentalAutorizado(storage);
        var referencia = ReferenciaStorage.Gerar(Serventia, Guid.NewGuid(), Guid.NewGuid(), "staging");
        var administrador = ContextoAcesso.Usuario("id", Serventia, Usuario, PerfilAcesso.Administrador);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            servico.AbrirLeituraAsync(administrador, referencia));
        Assert.Equal(0, storage.Chamadas);
    }

    [Fact]
    public void Storage_GeraChaveCanonicaESemCaminhoArbitrario()
    {
        var operacao = Guid.Parse("40000000-0000-0000-0000-000000000001");
        var objeto = Guid.Parse("50000000-0000-0000-0000-000000000001");

        var referencia = ReferenciaStorage.Gerar(Serventia, operacao, objeto, "staging");

        Assert.Equal($"{Serventia.Valor:N}/{operacao:N}/staging/{objeto:N}", referencia.Chave);
        Assert.Throws<ArgumentException>(() => ReferenciaStorage.Gerar(Serventia, operacao, objeto, "../fora"));
    }

    private sealed class RepositorioVinculoFake(VinculoUsuarioAcesso? vinculo) : IRepositorioVinculosAcesso
    {
        public Task<VinculoUsuarioAcesso?> BuscarAsync(
            string emissor,
            string subject,
            ServentiaId serventiaId,
            CancellationToken cancellationToken) => Task.FromResult(vinculo);
    }

    private sealed class RepositorioAutorizacaoFake : IRepositorioAutorizacoesRevisao
    {
        public RegistroAutorizacaoRevisao? Registro { get; private set; }

        public Task AdicionarAsync(RegistroAutorizacaoRevisao registro, CancellationToken cancellationToken)
        {
            Registro = registro;
            return Task.CompletedTask;
        }
    }

    private sealed class StorageFake : IStorageObjetos
    {
        public int Chamadas { get; private set; }

        public Task<Stream> AbrirLeituraAsync(string chave, CancellationToken cancellationToken)
        {
            Chamadas++;
            return Task.FromResult<Stream>(new MemoryStream());
        }

        public Task GravarStagingAsync(string chave, Stream conteudo, CancellationToken cancellationToken)
        {
            Chamadas++;
            return Task.CompletedTask;
        }

        public Task PromoverAsync(string chaveStaging, string chaveFinal, CancellationToken cancellationToken)
        {
            Chamadas++;
            return Task.CompletedTask;
        }
    }
}
