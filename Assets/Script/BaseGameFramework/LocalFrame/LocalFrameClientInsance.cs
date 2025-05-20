using System;

public class LocalFrameClientInsance : ILocalFrame
{
    LocalFrame _localFrame;
    float totalTime;
    float preFrameSeconds;
    public LocalFrameClientInsance(LocalFrame localFrame)
    {
        _localFrame = localFrame;
    }

    public void Dispose()
    {
    }

    public void Update()
    {
        totalTime += UnityEngine.Time.deltaTime;
        if(totalTime - preFrameSeconds < _localFrame.DeltaTime)
        {
            return;
        }

        preFrameSeconds += _localFrame.DeltaTime;

        if(!CheckNeedClientUpdate())
        {
            preFrameSeconds = totalTime;  // 只支持最快一个表现帧跑一个逻辑帧。
            return;
        }

        AddLocalFrame();
    }

    private void AddLocalFrame()
    {
        _localFrame.ReceivedServerFrame++;
    }

    private bool CheckNeedClientUpdate()
    {
        return _localFrame.GameFrame == _localFrame.ReceivedServerFrame;
    }
}