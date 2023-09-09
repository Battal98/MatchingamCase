using NaughtyAttributes;
using Runtime.LevelModule.Signals;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Test
{
    public class TestPathfinder : MonoBehaviour
    {
        private GridGraph map;

        [SerializeField]
        private Vector2 startPosition;
        [SerializeField]
        private Vector2 endPosition;

        [Tooltip("y parameters always be 2 or multiplier")]
        [SerializeField]
        private Vector2 gridGraphSize;

        private List<Vector3> positionList = new List<Vector3>();

        [SerializeField]
        private LineRenderer lineRenderer;
        [SerializeField]
        private List<TestPositionHandler> positionHandlerList = new List<TestPositionHandler>();

        private void Start()
        {
            Initialize();

            LevelSignals.Instance.onCreatePositionHandlerObjects?.Invoke();
        }

        public void AddListPosiitonHandler(TestPositionHandler positionHandler)
        {
            positionHandlerList.Add(positionHandler);
        }

        private void Initialize()
        {
            positionList.Clear();

            map ??= new GridGraph((int)gridGraphSize.x, (int)gridGraphSize.y);

            map.Walls ??= new List<Vector2>();
            map.Forests ??= new List<Vector2>();

            map.Walls.Clear();
        }

        [Button]
        public void PathFind()
        {
            Initialize();

            foreach (var positionhandler in positionHandlerList)
            {
                if (positionhandler.IsIsland && (startPosition != positionhandler.Position && endPosition != positionhandler.Position))
                {
                    map.Walls.Add(positionhandler.Position);
                }
            }

            var path = AStar.Search(map, map.Grid[(int)startPosition.x, (int)startPosition.y], map.Grid[(int)endPosition.x, (int)endPosition.y]);


            if (path is null)
                return;

            GridSignals.Instance.RequestPosition?.Invoke(new Vector2(startPosition.x,startPosition.y));

            foreach (var item in path)
            {
                GridSignals.Instance.RequestPosition?.Invoke(new Vector2(item.Position.x, item.Position.y));
            }

            GridSignals.Instance.RequestPosition?.Invoke(new Vector2(endPosition.x, endPosition.y));
        }

        private void OnEnable()
        {
            GridSignals.Instance.ResponsePosition += OnResponsePosition;
        }

        private void OnResponsePosition(Vector3 obj)
        {
            positionList.Add(obj);

            lineRenderer.positionCount = positionList.Count;
            lineRenderer.SetPositions(positionList.ToArray());
        }

        private void OnDisable()
        {
            GridSignals.Instance.ResponsePosition -= OnResponsePosition;
        }
    }


}