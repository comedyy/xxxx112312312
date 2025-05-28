
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class SyncFrameCache : IFetchFrame, IPutMessage
{
    public int Count => _allMessage.Count;
    public int ReceivedServerFrame { get; private set; }

    public string DebugInfo
    {
        get
        {
            if (Count == 0) return "== 0";
            else return _allMessage.First().Key.ToString();
        }
    }

    protected Dictionary<int , List<UserFrameInput>> _allMessage = new Dictionary<int, List<UserFrameInput>>();
    public void GetAllMessage(int frame, List<UserFrameInput> listOut)
    {
        if(_allMessage.TryGetValue(frame, out var list))
        {
            listOut.AddRange(list);
            ListPool<UserFrameInput>.Release(list);
            _allMessage.Remove(frame);
        }
    }
    
    public void AddLocalFrame(int frame, UserFrameInput? item)
    {
        if (item == null)
        {
            ReceivedServerFrame = frame;
            return;
        }

        var list = ListPool<UserFrameInput>.Get();
        list.Add(item.Value);
        AddFrameWithList(frame, list);
    }

    public void AddFrameWithList(int frame, List<UserFrameInput> item)
    {
        if (_allMessage.ContainsKey(frame))
        {
            UnityEngine.Debug.LogError($"AddLocalFrame duplicated key: {frame}");
            return;
        }

        if (frame != ReceivedServerFrame + 1)
        {
            UnityEngine.Debug.LogError($"AddLocalFrame not continue: {frame} {ReceivedServerFrame}");
            return;
        }

        if (item != null)
        {
            if (item.Count != 0)
            {
                _allMessage.Add(frame, item);
            }
            else
            {
                ListPool<UserFrameInput>.Release(item);
            }
        }

        ReceivedServerFrame = frame;
    }
}
