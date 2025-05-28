
using LiteNetLib.Utils;

public struct ClientRoomShowInfo : INetSerializable
{
    public int roomType;

    public void Deserialize(NetDataReader reader)
    {
        roomType = reader.GetInt();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(roomType);
    }
}

public partial struct UpdateRoomMemberList
{
    public ClientRoomShowInfo ClientRoomShowInfo;
    public void OnDeserialize(NetDataReader reader)
    {
        if(roomShowInfo.Length == 0) return;
        ClientRoomShowInfo = ClientBattleRoomMgr.ReadObj<ClientRoomShowInfo>(roomShowInfo);
    }
}