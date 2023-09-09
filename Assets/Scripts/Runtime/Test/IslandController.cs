using NaughtyAttributes;
using Runtime.Abstactions;
using Runtime.IslandModule.Enums;
using System;
using UnityEngine;

namespace Runtime.Test
{
    public class IslandController : GridManager, IInteractable
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

        private Vector2 _pathPosition;

        private void Awake()
        {
            SetIslandState();
        }

        private void OnEnable()
        {
            GridSignals.Instance.onSetIslandPathPosition += OnSetPathPosition;
        }

        private void OnSetPathPosition(Vector2 arg0, GameObject obj)
        {
            if (obj != this.gameObject)
            {
                return;
            }
            _pathPosition = arg0;
        }

        private void OnDisable()
        {
            GridSignals.Instance.onSetIslandPathPosition -= OnSetPathPosition;
        }

        public Vector2 GetPathPosition()
        {
            return _pathPosition;
        }

        private void SetIslandState()
        {
            float gridSize = GetGridIndex();
            _maxGridSize = (int)gridSize * (int)gridSize;

            if (_maxGridSize == 0)
            {
                return;
            }

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
            Debug.Log("First");
            return isFirstObject;
        }

        public GameObject GetObject()
        {
            return this.gameObject;
        }

        public IslandController GetIslandController()
        {
            return this;
        }
    }
}