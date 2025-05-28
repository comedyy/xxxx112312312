using System;
using System.Collections.Generic;

public class ContinueBattleControl : ILocalFrame
{
    float totalTime;
    float preFrameSeconds;
    public PlaybackReader _reader;
    InputCache _inputCache;
    bool _isInReplayMode = false;
    public int PlaybackScale { get; private set; } = 1; // 播放速度
    public IPutMessage _putMessage;

    public ContinueBattleControl(InputCache inputCache, PlaybackReader playbackReader, IPutMessage putMessage)
    {
        _reader = playbackReader;
        _inputCache = inputCache;
        _inputCache.CanInput = false;
        _isInReplayMode = true;
        PlaybackScale = 10;
        _putMessage = putMessage;
    }

    public void Dispose()
    {
    }

    public void Update()
    {
        UpdateReplayState();

        var deltaTime = MathF.Min(UnityEngine.Time.deltaTime, ComFrameCount.DELTA_TIME);
        totalTime += deltaTime;
        if (totalTime - preFrameSeconds < ComFrameCount.DELTA_TIME) // 还未能upate
        {
            return;
        }

        preFrameSeconds += ComFrameCount.DELTA_TIME;

        for (int i = 0; i < PlaybackScale; i++)
        {
            AddLocalFrame();
        }
    }

    private void UpdateReplayState()
    {
        if (!_isInReplayMode) return;

        if (_reader.IsUnSync || _reader.AllEnd)
        {
            _isInReplayMode = false;
            _inputCache.CanInput = true;
            PlaybackScale = 1;
            return;
        }
    }

    private void AddLocalFrame()
    {
        var targetFrame = _putMessage.ReceivedServerFrame + 1;
        if (!_isInReplayMode)
        {
            var item = _inputCache.FetchItem();
            _putMessage.AddLocalFrame(targetFrame, item);
        }
        else
        {
            if (_reader.AllEnd || _reader.IsUnSync)
            {
                return;
            }

            var list = ListPool<UserFrameInput>.Get();
            _reader.GetMessageItem(targetFrame, list);
            _putMessage.AddFrameWithList(targetFrame, list);
        }
        
    }

    public void SetBattleEnd()
    {
    }
}