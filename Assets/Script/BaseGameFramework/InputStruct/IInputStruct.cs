using LiteNetLib.Utils;

public interface IInputStruct : INetSerializable
{
    bool isSingtonInput { get; }
    int structType { get; }
    int GetCheckSum();
}