namespace Refactoring.LegacyService.Models
{
    public class Position
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public PositionStatus Status { get; set; }
    }

    public enum PositionStatus
    {
        none = 0
    }
}
