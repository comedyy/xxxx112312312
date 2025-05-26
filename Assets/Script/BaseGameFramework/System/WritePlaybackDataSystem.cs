using Game.Battle.CommonLib;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Profiling;

public partial class WritePlaybackDataSystem : SystemBase
{
    bool _forceSaveThisFrame = false;
    bool? _needSave = null;

    BattleRecordSaver _recordSaver;
    PlaybackWriter _playbackWriter;

    private void Init()
    {
        PlaybackController playbackController = BattleControllerMgr.Instance.GetController<PlaybackController>();
        BattleDataController battleDataController = BattleControllerMgr.Instance.GetController<BattleDataController>();
        if (playbackController == null
        || playbackController.Writer == null)
        {
            _needSave = false;
            return;
        }

        _needSave = true;
        _playbackWriter = playbackController.Writer;
        _playbackWriter.SaveStartMessage(battleDataController.battleStartMessage);
        _recordSaver = new BattleRecordSaver();
    }


    protected override void OnUpdate()
    {
        if (!_needSave.HasValue) // 还没有初始化
        {
            Init();
        }

        if (!_needSave.Value)   // 无需存档
        {
            return;    
        }

        var checksumMgr = BattleControllerMgr.Instance.GetController<CheckSumMgr>();
        var frameCount = SystemAPI.GetSingleton<ComFrameCount>().frameLogic;

        PlaybackMessageItem item = _recordSaver.GetPlayBackMessageItem(frameCount, checksumMgr, InputUserSystem._fetchInputList, _forceSaveThisFrame);
        if (item.playbackBit != 0)
        {
            item.frame = (ushort)frameCount;
            _playbackWriter.SaveFrame(item);
        }
        
        _playbackWriter.OnUpdate(true, _forceSaveThisFrame);
        _forceSaveThisFrame = false;
        
        if (SystemAPI.GetSingleton<ComGameState>().IsEnd)
        {
            _recordSaver.EndSaveRecord();
        }
    }

    protected override void OnDestroy()
    {
        if (_playbackWriter != null)
        {
            _playbackWriter.Destroy();
        }

        base.OnDestroy();
    }
}
