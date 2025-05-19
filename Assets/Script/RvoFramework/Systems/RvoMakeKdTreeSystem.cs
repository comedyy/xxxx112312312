using Unity.Entities;

public partial class RvoMakeKdTreeSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        // Initialize the system here if needed
    }

    protected override void OnUpdate()
    {
        // Perform the KDTree update logic here
        // This is where you would implement the logic to update the KDTree
        // For example, you might call a method to build or update the KDTree based on your game logic
        UnityEngine.Debug.Log("RvoMakeKdTreeSystem OnUpdate");
    }
    
}