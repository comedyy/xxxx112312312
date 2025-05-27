using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;

public class BattleRecordSaver
{
    LocalFrame _localFrame;
    bool _end = false;
    int _stage = -1;
    
    public BattleRecordSaver()
    {
    }

    public PlaybackMessageItem GetPlayBackMessageItem(int frame, CheckSumMgr checkSumMgr, List<UserFrameInput> frameInput, bool forceSave)
    {
        var item = new PlaybackMessageItem();
        if (_end) return item;
        if (frame >= ushort.MaxValue - 100) return item; // 防止帧太多，导致接关失败。

#if DEBUG_1
        var hash = checkSumMgr.GetMessageHash();
        return new PlaybackMessageItem(){
            playbackBit = PlaybackBit.Package | PlaybackBit.ChangeState | PlaybackBit.Hash,
            frame = (ushort)frame,
            list = frameInput, 
            currentState = (byte)_localFrame._clientStageIndex, hash = hash
        };
#else
        if (frameInput.Count > 0)
        {
            item.playbackBit |= PlaybackBit.Package;
            item.list = frameInput;
        }

        // if (_localFrame._clientStageIndex != _stage)
        // {
        //     item.playbackBit |= PlaybackBit.ChangeState;
        //     item.currentState = (byte)_localFrame._clientStageIndex;
        //     _stage = _localFrame._clientStageIndex;
        // }

        if (CalculateHashSystem.NeedCalHash(frame, false, out var _))
        {
            item.playbackBit |= PlaybackBit.Hash;
            item.hash = new MessageHash() { hash = checkSumMgr.GetResultHash() };
            UnityEngine.Debug.Log($"[BattleRecordSaver] frame:{frame} hash:{item.hash.hash}");
        }
        
        if(forceSave)
        {
            forceSave = false;
            item.playbackBit |= PlaybackBit.ForceSavePoint;
        }
#endif

        return item;
    }

    public void EndSaveRecord()
    {
        if(_end) return;

        OnBattleEndProcessPlayback(false, "", false);
        _end = true;
    }

    void OnBattleEndProcessPlayback(bool needUpload, string url, bool needUpLoadUnsync)
    {
        // await System.Threading.Tasks.Task.Delay(1); // 下一帧执行，因为当前堆栈还在ecs的system中，最后一帧还未存档。
//         if(World.DefaultGameObjectInjectionWorld == null) return;

        var playbackController = BattleControllerMgr.Instance.GetController<PlaybackController>();
        var zipPath = playbackController.SaveAndCompress();
        PlaybackController.DeleteOnGoing();

//         if(needUpload)
//         {
//             // Updaload to server.
//             CoroutineManager.Instance().StartCoroutine(MainNet.Upload(url, zipPath));
//         }

// #if DEVELOPMENT_BUILD || UNITY_EDITOR
//         if(needUpLoadUnsync)
//         {
//             CoroutineManager.Instance().StartCoroutine(MainNet.Upload(MainDownLoadAndReplay.URL_TEST_SYNC, zipPath, "_unsync_id_"+_localFrame.ControllerId));
//         }
// #endif
    }

}