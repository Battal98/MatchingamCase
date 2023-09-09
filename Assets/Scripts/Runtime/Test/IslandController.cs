using NaughtyAttributes;
using Runtime.IslandModule.Enums;
using UnityEngine;

namespace Runtime.Test
{
    public class IslandController : GridManager
    {
        [BoxGroup(TAG)]
        [SerializeField]
        private bool isFirstObject = false;
        [BoxGroup(TAG)]
        [SerializeField]
        private IslandState islandState;

        private bool _isIslandFull = false;

        private int _maxGridSize;

        private const string TAG = "SELF";

        private void Awake()
        {
            SetIslandState();
        }

        private void SetIslandState()
        {
            float gridSize = GetGridIndex();
            _maxGridSize = (int)gridSize * (int)gridSize;

            if (GetISlotList().Count >= _maxGridSize)
            {
                for (int i = 0; i < GetISlotList().Count; i++)
                {
                    if (GetISlotList()[i].GetSlotState() == SlotState.Empty)
                    {
                        islandState = IslandState.Usable;
                        break;
                    }
                    else
                    {
                        islandState = IslandState.Full;
                    }
                }
            }

            else
            {
                islandState = IslandState.Usable;
            }

        }

        private void OnMouseDown()
        {
            Debug.Log("clicked me : " + this.gameObject.name);
        }

        public bool IsFirstObject(bool isFirst = false)
        {
            return isFirstObject;
        }
    }
}