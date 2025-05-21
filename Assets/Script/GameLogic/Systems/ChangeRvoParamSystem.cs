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
        Entities.ForEach((ref RvoComponent rvo, in LTransform transform) =>
        {
            var pos = transform.position;
            var dir = fpMath.normalize(- new fp3(pos.x, 0, pos.z)) * 3;
            MSPathSystem.SetAgentVelocityPrefCS(rvo.AgentType, rvo.AgentId, dir.x, dir.z);
        }).Run();
    }
}