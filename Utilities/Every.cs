
namespace DalamudSystem.Utilities;

public class Every
{
    private readonly List<KeyValuePair<int, Action>> Actions = [];
    private int CurrentTick = 0;

    internal void Advance()
    {
        CurrentTick++;
        if (CurrentTick > 65536)
        {
            CurrentTick = 0;
        }
        foreach (KeyValuePair<int, Action> Action in Actions)
        {
            if (CurrentTick % Action.Key == 0)
            {
                Action.Value();
            }
        }
    }

    public void Register(int Interval, Action Action)
    {
        Actions.Add(new KeyValuePair<int, Action>(Interval, Action));
    }

    internal void Clear()
    {
        Actions.Clear();
    }
}
