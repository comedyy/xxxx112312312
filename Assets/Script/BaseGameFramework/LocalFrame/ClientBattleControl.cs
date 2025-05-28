using System;
using System.Collections.Generic;

public class ClientBattleControl : ILocalFrame
{
    float totalTime;
    float preFrameSeconds;

    InputCache _inputCache;
    private readonly IPutMessage _putMessage;
    public ClientBattleControl(InputCache inputCache, IPutMessage putMessage)
    {
        _putMessage = putMessage;
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
        var targetFrame = _putMessage.ReceivedServerFrame + 1;

        var item = _inputCache.FetchItem();
        _putMessage.AddLocalFrame(targetFrame, item);
    }

    public void SetBattleEnd()
    {
    }
}