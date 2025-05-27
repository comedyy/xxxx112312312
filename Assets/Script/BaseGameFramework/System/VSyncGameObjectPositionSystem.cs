using Unity.Entities;

public partial class VSyncGameObjectPositionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (LocalFrame.Instance.CanInput)
        {
            return;
        }

        Entities.WithoutBurst().ForEach((GameobjectrComponent gameObjectrComponent, ref VTransform vTransform) =>
        {
            if (gameObjectrComponent.gameObject != null)
            {
                gameObjectrComponent.gameObject.transform.position = vTransform.position;
                gameObjectrComponent.gameObject.transform.rotation = vTransform.quaternion;
            }
        }).Run();
    }
}