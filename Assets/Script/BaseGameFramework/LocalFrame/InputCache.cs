

using System.Collections.Generic;
using Deterministics.Math;
using UnityEngine;

public class InputCache
{
    int _controllerId;
    public bool CanInput { get; set; } = true;

    public InputCache(int controllerId)
    {
        _controllerId = controllerId;
    }

    public List<IInputStruct> _inputStructs = new List<IInputStruct>();
    public void AddMsg<T>(T x) where T : struct, IInputStruct
    {
        if (!CanInput)
        {
            Debug.LogWarning($"InputCache: {typeof(T)} cannot be added because CanInput is false.");
            return;
        }

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

    public UserFrameInput? FetchItem()
    {
        if (_inputStructs.Count == 0)
        {
            return null;
        }

        var messageItem = new UserFrameInput
        {
            inputList = _inputStructs,
            id = _controllerId,
        };

        return messageItem;
    }
}