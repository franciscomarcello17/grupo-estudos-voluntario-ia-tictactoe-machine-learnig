namespace JogoDaVelhaIA.API.Models.ViewModels
{
    public class ResultadoTreinamentoViewModel
    {
        public int EpisodiosConcluidos { get; set; }
        public int VitoriasIA { get; set; }
        public int VitoriasHumano { get; set; }
        public int Empates { get; set; }
        public string TempoDecorrido { get; set; }
        public string Mensagem { get; set; }
    }
}