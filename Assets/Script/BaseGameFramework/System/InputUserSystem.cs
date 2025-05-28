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
                    ProcessMsg(inputStruct);
                }
            }
        }
    }

    private void ProcessMsg(IInputStruct inputStruct)
    {
        if (inputStruct is UserPositionInput positionInput)
        {
            SystemAPI.TryGetSingletonEntity<UserMoveSpeedComponet>(out var entity);
            if (entity == Entity.Null)
            {
                return;
            }

            var x = SystemAPI.GetComponentRW<LComPosition>(entity);
            x.ValueRW.Value = new Deterministics.Math.fp3(positionInput.x, 0, positionInput.z);

            // var gameObjectrComponent = EntityManager.GetComponentObject<GameobjectrComponent>(entity);
            // gameObjectrComponent.gameObject.transform.position = new UnityEngine.Vector3(positionInput.x, 0, positionInput.z);
        }
    }
}