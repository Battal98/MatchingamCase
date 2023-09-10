﻿using DG.Tweening;
using NaughtyAttributes;
using Runtime.Abstactions;
using Runtime.IslandModule.Controller;
using Runtime.IslandModule.Enums;
using Runtime.LevelModule.Signals;
using Runtime.Pathfind;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Runtime.PathfindModule
{
    [Serializable]
    public struct CharacterData
    {
        public CharacterColor ColorType;
    }

    public class IslandController : GridManager, IInteractable
    {
        [BoxGroup(TAG)]
        [SerializeField]
        private bool isFirstObject = false;

        [BoxGroup(TAG)]
        [SerializeField]
        private IslandState islandState;

        [BoxGroup(TAG)]
        [SerializeField]
        private Vector2 characterGridSize;

        [BoxGroup(TAG)]
        [SerializeField]
        private List<CharacterData> initializeCharacterDatas = new List<CharacterData>();

        [BoxGroup(TAG)]
        [SerializeField]
        private List<Slot> fullSlots = new List<Slot>();

        [BoxGroup(TAG)]
        [SerializeField]
        private List<HumanController> movableObjects = new List<HumanController>();

        private bool _isIslandFull = false;
        private bool _iHaveACharacter = false;
        private bool _isCompleted = false;

        private int _maxGridSize;
        private int _emptySlotCount;

        private const string TAG = "SELF";

        private Vector2 _pathPosition;

        private void Awake()
        {
            OnSetCharactersAndIsland();
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

        public bool IsInteractable()
        {
            return _iHaveACharacter;
        }

        private void OnSetCharactersAndIsland()
        {
            SetInitializeCharacters();

            SetInitializeIslandState();
        }

        public void SetInitializeIslandState()
        {
            int emptyCount = 0;
            int fullCount = 0;

            fullSlots.Clear();

            for (int i = 0; i < GetSlotList().Count; i++)
            {
                if (GetSlotList()[i].GetSlotState() == SlotState.Empty)
                {
                    emptyCount++;
                }
                else if (GetSlotList()[i].GetSlotState() == SlotState.Full)
                {
                    fullCount++;
                    fullSlots.Add(GetSlotList()[i]);
                    _iHaveACharacter = true;
                }
            }

            int totalCount = GetSlotList().Count;

            if (emptyCount == totalCount)
            {
                islandState = IslandState.Empty;
                _iHaveACharacter = false;
            }
            else if (fullCount == totalCount / 4)
            {
                islandState = IslandState.Quarter;
            }
            else if (fullCount == totalCount / 2)
            {
                islandState = IslandState.Half;
            }
            else if (fullCount == (totalCount / 4) * 3)
            {
                islandState = IslandState.QuarterHalf;
            }
            else if (fullCount == totalCount)
            {
                islandState = IslandState.Full;
                _isIslandFull = true;
            }

            CalculateMovableObjects();
        }

        public int GetEmptySlotCount()
        {
            _emptySlotCount = 0;
            
            for (int i = 0; i < GetSlotList().Count; i++)
            {
                if (GetSlotList()[i].GetSlotState() == SlotState.Empty)
                {
                    _emptySlotCount++;
                }
            }
            return _emptySlotCount;
        }

        public Vector3 GetEmptySlotPosition(GameObject childObject)
        {
            Vector3 vectorHolder = Vector3.zero;

            for (int i = 0; i < GetSlotList().Count; i++)
            {
                if (GetSlotList()[i].GetSlotState() == SlotState.Empty)
                {
                    GetSlotList()[i].SetParentForHuman(childObject);
                    GetSlotList()[i].SetSlotState(SlotState.Full);

                    Vector3 offset = new Vector3(0, (this.transform.localScale.y / 2f) - 0.2f, 0);

                    vectorHolder = GetSlotList()[i].gameObject.transform.position + offset;

                    return vectorHolder;
                }
            }

            return vectorHolder;
        }

        public bool HasEnoughEmptySlots(int numHumansToMove)
        {
            return GetEmptySlotCount() >= numHumansToMove;
        }

        public bool HasSameColorGroup(List<HumanController> movingHumans)
        {
            if (fullSlots.Count != 0)
            {
                foreach (var human in movingHumans)
                {
                    if (human.GetColorType() != fullSlots[fullSlots.Count - 1].GetSlotColorType())
                    {
                        return false;
                    }
                }
                return true;
            }
            return true;
        }

        private void CalculateMovableObjects()
        {
            movableObjects.Clear();

            for (int i = fullSlots.Count - 1; i >= 0; i--)
            {
                if (fullSlots[i].GetSlotColorType() == fullSlots[fullSlots.Count - 1].GetSlotColorType())
                {
                    movableObjects.Add(fullSlots[i].GetActiveObjectController());
                }
                else
                {
                    break;
                }
            }
            movableObjects.Reverse();
        }
        public async void ChooseCharactersForMovement(List<Vector3> pathList, IslandController targetIsland)
        {
            int SlotListCount = GetSlotList().Count;

            CalculateMovableObjects();

            if (targetIsland.HasEnoughEmptySlots(movableObjects.Count) && targetIsland.HasSameColorGroup(movableObjects) && !_isCompleted)
            {
                for (int k = 0; k < pathList.Count; k++)
                {
                    pathList[k] += new Vector3(0, +0.2f, 0);
                }

                for (int i = movableObjects.Count - 1; i >= 0; i--)
                {
                    var movableObject = movableObjects[i]; 

                    var lastPosition = targetIsland.GetEmptySlotPosition(movableObject.gameObject);

                    pathList.Add(lastPosition);

                    movableObject.SetAnimation(HumanAnimation.Run);

                    bool isClearPath = i == 0;

                    movableObject.transform.DOPath(pathList.ToArray(), 3f, PathType.Linear).OnWaypointChange(index =>
                    {
                        if (index < pathList.Count - 1)
                        {
                            Vector3 direction = (pathList[index + 1] - pathList[index]).normalized;

                            Quaternion rotation = Quaternion.LookRotation(-direction, Vector3.up);

                            movableObject.transform.DORotateQuaternion(rotation, 0.1f);
                        }
                    }).OnComplete(() =>
                    {
                        movableObject.SetAnimation(HumanAnimation.Idle);

                        movableObject.transform.DORotate(new Vector3(0,90,0), 0.3f, RotateMode.LocalAxisAdd);

                        if (isClearPath)
                        {
                            PathSignals.Instance.onClearPath?.Invoke();
                        }
                    });

                    await Task.Delay(TimeSpan.FromSeconds(0.5f));

                    RemoveHumanFromSourceIsland(movableObject.gameObject);

                    targetIsland.SetInitializeIslandState();

                    targetIsland.SetSlotsColorType(movableObject.GetColorType());
                }

                this.SetInitializeIslandState();
            }
            else
            {
                PathSignals.Instance.onClearPath?.Invoke();
            }
        }

        public void IsCompletedIsland(bool state)
        {
            _isCompleted = state;
        }

        public void SetSlotsColorType(CharacterColor colorType)
        {
            for (int i = 0; i < fullSlots.Count; i++)
            {
                fullSlots[i].SetSlotColor(colorType);
            }

            if (IsTargetIslandFullAndSameColor())
            {
                IsCompletedIsland(true);

                LevelSignals.Instance.onCalculateCopmletedCount?.Invoke();
            }
        }

        private void RemoveHumanFromSourceIsland(GameObject human)
        {
            // Iterate through the slots in the source island to find the slot containing the human
            foreach (var slot in GetSlotList())
            {
                if (slot.GetSlotState() == SlotState.Full)
                {
                    // Check if the current slot contains the specified human
                    if (slot.GetCharacteObject() == human)
                    {
                        // Remove the human from the slot
                        slot.SetSlotState(SlotState.Empty);
                        break; // Exit the loop since we found and removed the human
                    }
                }
            }
        }

        public bool IsFirstObject(bool isFirst = false)
        {
            return isFirstObject;
        }

        public IslandController GetIslandController()
        {
            return this;
        }

        public bool IsTargetIslandFullAndSameColor()
        {
            // Check if the target island is full
            if (islandState == IslandState.Full)
            {
                // Check if all color groups are the same
                if (fullSlots.Count > 0)
                {
                    CharacterColor targetColor = fullSlots[0].GetSlotColorType();
                    foreach (var slot in fullSlots)
                    {
                        if (slot.GetSlotColorType() != targetColor)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            else
            {
                return false;
            }

            return true;
        }
        public void SetInitializeCharacters()
        {
            int t = 0;

            if (initializeCharacterDatas.Count == 0)
            {
                _iHaveACharacter = false;
                return;
            }

            _iHaveACharacter = true;

            for (int k = 0; k < GetSlotList().Count; k++)
            {
                if (t >= initializeCharacterDatas.Count)
                {
                    return;
                }

                GetSlotList()[k].SetCharacterColorInitialize(initializeCharacterDatas[t].ColorType);

                if ((k + 1) % 4 == 0) // Check if we've assigned 4 colors, then move to the next color
                {
                    t = (t + 1) % GetSlotList().Count; // Use modular arithmetic to cycle through colorList
                }
            }

        }
    }
}