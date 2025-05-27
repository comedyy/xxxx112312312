using LiteNetLib.Utils;

public interface IInputStruct : INetSerializable, IGetCheckSum
{
    bool isSingtonInput { get; }
    int structType { get; }
}
