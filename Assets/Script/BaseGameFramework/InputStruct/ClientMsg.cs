
using LiteNetLib.Utils;

// TODO: 把协议生成自动化。
public struct BattleStartMessage : INetSerializable
{
    public uint seed;
    public string guid;

    public void Deserialize(NetDataReader reader)
    {
        seed = reader.GetUInt();
        guid = reader.GetString();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(seed);
        writer.Put(guid);
    }
}

