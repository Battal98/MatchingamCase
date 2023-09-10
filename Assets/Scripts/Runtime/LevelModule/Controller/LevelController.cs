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

        [SerializeField]
        private List<PositionHandler> positionHandlerObjects = new List<PositionHandler>();

        private void OnEnable()
        {
            LevelSignals.Instance.onCreatePositionHandlerObjects += OnCreatePositionHandlerObject;
        }

        private void OnDisable()
        {
            LevelSignals.Instance.onCreatePositionHandlerObjects += OnCreatePositionHandlerObject;
        }

        private void OnCreatePositionHandlerObject()
        {
            int gridSize = leftIslands.Count * rightIslands.Count;

            for (int i = 0; i < gridSize; i++)
            {
                var obj = Instantiate(positionHandlerObject, positionHandlerHolder.transform);
                var objComponent = obj.GetComponent<PositionHandler>();
                positionHandlerObjects.Add(objComponent);
            }

            int t = 0;

            for (int k = 0; k < 2; k++)
            {
                for (int i = 0; i < leftIslands.Count + rightIslands.Count; i++)
                {
                    if (i < leftIslands.Count)
                    {
                        positionHandlerObjects[t].Position = new Vector2(k, i);
                        positionHandlerObjects[t].gameObject.transform.position = leftIslands[i].transform.position
                            + new Vector3(0, (leftIslands[i].transform.localScale.y/2f), (k * -1.5f) - (leftIslands[i].transform.localScale.z / 2f));

                        PositionHandler(t, leftIslands[i].gameObject);
                    }
                    else
                    {
                        var newIndex = Mathf.Abs(i - leftIslands.Count);
                        positionHandlerObjects[t].Position = new Vector2(k+2, newIndex);
                        positionHandlerObjects[t].gameObject.transform.position = rightIslands[newIndex].transform.position
                            + new Vector3(0, (rightIslands[newIndex].transform.localScale.y / 2f), ((k-1) * -1.5f) + (rightIslands[newIndex].transform.localScale.z / 2f));

                        PositionHandler(t, rightIslands[newIndex].gameObject);
                    }

                    t++;
                }

            }
        }

        private void PositionHandler(int i, GameObject islandObject)
        {
            if (positionHandlerObjects[i].Position.x == 0 || positionHandlerObjects[i].Position.x == 3)
            {
                positionHandlerObjects[i].IsIsland = true;
                pathfinder.AddListPositionHandler(positionHandlerObjects[i]);
                GridSignals.Instance.onSetIslandPathPosition?.Invoke(positionHandlerObjects[i].Position, islandObject);
            }
        }
    }
}