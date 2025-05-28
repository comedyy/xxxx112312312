using Deterministics.Math;
using Unity.Core;
using Unity.Entities;

public partial class LogicUpdateSystemGroup : BaseUnsortSystemGroup
{
    LocalFrame _localFrame;
    internal void Inject(LocalFrame localFrame)
    {
        _localFrame = localFrame;
    }

    protected override void OnUpdate()
    {
        ref var frameCount = ref SystemAPI.GetSingletonRW<ComFrameCount>().ValueRW;
        int needFrame = GetNeedCalFrame(_localFrame);
        for(int i = 0; i < needFrame; i++)
        {
            var gameStateCom = SystemAPI.GetSingleton<ComGameState>();
            if (gameStateCom.IsEnd)
            {
                _localFrame.SetBattleEnd();
                break;
            }

            frameCount.frameLogic++;
            frameCount.frameUnity = UnityEngine.Time.frameCount;

            _localFrame.GameFrame = frameCount.frameLogic;

            base.OnUpdate();  // Update
        }
    }

    private int GetNeedCalFrame(LocalFrame _localFrame)
    {
        var receivedServerFrame = _localFrame.ReceivedServerFrame;
        var frameCount = _localFrame.GameFrame;
        if(frameCount < receivedServerFrame)
        {
            var offSet = receivedServerFrame - frameCount;

            if(offSet < 5)
            {
                return fpMath.min(1, offSet);
            }
            else if(offSet < 50)
            {
                return fpMath.min(2, offSet);
            }
            else if(offSet < 100)
            {
                return fpMath.min(4, offSet);
            }
            else
            {
                return 20;
            }
        }

        return 0;
    }
}