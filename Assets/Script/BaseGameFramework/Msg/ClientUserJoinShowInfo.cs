using LiteNetLib.Utils;

public struct ClientUserJoinShowInfo : INetSerializable
{
    public string Name;
    public int idType;

    public void Deserialize(NetDataReader reader)
    {
        Name = reader.GetString();
        idType = reader.GetInt();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Name);
        writer.Put(idType);
    }
}

public partial struct RoomUser
{
    public ClientUserJoinShowInfo showInfo;
    public string Name => showInfo.Name;

    public void OnDeserialize(NetDataReader reader)
    {
        showInfo = ClientBattleRoomMgr.ReadObj<ClientUserJoinShowInfo>(userInfo);
    }
}