namespace ArkhamHorrorPersonalGoals.Models
{
    public class Goal
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Objective { get; set; }

        public int VictoryPoints { get; set; }

        public string? Assignee { get; set; }

        public Goal(string title, string objective, int victoryPoints)
        {
            Title = title;
            Objective = objective;
            VictoryPoints = victoryPoints;
        }

        public override string ToString()
        {
            return $"{Title} - {Objective} ({VictoryPoints}VP)";
        }
    }
}
