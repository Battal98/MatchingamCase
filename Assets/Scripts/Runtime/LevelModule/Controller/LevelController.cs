using Runtime.LevelModule.Datas;
using Runtime.LevelModule.Signals;
using Runtime.PathfindModule;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.LevelModule.Controller
{
    public class LevelController : GridManager
    {
        [SerializeField] 
        private List<GameObject> leftIslands = new List<GameObject>();
        [SerializeField] 
        private List<GameObject> rightIslands = new List<GameObject>();

        [SerializeField]
        private Pathfinder pathfinder;

        [SerializeField]
        private GameObject positionHandlerObject;

        [SerializeField]
        private GameObject positionHandlerHolder;

        private List<PositionHandler> _positionHandlerObjects = new List<PositionHandler>();

        private GameLevelData _levelData;

        private int completedCount;

        private const float _positionHandlerZOffset = -1.5f;

        private void OnEnable()
        {
            LevelSignals.Instance.onCreatePositionHandlerObjects += OnCreatePositionHandlerObject;
            LevelSignals.Instance.onLevelInitializeDone += OnLevelInitializeDone;
            LevelSignals.Instance.onCalculateCopmletedCount += OnCalculateCompletedCount;
        }

        private void OnDisable()
        {
            LevelSignals.Instance.onCreatePositionHandlerObjects -= OnCreatePositionHandlerObject;
            LevelSignals.Instance.onLevelInitializeDone -= OnLevelInitializeDone;
            LevelSignals.Instance.onCalculateCopmletedCount -= OnCalculateCompletedCount;
        }

        private void OnLevelInitializeDone()
        {
            _levelData = LevelSignals.Instance.onSendToData.Invoke();
        }

        private void OnCalculateCompletedCount()
        {
            completedCount++;

            if (completedCount >= _levelData.LevelCompletedCount)
            {
                LevelSignals.Instance.onLevelSuccessful?.Invoke();
            }
        }

        private void OnCreatePositionHandlerObject()
        {
            int gridSize = (leftIslands.Count + rightIslands.Count) * 2;

            for (int i = 0; i < gridSize; i++)
            {
                var obj = Instantiate(positionHandlerObject, positionHandlerHolder.transform);
                var objComponent = obj.GetComponent<PositionHandler>();
                _positionHandlerObjects.Add(objComponent);
            }

            int t = 0;

            for (int k = 0; k < 2; k++)
            {
                for (int i = 0; i < leftIslands.Count + rightIslands.Count; i++)
                {
                    if (i < leftIslands.Count)
                    {
                        _positionHandlerObjects[t].Position = new Vector2(k, i);
                        _positionHandlerObjects[t].gameObject.transform.position = leftIslands[i].transform.position
                            + new Vector3(0, (leftIslands[i].transform.localScale.y/2f), (k * _positionHandlerZOffset) - (leftIslands[i].transform.localScale.z / 2f));

                        PositionHandler(t, leftIslands[i].gameObject);
                    }
                    else
                    {
                        var newIndex = Mathf.Abs(i - leftIslands.Count);
                        _positionHandlerObjects[t].Position = new Vector2(k+2, newIndex);
                        _positionHandlerObjects[t].gameObject.transform.position = rightIslands[newIndex].transform.position
                            + new Vector3(0, (rightIslands[newIndex].transform.localScale.y / 2f), ((k-1) * _positionHandlerZOffset) + (rightIslands[newIndex].transform.localScale.z / 2f));

                        PositionHandler(t, rightIslands[newIndex].gameObject);
                    }

                    t++;
                }

            }
        }

        private void PositionHandler(int i, GameObject islandObject)
        {
            if (_positionHandlerObjects[i].Position.x == 0 || _positionHandlerObjects[i].Position.x == 3)
            {
                _positionHandlerObjects[i].IsIsland = true;

                pathfinder.AddListPositionHandler(_positionHandlerObjects[i]);

                GridSignals.Instance.onSetIslandPathPosition?.Invoke(_positionHandlerObjects[i].Position, islandObject);
            }
        }
    }
}