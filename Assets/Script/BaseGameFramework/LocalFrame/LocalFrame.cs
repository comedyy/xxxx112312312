using System;
using System.Collections.Generic;

public class LocalFrame : ILocalFrame
{
    public static LocalFrame Instance { get; private set; }
    public int GameFrame;
    public int ReceivedServerFrame;
    ILocalFrame _localFrameInstance;

    public IFetchFrame syncFrameInputCache => _syncFrameCache;
    SyncFrameCache _syncFrameCache = new SyncFrameCache();

    public InputCache _inputCache;

    public LocalFrame(int id, BattleType battleType)
    {
        _localFrameInstance = BattleControlFactory.Create(battleType, this, out _inputCache);
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