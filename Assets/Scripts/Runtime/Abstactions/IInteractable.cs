using Runtime.PathfindModule;
using UnityEngine;

namespace Runtime.Abstactions
{
    public interface IInteractable
    {
        Vector2 GetPathPosition();
        IslandController GetIslandController();

        bool IsInteractable();
    }
}
