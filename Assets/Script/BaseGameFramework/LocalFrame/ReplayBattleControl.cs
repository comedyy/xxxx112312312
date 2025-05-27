using System;
using System.Collections.Generic;

public class ReplayBattleControl : ILocalFrame
{
    LocalFrame _localFrame;
    float totalTime;
    float preFrameSeconds;


    public PlaybackReader _reader;
    public ReplayBattleControl(LocalFrame localFrame, PlaybackReader playbackReader)
    {
        _localFrame = localFrame;
        _reader = playbackReader;
    }

    public void Dispose()
    {
    }

    public void Update()
    {
        if(_reader.IsUnSync || _reader.AllEnd)
        {
            return;
        }

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

        var list = ListPool<UserFrameInput>.Get();

        _reader.GetMessageItem(_localFrame.ReceivedServerFrame, list);
        // var ok = _localFrame._inputCache.FetchItem(out var item);
        foreach (var item in list)
        {
            _localFrame.syncFrameInputCache.AddLocalFrame(_localFrame.ReceivedServerFrame, item);
        }

        ListPool<UserFrameInput>.Release(list);
    }
}