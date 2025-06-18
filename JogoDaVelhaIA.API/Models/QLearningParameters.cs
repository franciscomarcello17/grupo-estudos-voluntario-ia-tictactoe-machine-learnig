namespace JogoDaVelhIA.Models
{
    public class QLearningParameters
    {
        public double LearningRate { get; set; } = 0.1;
        public double DiscountFactor { get; set; } = 0.9;
        public double ExplorationRate { get; set; } = 0.3;
    }
}