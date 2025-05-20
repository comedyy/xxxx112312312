using Deterministics.Math;
using Unity.Core;

namespace Game.BattleShare.ECS.SystemGroup
{
    public partial class LogicUpdateSystemGroup : BaseUnsortSystemGroup
    {
        LocalFrame _localFrame;
        internal void Inject(LocalFrame localFrame)
        {
            _localFrame = localFrame;
        }
    
        protected override void OnUpdate()
        {
            int needFrame = GetNeedCalFrame(_localFrame);
            for(int i = 0; i < needFrame; i++)
            {
                if(_localFrame.BattleEnd) return; // 游戏已经结束
                
                _localFrame.ElapsedTime += _localFrame.DeltaTime;
                _localFrame.changeFramePresentaionTime = UnityEngine.Time.time;
                _localFrame.GameFrame ++;

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
}