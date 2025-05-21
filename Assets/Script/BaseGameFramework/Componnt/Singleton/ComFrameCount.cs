

using Unity.Entities;

public struct ComFrameCount : IComponentData
{
    public int currentFrame;
    public int frameUnity;

    public static fp DELTA_TIME = fp._0_05;
}