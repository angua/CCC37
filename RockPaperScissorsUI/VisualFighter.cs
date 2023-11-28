namespace RockPaperScissorsUI;

class VisualFighter
{
    public string FighterType { get; set; }

    public double PositionX { get; set; }
    public double PositionY { get; set; }

    public UnknownOrAvailable IsUnknownOrAvailable { get; set; } = UnknownOrAvailable.None;
}
