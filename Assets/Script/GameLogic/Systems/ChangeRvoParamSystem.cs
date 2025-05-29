using Deterministics.Math;
using Game.Battle.CommonLib;
using Unity.Entities;

public partial class ChangeRvoParamSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        // Initialize the system here if needed
    }

    protected override void OnUpdate()
    {
        var entity = SystemAPI.GetSingletonBuffer<BufferUserEntity>()[0].entity;
        var posHero = EntityManager.GetComponentData<LComPosition>(entity).Value;

        Entities.ForEach((ref RvoComponent rvo, in LComPosition position) =>
        {
            var pos = position.Value;
            var dir = fpMath.normalize(posHero - new fp3(pos.x, 0, pos.z)) * 3;
            MSPathSystem.SetAgentVelocityPrefCS(rvo.AgentType, rvo.AgentId, dir.x, dir.z);
        }).Run();
    }
}