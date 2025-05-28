using System;
using System.Collections.Generic;

public class LocalFrame
{
    public static LocalFrame Instance { get; private set; }
    public int GameFrame;
    public int ReceivedServerFrame => _syncFrameCache.ReceivedServerFrame;
    ILocalFrame _localFrameInstance;

    public IFetchFrame syncFrameInputCache => _syncFrameCache;
    SyncFrameCache _syncFrameCache = new SyncFrameCache();

    public InputCache _inputCache;
    public bool CanInput => _inputCache != null && _inputCache.CanInput;

    public LocalFrame(BattleType battleType)
    {
        _localFrameInstance = BattleControlFactory.Create(battleType, _syncFrameCache, out _inputCache);
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

    internal void SetBattleEnd()
    {
        _localFrameInstance.SetBattleEnd();
    }
}