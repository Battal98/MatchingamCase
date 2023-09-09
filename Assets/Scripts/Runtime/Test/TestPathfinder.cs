using Runtime.LevelModule.Signals;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Test
{
    public class TestPathfinder : MonoBehaviour
    {
        private GridGraph map;

        private Vector2 _startPosition;
        private Vector2 _endPosition;

        [Tooltip("y parameters always be 2 or multiplier")]
        [SerializeField]
        private Vector2 gridGraphSize;

        private List<Vector3> positionList = new List<Vector3>();

        [SerializeField]
        private LineRenderer lineRenderer;
        [SerializeField]
        private List<TestPositionHandler> positionHandlerListForIsland = new List<TestPositionHandler>();

        private void Start()
        {
            Initialize();

            LevelSignals.Instance.onCreatePositionHandlerObjects?.Invoke();
        }

        public void AddListPositionHandler(TestPositionHandler positionHandler)
        {
            positionHandlerListForIsland.Add(positionHandler);
        }

        private void Initialize()
        {
            positionList.Clear();

            map ??= new GridGraph((int)gridGraphSize.x, (int)gridGraphSize.y);

            map.Walls ??= new List<Vector2>();
            map.Forests ??= new List<Vector2>();

            map.Walls.Clear();
        }

        public void PathFind(Vector2 startPosition, Vector2 endPosition)
        {
            Initialize();

            _startPosition = startPosition;
            _endPosition = endPosition;

            foreach (var positionhandler in positionHandlerListForIsland)
            {
                if (positionhandler.IsIsland && (_startPosition != positionhandler.Position && _endPosition != positionhandler.Position))
                {
                    map.Walls.Add(positionhandler.Position);
                }
            }

            var path = AStar.Search(map, map.Grid[(int)_startPosition.x, (int)_startPosition.y], map.Grid[(int)_endPosition.x, (int)_endPosition.y]);


            if (path is null)
                return;

            GridSignals.Instance.onRequestPosition?.Invoke(new Vector2(_startPosition.x,_startPosition.y));

            foreach (var item in path)
            {
                GridSignals.Instance.onRequestPosition?.Invoke(new Vector2(item.Position.x, item.Position.y));
            }

            GridSignals.Instance.onRequestPosition?.Invoke(new Vector2(_endPosition.x, _endPosition.y));
        }

        private void OnEnable()
        {
            GridSignals.Instance.onResponsePosition += OnResponsePosition;
        }

        private void OnResponsePosition(Vector3 obj)
        {
            positionList.Add(obj);

            lineRenderer.positionCount = positionList.Count;
            lineRenderer.SetPositions(positionList.ToArray());
        }

        private void OnDisable()
        {
            GridSignals.Instance.onResponsePosition -= OnResponsePosition;
        }
    }


}