namespace GameOfLife.API.Configuration
{
    public class GameOfLifeConfiguration
    {
        public int MaxAutoEvolution { get; set; }
        
        public EvolutionRulesConfiguration EvolutionRules { get; set; } = new();
    }
}
