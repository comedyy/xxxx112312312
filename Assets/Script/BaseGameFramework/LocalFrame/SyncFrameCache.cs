
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
    
    public void AddLocalFrame(int frame, UserFrameInput item)
    {
        var list = ListPool<UserFrameInput>.Get();
        list.Add(item);
        _allMessage.Add(frame, list);
    }
}