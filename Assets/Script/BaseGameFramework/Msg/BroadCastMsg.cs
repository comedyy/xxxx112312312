
using LiteNetLib.Utils;

public struct RoomChatItem : INetSerializable
{
    public int id;
    public string context;
    public string name;
    public string head;

    public void Deserialize(NetDataReader reader)
    {
        id = reader.GetInt();
        context = reader.GetString();
        name = reader.GetString();
        head = reader.GetString();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(id);
        writer.Put(context);
        writer.Put(name);
        writer.Put(head);
    }
}

public partial struct BroadCastMsg
{
    public RoomChatItem roomChatItem;
    public int id => roomChatItem.id;
    public string context => roomChatItem.context;
    public string name => roomChatItem.name;
    public string head => roomChatItem.head;

    public void OnDeserialize(NetDataReader reader)
    {
        if(broadCoastType == BroadCoastType.Chat)
        {
            roomChatItem = ClientBattleRoomMgr.ReadObj<RoomChatItem>(data);
        }
    }
}