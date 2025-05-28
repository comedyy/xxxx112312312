using System;
using System.Collections.Generic;

public class PlaybackBattleControl : ILocalFrame
{
    float totalTime;
    float preFrameSeconds;

    public PlaybackReader _reader;
    private readonly IPutMessage _putMessage;

    public int PlaybackScale { get; set; } = 1; // 播放速度
    public PlaybackBattleControl(PlaybackReader playbackReader, IPutMessage putMessage)
    {
        _reader = playbackReader;
        this._putMessage = putMessage;
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

        for (int i = 0; i < PlaybackScale; i++)
        {
            AddLocalFrame();
        }
    }

    private void AddLocalFrame()
    {
        var targetFrame = _putMessage.ReceivedServerFrame + 1;

        var list = ListPool<UserFrameInput>.Get();

        _reader.GetMessageItem(targetFrame, list);
        _putMessage.AddFrameWithList(targetFrame, list);
    }

    public void SetBattleEnd()
    {
    }
}