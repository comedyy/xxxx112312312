using Unity.Entities;

public partial class TimeoutFinishGameSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if(SystemAPI.TryGetSingleton<ComFrameCount>(out var frameCount))
        {
            if (frameCount.currentFrame > 100)
            {
                SystemAPI.GetSingletonRW<ComGameState>().ValueRW.IsEnd = true;
                UnityEngine.Debug.Log("Game Over");
            }
        }
    }
}