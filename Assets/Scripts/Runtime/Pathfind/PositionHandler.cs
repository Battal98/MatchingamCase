using UnityEngine;

namespace Runtime.PathfindModule
{
    public class PositionHandler : MonoBehaviour
    {
        public Vector2 Position;

        public bool IsIsland = false;

        private void OnEnable()
        {
            GridSignals.Instance.onRequestPosition += OnRequestPosition;
        }

        private void OnRequestPosition(Vector2 pos)
        {
            if (pos == Position)
            {
                GridSignals.Instance.onResponsePosition?.Invoke(this.transform.position);
            }
        }

        private void OnDisable()
        {
            GridSignals.Instance.onRequestPosition -= OnRequestPosition;
            
        }
    }
}