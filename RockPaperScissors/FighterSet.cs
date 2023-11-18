namespace RockPaperScissors;

public class FighterSet
{
    private int[] _fighters = new int[6];

    public int this[Fighter fighter]
    {
        get => _fighters[(int)fighter];
        set => _fighters[(int)fighter] = value;
    }

    public bool Contains(Fighter fighter) => this[fighter] > 0;

    public FighterSet Clone()
    {
        var clone = new FighterSet();
        Array.Copy(_fighters, clone._fighters, _fighters.Length);
        return clone;
    }

    public int Count => _fighters.Sum();
}