
using System;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib.Utils;

[Serializable]
public struct FrameHashItem
{
    public byte hashType;
    public int hash;
    public int frame;

    public List<int> listValue;
    public List<short> lstParamIndex;
    public List<string> lstParam;
    public List<int> listEntity;

    public void Write(NetDataWriter writer)
    {
        writer.Put(hashType);
        writer.Put(hash);

        if (listValue == null)
        {
            writer.Put((short)-1);
            return;
        }

        writer.Put((short)listValue.Count);
        for (int i = 0; i < listValue.Count; i++)
        {
            writer.Put(listValue[i]);
        }

        writer.Put((short)listEntity.Count);
        for (int i = 0; i < listEntity.Count; i++)
        {
            writer.Put(listEntity[i]);
        }

        if(lstParamIndex == null)
        {
            writer.Put((short)-1);
            writer.Put((short)lstParam.Count);
            for (int i = 0; i < lstParam.Count; i++)
            {
                writer.Put(lstParam[i]);
            }
        }
        else
        {
            writer.Put((short)lstParamIndex.Count);
            for (int i = 0; i < lstParamIndex.Count; i++)
            {
                writer.Put(lstParamIndex[i]);
            }
        }
    }

    public void Read(NetDataReader reader)
    {
        hashType = reader.GetByte();
        hash = reader.GetInt();

        var listCount = reader.GetShort();
        if (listCount == -1) return;

        listValue = new List<int>();
        for (int i = 0; i < listCount; i++)
        {
            listValue.Add(reader.GetInt());
        }

        var listCount1 = reader.GetShort();
        listEntity = new List<int>();
        for (int i = 0; i < listCount1; i++)
        {
            listEntity.Add(reader.GetInt());
        }

        var listCount2 = reader.GetShort();
        if(listCount2 == -1)
        {
            listCount2 = reader.GetShort();
            lstParam = new List<string>();
            for (int i = 0; i < listCount2; i++)
            {
                lstParam.Add(reader.GetString());
            }
        }
        else
        {
            lstParamIndex = new List<short>();
            for (int i = 0; i < listCount2; i++)
            {
                lstParamIndex.Add(reader.GetShort());
            }
        }
    }
    public static bool operator ==(FrameHashItem item1, FrameHashItem item2)
    {
        if (item1.hash != item2.hash) return false;

        if (item1.listEntity != null && item2.listEntity != null)
        {
            if (item1.listEntity.Count != item2.listEntity.Count) return false;
            for (int i = 0; i < item1.listEntity.Count; i++)
            {
                if (item1.listEntity[i] != item1.listEntity[i]) return false;
            }
        }

        if (item1.listValue != null && item2.listValue != null)
        {
            if (item1.listValue.Count != item2.listValue.Count) return false;
            for (int i = 0; i < item1.listValue.Count; i++)
            {
                if (item1.listValue[i] != item1.listValue[i]) return false;
            }
        }

        return true;
    }

    public static bool operator !=(FrameHashItem item1, FrameHashItem item2)
    {
        return !(item1 == item2);
    }

    public string GetString(List<string> symbol)
    {
        if (listValue != null)
        {
            if(listEntity != null && lstParamIndex != null && lstParamIndex.Count > 0 && symbol != null)
            {
                return $"{(CheckSumType)hashType} Hash:{hash} \n listValue：{string.Join("!", listValue)} \n{string.Join("!", listEntity.Select(m=>(m>>16, m & 0xffff)))} \n{string.Join("\n", lstParamIndex.Select(m=>symbol[m]))}";
            }
            else if(listEntity != null && lstParam != null && lstParam.Count > 0)
            {
                return $"{(CheckSumType)hashType} Hash:{hash} \n listValue：{string.Join("!", listValue)} \n{string.Join("!", listEntity.Select(m=>(m>>16, m & 0xffff)))} \n{string.Join("\n", lstParam)}";
            }
            else if(listEntity != null)
            {
                return $"{(CheckSumType)hashType} Hash:{hash} \n listValue：{string.Join("!", listValue)} \n{string.Join("!", listEntity.Select(m=>(m>>16, m & 0xffff)))}";
            }
            else
            {
                return $"{(CheckSumType)hashType} Hash:{hash} \n listValue：{string.Join("!", listValue)} \n";
            }
        }
        else
        {
            return $"{(CheckSumType)hashType} Hash:{hash}";
        }
    }
}