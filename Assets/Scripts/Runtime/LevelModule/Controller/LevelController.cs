using NaughtyAttributes;
using Runtime.LevelModule.Signals;
using Runtime.Test;
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
        private TestPathfinder pathfinder;

        [SerializeField]
        private GameObject positionHandlerObject;

        [SerializeField]
        private GameObject positionHandlerHolder;

        [SerializeField]
        private List<TestPositionHandler> positionHandlerObjects = new List<TestPositionHandler>();

        private void Start()
        {
            //CreatePositionHolderObject();
        }

        private void OnEnable()
        {
            LevelSignals.Instance.onCreatePositionHandlerObjects += OnCreatePositionHHandlerObject;
        }

        private void OnDisable()
        {
            LevelSignals.Instance.onCreatePositionHandlerObjects += OnCreatePositionHHandlerObject;
        }

        [Button]
        private void OnCreatePositionHHandlerObject()
        {
            int gridSize = leftIslands.Count * rightIslands.Count;

            for (int i = 0; i < gridSize; i++)
            {
                var obj = Instantiate(positionHandlerObject, positionHandlerHolder.transform);
                var objComponent = obj.GetComponent<TestPositionHandler>();
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
                            + new Vector3(0, 0, k * -2.5f);

                        PositionHandler(t);
                    }
                    else
                    {
                        var newIndex = Mathf.Abs(i - leftIslands.Count);
                        positionHandlerObjects[t].Position = new Vector2(k+2, newIndex);
                        positionHandlerObjects[t].gameObject.transform.position = rightIslands[newIndex].transform.position
                            + new Vector3(0, 0, (k-1) * -2.5f);

                        PositionHandler(t);
                    }

                    t++;
                }

            }
        }

        private void PositionHandler(int i)
        {
            if (positionHandlerObjects[i].Position.x == 0 || positionHandlerObjects[i].Position.x == 3)
            {
                positionHandlerObjects[i].IsIsland = true;
                pathfinder.AddListPosiitonHandler(positionHandlerObjects[i]);
            }
        }
    }
}