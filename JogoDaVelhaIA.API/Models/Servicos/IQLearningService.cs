namespace JogoDaVelhIA.API.Models.Servicos
{
    public interface IQLearningService
    {
        void InicializarParametros(double taxaAprendizado = 0.1, double fatorDesconto = 0.9, double epsilon = 0.3);
        int ObterMelhorJogada(string estadoAtual, char simbolo);
        void AtualizarQValue(string estadoAnterior, int acao, string estadoAtual, double recompensa);
        Task TreinarIAAsync(int numeroEpisodios);
        string NormalizarEstado(string estado);
    }
}