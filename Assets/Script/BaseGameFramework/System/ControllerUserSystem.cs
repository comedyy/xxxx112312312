using Unity.Entities;

public partial class ControllerUserSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (LocalFrame.Instance._inputCache == null)
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

        SystemAPI.TryGetSingletonEntity<UserMoveSpeedComponet>(out var entity);
        if (entity == Entity.Null)
        {
            return;
        }

        var gameObjectrComponent = EntityManager.GetComponentObject<GameobjectrComponent>(entity);
        var moveSpeedComponet = EntityManager.GetComponentData<UserMoveSpeedComponet>(entity);
        gameObjectrComponent.gameObject.transform.position = gameObjectrComponent.gameObject.transform.position += UnityEngine.Time.deltaTime * moveSpeedComponet.speed * dir;

        LocalFrame.Instance._inputCache.AddMsg(new UserPositionInput
        {
            x = (fp)gameObjectrComponent.gameObject.transform.position.x,
            z = (fp)gameObjectrComponent.gameObject.transform.position.z,
        });
    }
}