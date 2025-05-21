using System;
using System.Collections.Generic;

public class LocalFrame : ILocalFrame
{
    public static LocalFrame Instance { get; private set; }
    public int GameFrame;
    public int ReceivedServerFrame;
    ILocalFrame _localFrameInstance;

    public LocalFrame(int id, BattleType battleType)
    {
        if (battleType == BattleType.Client)
        {
            _localFrameInstance = new LocalFrameClientInsance(this);
        }

        Instance = this;
    }

    public void Update()
    {
        _localFrameInstance.Update();
    }

    public void Dispose()
    {
        _localFrameInstance.Dispose();
    }


    public Dictionary<int, List<MessageItem>> _allMessage = new Dictionary<int, List<MessageItem>>();
    public List<IInputStruct> _inputStructs = new List<IInputStruct>();
    public void AddMsg<T>(T x) where T : struct, IInputStruct
    {
        if (x.isSingtonInput)
        {
            for (int i = 0; i < _inputStructs.Count; i++)
            {
                if (_inputStructs[i].structType == x.structType)
                {
                    _inputStructs[i] = x;
                    return;
                }
            }
        }

        _inputStructs.Add(x);
    }
    
    
    public void GetFrameInput(List<MessageItem> listOut)
    {
        var frame = GameFrame;
        if (_allMessage.TryGetValue(frame, out var list))
        {
            listOut.AddRange(list);
            _allMessage.Remove(frame);
        }
    }
}