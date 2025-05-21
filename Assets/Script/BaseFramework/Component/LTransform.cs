using Deterministics.Math;
using Unity.Entities;

public struct LTransform : IComponentData
{
    public fpQuaternion quaternion;
    public fp3 position;
}