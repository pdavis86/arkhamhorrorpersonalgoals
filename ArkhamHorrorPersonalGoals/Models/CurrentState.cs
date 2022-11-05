namespace ArkhamHorrorPersonalGoals.Models
{
    public class CurrentState
    {
        public string? VisitorId { get; set; }
        
        public string? VisitorName { get; set; }

        public string? DisplayGoal { get; set; }

        public IEnumerable<Goal>? AssignedGoals { get; set; }
    }
}
