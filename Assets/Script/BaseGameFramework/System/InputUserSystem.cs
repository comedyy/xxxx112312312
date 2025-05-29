using System;
using System.Collections.Generic;
using Unity.Entities;
public interface IFetchFrame
{
    void GetAllMessage(int frame, List<UserFrameInput> messageItems);
}

public partial class InputUserSystem : SystemBase
{
    public static List<UserFrameInput> _fetchInputList = new List<UserFrameInput>();
    Dictionary<int, Action<IInputStruct>> _overrideActions = new Dictionary<int, Action<IInputStruct>>();
    public IFetchFrame fetchFrame;

    protected override void OnUpdate()
    {
        var frameComponent =  SystemAPI.GetSingleton<ComFrameCount>();
        _fetchInputList.Clear();
        fetchFrame.GetAllMessage(frameComponent.frameLogic, _fetchInputList);

        foreach (var x in _fetchInputList)
        {
            foreach (var inputStruct in x.inputList)
            {
                if (_overrideActions.TryGetValue(inputStruct.structType, out var action))
                {
                    action(inputStruct);
                }
                else
                {
                    ProcessMsg(x.id, inputStruct);
                }
            }
        }
    }

    public Entity GetUserEntity(int id)
    {
        var buffer = SystemAPI.GetSingletonBuffer<BufferUserEntity>();
        foreach (var x in buffer)
        {
            if (x.id == id) return x.entity;
        }
        
        throw new Exception($"GetUserEntity {id}");
    }

    private void ProcessMsg(int id, IInputStruct inputStruct)
    {
        var entity = GetUserEntity(id);
        if (inputStruct is UserPositionInput positionInput)
        {
            var x = SystemAPI.GetComponentRW<LComPosition>(entity);
            x.ValueRW.Value = new Deterministics.Math.fp3(positionInput.x, 0, positionInput.z);
        }
    }
}