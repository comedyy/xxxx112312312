using System;
using System.Collections.Generic;

public class ContinueBattleControl : ILocalFrame
{
    LocalFrame _localFrame;
    float totalTime;
    float preFrameSeconds;
    public PlaybackReader _reader;
    InputCache _inputCache;
    bool _isInReplayMode = false;

    public ContinueBattleControl(LocalFrame localFrame, InputCache inputCache, PlaybackReader playbackReader)
    {
        _localFrame = localFrame;
        _reader = playbackReader;
        _inputCache = inputCache;
        _inputCache.CanInput = false;
        _isInReplayMode = true;
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

        AddLocalFrame();
    }

    private void UpdateReplayState()
    {
        if (!_isInReplayMode) return;

        if (_reader.IsUnSync || _reader.AllEnd)
        {
            _isInReplayMode = false;
            _inputCache.CanInput = true;
            return;
        }
    }

    private void AddLocalFrame()
    {
        _localFrame.ReceivedServerFrame++;

        if (!_isInReplayMode)
        {
            var ok = _inputCache.FetchItem(out var item);
            if (ok)
            {
                _localFrame.syncFrameInputCache.AddLocalFrame(_localFrame.ReceivedServerFrame, item);
            }
        }
        else
        {
            var list = ListPool<UserFrameInput>.Get();
            _reader.GetMessageItem(_localFrame.ReceivedServerFrame, list);
            foreach (var item in list)
            {
                _localFrame.syncFrameInputCache.AddLocalFrame(_localFrame.ReceivedServerFrame, item);
            }
            ListPool<UserFrameInput>.Release(list);
        }
        
    }
}