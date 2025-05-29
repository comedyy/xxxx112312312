using Unity.Entities;
using UnityEngine;

public partial class ControllerUserSystem : SystemBase
{
    GameObject _go;
    RefRO<UserMoveSpeedComponet> _moveComponentCache;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        var selfEntity = BattleControllerMgr.Instance.GetController<SelfUserController>()._selfEntity;
        _go = EntityManager.GetComponentObject<GameobjectComponent>(selfEntity).gameObject;
        _moveComponentCache = SystemAPI.GetComponentRO<UserMoveSpeedComponet>(selfEntity);
    }

    protected override void OnUpdate()
    {
        if (!LocalFrame.Instance.CanInput)
        {
            return;
        }

        if (SystemAPI.GetSingleton<ComGameState>().IsEnd)
        {
            return;
        }

        var x = UnityEngine.Input.GetAxis("Horizontal");
        var y = UnityEngine.Input.GetAxis("Vertical");
        var dir = new UnityEngine.Vector3(x, 0, y).normalized;

        _go.gameObject.transform.position = _go.gameObject.transform.position += UnityEngine.Time.deltaTime * _moveComponentCache.ValueRO.speed * dir;

        LocalFrame.Instance._inputCache.AddMsg(new UserPositionInput
        {
            x = (fp)_go.gameObject.transform.position.x,
            z = (fp)_go.gameObject.transform.position.z,
        });
    }
}