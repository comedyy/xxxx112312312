
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class SyncFrameCache : IFetchFrame
{
    public int Count => _allMessage.Count;

    public string DebugInfo{
        get{
            if(Count == 0) return "== 0";
            else return _allMessage.First().Key.ToString();
        }
    }

    protected Dictionary<int , List<MessageItem>> _allMessage = new Dictionary<int, List<MessageItem>>();
    public void GetAllMessage(int frame, List<MessageItem> listOut)
    {
        if(_allMessage.TryGetValue(frame, out var list))
        {
            listOut.AddRange(list);
            ListPool<MessageItem>.Release(list);
            _allMessage.Remove(frame);
        }
    }
    
    public void AddLocalFrame(int frame, MessageItem item)
    {
        var list = ListPool<MessageItem>.Get();
        list.Add(item);
        _allMessage.Add(frame, list);
    }
}