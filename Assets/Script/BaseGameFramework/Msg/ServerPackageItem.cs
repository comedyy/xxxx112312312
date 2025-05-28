

using System.Collections.Generic;
using LiteNetLib.Utils;

public partial struct ServerPackageItem 
{
    public List<UserFrameInput> list;
    public void OnDeserialize(NetDataReader reader)
    {
        var count = reader.GetByte();
        if (count > 0)
        {
            list = ListPool<UserFrameInput>.Get();
            for (int i = 0; i < count; i++)
            {
                list.Add(reader.Get<UserFrameInput>());
            }
        }
    }
}