using Unity.Entities;

public struct BufferUserEntity : IBufferElementData
{
    public int id;
    public Entity entity;
}