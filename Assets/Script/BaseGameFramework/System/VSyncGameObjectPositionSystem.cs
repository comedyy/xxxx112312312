using Unity.Entities;

public partial class VSyncGameObjectPositionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var selfControllerId = BattleControllerMgr.Instance.GetController<SelfUserController>().selfControllerId;
        Entities.WithoutBurst().ForEach((ComHeroId comHeroId, GameobjectComponent gameObjectrComponent, ref VTransform vTransform) =>
        {
            if (selfControllerId == comHeroId.id) return;
            
            if (gameObjectrComponent.gameObject != null)
            {
                gameObjectrComponent.gameObject.transform.position = vTransform.position;
                gameObjectrComponent.gameObject.transform.rotation = vTransform.quaternion;
            }
        }).Run();
    }
}