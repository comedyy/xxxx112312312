using Unity.Entities;

public partial class RvoSyncPositionFromRvoSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        // Initialize the system here if needed
    }

    protected override void OnUpdate()
    {
        // Perform the logic to sync positions from RVO here
        // This is where you would implement the logic to sync positions from RVO
        // For example, you might call a method to update the positions of entities based on RVO calculations
        UnityEngine.Debug.Log("RvoSyncPositionFromRvoSystem OnUpdate");
    }
    
}