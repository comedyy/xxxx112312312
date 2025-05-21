using Game.Battle.CommonLib;
using Unity.Entities;

public partial class InitAddAgentsSystem : SystemBase
{
    protected override void OnStartRunning()
    {
        // Initialize the system here if needed
        for (int i = 0; i < 1000; i++)
        {
            var agentId = MSPathSystem.AddAgentCS(0, i, 1, 3, 5, fp._0_05, fp._0_05, 1, 10, 1, 0, 0, i);
            var entity = EntityManager.CreateEntity(typeof(RvoComponent), typeof(LTransform), typeof(VTransform), typeof(VLerpTransform));
            EntityManager.SetComponentData(entity, new RvoComponent
            {
                AgentId = agentId,
                AgentType = 0,
            });
        }
    }

    protected override void OnUpdate()
    {

    }
}