using LiteNetLib.Utils;

public struct JoinMessage : INetSerializable
{
    public int UserId;
    public int idType;
    public int[] skills;

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(UserId);
        writer.Put(idType);
        writer.Put((short)skills.Length);
        for (int i = 0; i < skills.Length; i++)
        {
            writer.Put(skills[i]);
        }
    }

    public void Deserialize(NetDataReader reader)
    {
        UserId = reader.GetInt();
        idType = reader.GetInt();
        int skillCount = reader.GetShort();
        skills = new int[skillCount];
        for (int i = 0; i < skillCount; i++)
        {
            skills[i] = reader.GetInt();
        }
    }
}