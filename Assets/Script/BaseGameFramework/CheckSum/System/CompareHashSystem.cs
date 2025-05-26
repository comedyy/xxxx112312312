using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Deterministics.Math;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Battle.ECS
{
    // 对比hash信息的system
    public partial class CompareHashSystem : SystemBase
    {
        PlaybackReader _playbackReader;
        bool? _needReader = null;
        protected override void OnUpdate()
        {
            if (!_needReader.HasValue)
            {
                // if (LocalFrame.Instance is LocalFrameNetGame) _needReader = true;
                // else
                {
                    _playbackReader = BattleControllerMgr.Instance.GetController<PlaybackController>().Reader;
                    _needReader = _playbackReader != null;
                }
            }

            if(!_needReader.Value) return;


            var frameCount = SystemAPI.GetSingleton<ComFrameCount>().frameLogic;
            if (!CalculateHashSystem.NeedCalHash(frameCount, true, out var hashIndex))
            {
                return;
            }
            
            // SendToServer
            var checkSum = BattleControllerMgr.Instance.GetController<CheckSumMgr>();
            // if (LocalFrame.Instance is LocalFrameNetGame localFrameNetGame)
            // {
            //     localFrameNetGame.SendHashToServer(frameCount, checkSum, hashIndex);
            // }
            // else
            {
                // 通过reader对比数据
#if DEBUG_1
                _playbackReader.CheckHash(frame, checkSum.GetMessageHash());
#else
                _playbackReader.CheckHash(frameCount, new MessageHash() { hash = checkSum.GetResultHash() });
#endif
            }
        }
    }
}
