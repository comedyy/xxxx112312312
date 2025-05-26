

using System.Collections.Generic;
using Deterministics.Math;
using UnityEngine;

public class InputCache
{
    int _controllerId;
    public InputCache(int controllerId)
    {
        _controllerId = controllerId;
    }

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

    public bool FetchItem(out UserFrameInput messageItem)
    {
        if(_inputStructs.Count == 0)
        {
            messageItem = default;
            return false;
        }
        messageItem = new UserFrameInput
        {
            inputList = _inputStructs,
            id = _controllerId,
        };

        return true;
    }
}