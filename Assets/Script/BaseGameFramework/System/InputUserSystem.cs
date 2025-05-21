using System;
using System.Collections.Generic;
using Unity.Entities;

public partial class InputUserSystem : SystemBase
{
    List<MessageItem> _fetchInputList = new List<MessageItem>();
    Dictionary<int, Action<IInputStruct>> _overrideActions = new Dictionary<int, Action<IInputStruct>>();

    protected override void OnUpdate()
    {
        LocalFrame.Instance.GetFrameInput(_fetchInputList);

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
        if (inputStruct is PositionInputStruct positionInput)
        {
            SystemAPI.TryGetSingletonEntity<UserMoveSpeedComponet>(out var entity);
            if (entity == Entity.Null)
            {
                return;
            }

            var x = EntityManager.GetComponentData<LTransform>(entity);
            x.position = new Deterministics.Math.fp3(positionInput.x, 0, positionInput.z);

            // var gameObjectrComponent = EntityManager.GetComponentObject<GameobjectrComponent>(entity);
            // gameObjectrComponent.gameObject.transform.position = new UnityEngine.Vector3(positionInput.x, 0, positionInput.z);
        }
    }
}