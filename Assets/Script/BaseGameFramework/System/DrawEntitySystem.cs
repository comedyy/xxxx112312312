using System.Collections.Generic;
using Game.BattleShare.ECS.SystemGroup;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public partial class DrawEntitySystem : SystemBase
{
    protected override void OnUpdate()
    {
        NativeList<float3> positions = new NativeList<float3>(1, Allocator.TempJob);
        Entities.ForEach((ref Entity entity, ref LTransformComponent trans) =>
        {
            // Perform drawing logic here
            // For example, you might want to set a component or call a method to render the entity
            positions.Add(trans.position);
        }).Run();

        List<float3> positionsList = new List<float3>(positions.Length);
        for (int i = 0; i < positions.Length; i++)
        {
            positionsList.Add(positions[i]);
        }
        UnityEngine.Object.FindObjectOfType<InstanceDrawer>().Update1(positionsList);
    }
}