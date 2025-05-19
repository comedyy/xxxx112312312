using Deterministics.Math;
using Unity.Core;

namespace Game.BattleShare.ECS.SystemGroup
{
    public partial class FixedTimeSystemGroup : BaseUnsortSystemGroup
    {
        // public static FixedTimeSystemGroup Instance;
        
        // BattleDataController BattleDataController => BattleCore.BattleControllerManager.GetBattleController<BattleDataController>();

        // LocalFrame _localFrame;

        // private bool _isLogicFrame = false;
        // public bool IsLogicFrame => _isLogicFrame;
        
        // protected override void OnCreate()
        // {
        //     Instance = this;
        //     base.OnCreate();
        // }

        // internal void InitLogicTime(LocalFrame localFrame)
        // {
        //     _localFrame = localFrame;
        // }
    
        // protected override void OnUpdate()
        // {
        //     int needFrame = GetNeedCalFrame(BattleDataController, _localFrame);
        //     // int needFrame = 1;
        //     if (needFrame <= 0)
        //     {
        //         _isLogicFrame = false;
        //         World.SetTime(new TimeData(UnityEngine.Time.time, UnityEngine.Time.deltaTime, BattleDataController.ElapsedTime, BattleDataController.DeltaTime));
        //     }
            
        //     for(int i = 0; i < needFrame; i++)
        //     {
        //         if(_localFrame.BattleEnd) return; // 游戏已经结束
                
        //         BattleDataController.ElapsedTime += BattleDataController.DeltaTime;
        //         BattleDataController.FrameCount ++;
        //         BattleDataController.changeFramePresentaionTime = UnityEngine.Time.time;
        //         LocalFrame.Instance.GameFrame = BattleDataController.FrameCount;

        //         World.SetTime(new TimeData(UnityEngine.Time.time, UnityEngine.Time.deltaTime, BattleDataController.ElapsedTime, BattleDataController.DeltaTime));
        //         base.OnUpdate();  // Update
        //         _isLogicFrame = true;
        //     }
            
        // }

        // private int GetNeedCalFrame(BattleDataController battleDataController, LocalFrame _localFrame)
        // {
        //     if(_localFrame is LocalFrameTestSync localFrameTestSync)
        //     {
        //         if(localFrameTestSync.IsCanLoading) return 0;
        //         return 1;//localFrameTestSync.Speed;
        //     }

        //     var receivedServerFrame = _localFrame.LastCanExecuteFrame;
        //     var frameCount = battleDataController.FrameCount;
        //     if(_localFrame is LocalFrameReloadClient && _localFrame.IsPlayback)
        //     {
        //         return receivedServerFrame - frameCount;
        //     }

        //     if(frameCount < receivedServerFrame)
        //     {
        //         var offSet = receivedServerFrame - frameCount;

        //         if(offSet < 5)
        //         {
        //             return fpMath.min(1, offSet);
        //         }
        //         else if(offSet < 50)
        //         {
        //             return fpMath.min(2, offSet);
        //         }
        //         else if(offSet < 100)
        //         {
        //             return fpMath.min(4, offSet);
        //         }
        //         else
        //         {
        //             return 20;
        //         }
        //     }

        //     return 0;
        // }
    }
}