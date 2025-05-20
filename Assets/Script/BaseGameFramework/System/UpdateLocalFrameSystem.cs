using Unity.Entities;

public partial class UpdateLocalFrameSystem : SystemBase
{
    protected override void OnUpdate()
    {
        LocalFrame.Instance.Update();
    }

    protected override void OnDestroy()
    {
        LocalFrame.Instance.Dispose();
    }
}