using LiteNetLib.Utils;

public partial struct UserFrameInputMsg : INetSerializable
{
    public UserFrameInput input;
    public void OnSerialize(NetDataWriter writer)
    {
        writer.Put(input);
    }
    public void OnDeserialize(NetDataReader reader)
    {
        input = reader.Get<UserFrameInput>();
    }
}