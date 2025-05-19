using Unity.Entities;

namespace Game.BattleShare.ECS.SystemGroup
{
    public partial class BaseUnsortSystemGroup : ComponentSystemGroup
    {
        public BaseUnsortSystemGroup()
        {
            EnableSystemSorting = false;
        }
    }
}