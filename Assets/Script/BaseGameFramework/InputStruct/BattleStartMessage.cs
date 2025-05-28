
using LiteNetLib.Utils;

// TODO: 把协议生成自动化。
public struct BattleStartMessage : INetSerializable
{
    public uint seed;
    public string guid;
    public int battleType;
    public JoinMessage[] joins;

    public void Deserialize(NetDataReader reader)
    {
        seed = reader.GetUInt();
        guid = reader.GetString();
        battleType = reader.GetInt();
        int userCount = reader.GetInt();
        joins = new JoinMessage[userCount];
        for (int i = 0; i < userCount; i++)
        {
            joins[i] = reader.Get<JoinMessage>();
        }
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(seed);
        writer.Put(guid);
        writer.Put(battleType);

        if (joins == null || joins.Length == 0)
        {
            writer.Put(0);
            return;
        }
        
        writer.Put(joins.Length);
        foreach (var userJoinMessage in joins)
        {
            writer.Put(userJoinMessage);
        }
    }
}

