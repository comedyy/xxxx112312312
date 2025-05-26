using System.Collections.Generic;
using LiteNetLib.Utils;

public struct UserFrameInput : INetSerializable
{ 
    public int id;
    public List<IInputStruct> inputList;

    public void Deserialize(NetDataReader reader)
    {
        id = reader.GetInt();
        int count = reader.GetInt();
        inputList = new List<IInputStruct>(count);
        for (int i = 0; i < count; i++)
        {
            var type = (InputType)reader.GetInt();
            IInputStruct inputStruct = InputStructFactory.Create(type);
            inputStruct.Deserialize(reader);
            inputList.Add(inputStruct);
        }
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(id);
        writer.Put(inputList.Count);
        foreach (var input in inputList)
        {
            writer.Put(input.structType);
            input.Serialize(writer);
        }
    }
}