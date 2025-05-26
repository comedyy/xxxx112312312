
using System;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib.Utils;
using Unity.Entities;

[Serializable]
public struct MessageHash : INetSerializable
{
    public int hash;
    public bool appendDetail;
    public FrameHashItem[] allHashDetails;
    public int escaped;
    public int frame;

    public void Deserialize(NetDataReader reader)
    {
        hash = reader.GetInt();

        appendDetail = reader.GetBool();
        if(appendDetail)
        {
            escaped = reader.GetInt();
            frame = reader.GetInt();
            var hashCount = reader.GetByte();
            allHashDetails = new FrameHashItem[hashCount];
            for(int i = 0; i < hashCount; i++)
            {
                allHashDetails[i].Read(reader);
            }
        }
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(hash);
        writer.Put(appendDetail);

        if(appendDetail)
        {
            writer.Put(escaped);
            writer.Put(frame);
            writer.Put((byte)allHashDetails.Length);
            for(int i = 0; i < allHashDetails.Length; i++)
            {
                allHashDetails[i].Write(writer);
            }
        }
    }
}

