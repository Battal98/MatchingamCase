using UnityEngine;

namespace Runtime.Test
{
    public class TestPositionHandler : MonoBehaviour
    {
        public Vector2 Position;

        public bool IsIsland = false;

        private void OnEnable()
        {
            GridSignals.Instance.RequestPosition += OnRequestPosition;
        }

        private void OnRequestPosition(Vector2 obj)
        {
            if (obj == Position)
            {
                GridSignals.Instance.ResponsePosition?.Invoke(this.transform.position);
            }
        }

        private void OnDisable()
        {
            GridSignals.Instance.RequestPosition -= OnRequestPosition;
            
        }
    }
}