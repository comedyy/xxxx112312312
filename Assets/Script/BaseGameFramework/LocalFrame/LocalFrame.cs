using System;

public class LocalFrame : ILocalFrame
{
    public static LocalFrame Instance { get; private set; }
    public bool BattleEnd;
    public fp ElapsedTime;
    public fp DeltaTime;
    public int GameFrame;
    public float changeFramePresentaionTime;
    public int ReceivedServerFrame;
    ILocalFrame _localFrameInstance;

    public LocalFrame(fp tick, int id, BattleType battleType)
    {
        DeltaTime = tick;

        if (battleType == BattleType.Client)
        {
            _localFrameInstance = new LocalFrameClientInsance(this);
        }
        
        Instance = this;
    }

    public void Update()
    {
        _localFrameInstance.Update();
    }

    public void Dispose()
    {
        _localFrameInstance.Dispose();
    }
}