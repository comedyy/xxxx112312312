using LiteNetLib.Utils;

public struct UserPositionInput : IInputStruct
{
    public fp x;
    public fp z;
    public bool isSingtonInput => true;
    public int structType => (int)InputType.Position;


    public int GetCheckSum()
    {
        return 0;
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(x.RawValue);
        writer.Put(z.RawValue);
    }

    public void Deserialize(NetDataReader reader)
    {
        x = fp.FromRaw(reader.GetLong());
        z = fp.FromRaw(reader.GetLong());
    }
}