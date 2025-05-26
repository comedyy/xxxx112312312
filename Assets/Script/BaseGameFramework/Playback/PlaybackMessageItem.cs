
using System.Collections.Generic;
using LiteNetLib.Utils;

public enum PlaybackBit : byte
{
    Package = 1 << 0,
    Hash = 1 << 1,
    ChangeState = 1 << 2, // 同样是占位符
    GameEnd = 1 << 3, // 同样是占位符
    ForceSavePoint = 1 << 4, // 占位符，需要有东西保存到硬盘。
}

public struct PlaybackMessageItem : INetSerializable
{
    public PlaybackBit playbackBit;
    public ushort frame;
    public List<UserFrameInput> list;
    public MessageHash hash;
    public byte currentState;

    void INetSerializable.Serialize(NetDataWriter writer)
    {
        writer.Put((byte)playbackBit);
        writer.Put(frame);

        if((playbackBit & PlaybackBit.Package) > 0)
        {
            writer.Put((byte)list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                writer.Put(list[i]);
            }
        }
        if((playbackBit & PlaybackBit.ChangeState) > 0)
        {
            writer.Put(currentState);
        }
        if((playbackBit & PlaybackBit.Hash) > 0)
        {
            writer.Put(hash);
        }
    }

    void INetSerializable.Deserialize(NetDataReader reader)
    {
        playbackBit = (PlaybackBit)reader.GetByte();
        frame = reader.GetUShort();

        if((playbackBit & PlaybackBit.Package) > 0)
        {
            var count = reader.GetByte();
            list = ListPool<UserFrameInput>.Get();
            for (int i = 0; i < count; i++)
            {
                list.Add(reader.Get<UserFrameInput>());
            }
        }

        if((playbackBit & PlaybackBit.ChangeState) > 0)
        {
            currentState = reader.GetByte();
        }

        if((playbackBit & PlaybackBit.Hash) > 0)
        {
            hash = reader.Get<MessageHash>();
        }
    }
}