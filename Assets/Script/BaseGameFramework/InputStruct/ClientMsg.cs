
using LiteNetLib.Utils;

public struct BattleStartMessage : INetSerializable
{
    public uint seed;

    public void Deserialize(NetDataReader reader)
    {
        seed = reader.GetUInt();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(seed);
    }
}

