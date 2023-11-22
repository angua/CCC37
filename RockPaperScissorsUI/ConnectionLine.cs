namespace RockPaperScissorsUI;

class ConnectionLine
{
    public string FighterType { get; set; }

    public double StartPositionX { get; set; }
    public double StartPositionY { get; set; }

    public double EndPositionX { get; set; }
    public double EndPositionY { get; set; }

    // to avoid xaml binding failures
    public double PositionX { get; set; } = 0;
    public double PositionY { get; set; } = 0;


}
