/// <summary>
/// RVO Obstacle Opt
/// </summary>
public class GameEventBattleRvoOpt
{
    public enum Opt
    {
        AddObstacle,
        RemoveObstacle,
        ProcessObstacle,
        RemoveAgent,
        SetAgentRadius,
        SetPosition
    }

    public Opt opt;
    public fp[] points;
    public int param;
    public int param1;

    public static GameEventBattleRvoOpt CreateEvent(Opt opt, int param, int param1, fp[] points)

    {
        var @event = new GameEventBattleRvoOpt();
        @event.opt = opt;
        @event.points = points;
        @event.param = param;
        @event.param1 = param1;
        return @event;
    }
}

public class GameEventBattleRvoOptSetPos
{
    public int type;
    public int index;
    public fp x;
    public fp z;

    public static GameEventBattleRvoOptSetPos CreateEvent(int type, int index, fp x, fp z)
    {
        var @event = new GameEventBattleRvoOptSetPos();
        @event.type = type;
        @event.index = index;
        @event.x = x;
        @event.z = z;
        return @event;
    }
}
