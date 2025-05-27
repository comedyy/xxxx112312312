using System;
using System.Collections.Generic;

public class ClientBattleControl : ILocalFrame
{
    LocalFrame _localFrame;
    float totalTime;
    float preFrameSeconds;

    InputCache _inputCache;
    public ClientBattleControl(LocalFrame localFrame, InputCache inputCache)
    {
        _localFrame = localFrame;
        _inputCache = inputCache;
    }

    public void Dispose()
    {
    }

    public void Update()
    {
        var deltaTime = MathF.Min(UnityEngine.Time.deltaTime, ComFrameCount.DELTA_TIME);
        totalTime += deltaTime;
        if(totalTime - preFrameSeconds < ComFrameCount.DELTA_TIME) // 还未能upate
        {
            return;
        }

        preFrameSeconds += ComFrameCount.DELTA_TIME;

        AddLocalFrame();
    }

    private void AddLocalFrame()
    {
        _localFrame.ReceivedServerFrame++;

        var ok = _inputCache.FetchItem(out var item);
        if (ok)
        {
            _localFrame.syncFrameInputCache.AddLocalFrame(_localFrame.ReceivedServerFrame, item);
        }
    }
}