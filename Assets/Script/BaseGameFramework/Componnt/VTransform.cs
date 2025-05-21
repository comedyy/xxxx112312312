using Unity.Entities;
using Unity.Mathematics;

public struct VTransform : IComponentData
{
    public float3 position;
    public quaternion quaternion;
}