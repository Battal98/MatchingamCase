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
using Unity.Mathematics;
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
        private IslandState islandState;

        [BoxGroup(TAG)]
        [SerializeField]
        private Vector2 characterGridSize;

        [BoxGroup(TAG)]
        [SerializeField]
        private List<CharacterData> initializeCharacterDatas = new List<CharacterData>();

        [BoxGroup(TAG)]
        [SerializeField]
        private ParticleSystem conffetParticle;

        private List<Slot> _fullSlots = new List<Slot>();

        private List<HumanController> _movableObjects = new List<HumanController>();

        private bool _isFirstObject = false;
        private bool _iHaveACharacter = false;
        private bool _isCompleted = false;

        private int _maxGridSize;
        private int _emptySlotCount;

        private int _emptyCount = 0;
        private int _fullCount = 0;

        private Vector2 _pathPosition;

        private const string TAG = "SELF";

        private const float _doPathDuration = 2f;
        private const float _pathYOffset = 0.2f;
        private const float _doRotateQuarternionDuration = 0.1f;
        private const float _doRotateDuration = 0.3f;
        private const float _durationBetweenMovableObjects = 0.5f;
        private const float _humanPositionYOffset = 0.075f;

        private const int _setCharacterModValue = 4;
        private const int _quarter = 4;
        private const int _half = 4;
        private const int _quarterHalf = 3;

        private void Awake()
        {
            OnSetCharactersAndIsland();
        }

        private void OnEnable()
        {
            GridSignals.Instance.onSetIslandPathPosition += OnSetPathPosition;
        }

        private void OnSetPathPosition(Vector2 pathPosition, GameObject sendedObject)
        {
            if (sendedObject != this.gameObject)
            {
                return;
            }

            _pathPosition = pathPosition;
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

        public void PlayConfettiParticle()
        {
            if (!_isCompleted)
            {
                return;
            }

            conffetParticle.Play();

            LevelSignals.Instance.onCalculateCopmletedCount?.Invoke();
        }

        private void OnSetCharactersAndIsland()
        {
            SetInitializeCharacters();

            SetInitializeIslandState();
        }

        public void SetInitializeIslandState()
        {
            CalculateFullSlot();

            CalculateIslandState(_emptyCount, _fullCount);

            CalculateMovableObjects();
        }

        private void CalculateFullSlot()
        {
            _emptyCount = 0;
            _fullCount = 0;

            _fullSlots.Clear();

            for (int i = 0; i < GetSlotList().Count; i++)
            {
                if (GetSlotList()[i].GetSlotState() == SlotState.Empty)
                {
                    _emptyCount++;
                }

                else if (GetSlotList()[i].GetSlotState() == SlotState.Full)
                {
                    _fullCount++;

                    _fullSlots.Add(GetSlotList()[i]);

                    _iHaveACharacter = true;
                }
            }
        }

        private void CalculateIslandState(int emptyCount, int fullCount)
        {
            int totalCount = GetSlotList().Count;

            if (emptyCount == totalCount)
            {
                islandState = IslandState.Empty;
                _iHaveACharacter = false;
            }

            else if (fullCount == totalCount / _quarter)
            {
                islandState = IslandState.Quarter;
            }

            else if (fullCount == totalCount / _half)
            {
                islandState = IslandState.Half;
            }

            else if (fullCount == (totalCount / _quarter) * _quarterHalf)
            {
                islandState = IslandState.QuarterHalf;
            }

            else if (fullCount == totalCount)
            {
                islandState = IslandState.Full;
            }
        }

        public void CalculateIslandFullSlots()
        {
            CalculateFullSlot();

            CalculateIslandState(_emptyCount, _fullCount);

            if (IsTargetIslandFullAndSameColor())
            {
                IsCompletedIsland(true);
            }
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

        public Vector3 GetEmptySlotPosition(HumanController childObjectController)
        {
            Vector3 vectorHolder = Vector3.zero;

            _iHaveACharacter = true;

            for (int i = 0; i < GetSlotList().Count; i++)
            {
                if (GetSlotList()[i].GetSlotState() == SlotState.Empty)
                {
                    GetSlotList()[i].SetParentForHuman(childObjectController.gameObject);
                    GetSlotList()[i].SetSlotState(SlotState.Full);
                    GetSlotList()[i].SetSlotColor(childObjectController.GetColorType());

                    _fullSlots.Add(GetSlotList()[i]);

                    Vector3 offset = new Vector3(0, (this.transform.localScale.y / _half) + _humanPositionYOffset, 0);

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
            if (_fullSlots.Count != 0)
            {
                foreach (var human in movingHumans)
                {
                    if (human.GetColorType() != _fullSlots[_fullSlots.Count - 1].GetSlotColorType())
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
            _movableObjects.Clear();

            for (int i = _fullSlots.Count - 1; i >= 0; i--)
            {
                if (_fullSlots[i].GetSlotColorType() == _fullSlots[_fullSlots.Count - 1].GetSlotColorType())
                {
                    _movableObjects.Add(_fullSlots[i].GetActiveObjectController());
                }

                else
                {
                    break;
                }
            }

            _movableObjects.Reverse();
        }

        public async void ChooseCharactersForMovement(List<Vector3> pathList, IslandController targetIsland)
        {
            int SlotListCount = GetSlotList().Count;

            CalculateMovableObjects();

            if (targetIsland.HasEnoughEmptySlots(_movableObjects.Count) && targetIsland.HasSameColorGroup(_movableObjects) && !_isCompleted)
            {
                for (int k = 0; k < pathList.Count; k++)
                {
                    pathList[k] += new Vector3(0, _pathYOffset, 0);
                }

                PathSignals.Instance.onSetIsSelectable?.Invoke(false);

                for (int i = _movableObjects.Count - 1; i >= 0; i--)
                {
                    var movableObject = _movableObjects[i];

                    var lastPosition = targetIsland.GetEmptySlotPosition(movableObject);

                    pathList.Add(lastPosition);

                    movableObject.SetAnimation(HumanAnimation.Run);

                    bool isClearPath = i == 0;

                    movableObject.transform.DOPath(pathList.ToArray(), _doPathDuration, PathType.Linear).OnWaypointChange(index =>
                    {
                        if (index < pathList.Count - 1)
                        {
                            Vector3 direction = (pathList[index + 1] - pathList[index]).normalized;

                            Quaternion rotation = Quaternion.LookRotation(-direction, Vector3.up);

                            movableObject.transform.DORotateQuaternion(rotation, _doRotateQuarternionDuration);
                        }
                    }).OnComplete(() =>
                    {
                        movableObject.SetAnimation(HumanAnimation.Idle);
                        pathList.Remove(lastPosition);
                        movableObject.transform.DORotate(new Vector3(0, 90, 0), _doRotateDuration,RotateMode.LocalAxisAdd);

                        if (isClearPath)
                        {
                            PathSignals.Instance.onClearPath?.Invoke();

                            PathSignals.Instance.onSetIsSelectable?.Invoke(true);

                            if (targetIsland.GetIslandState() == IslandState.Full)
                            {
                                targetIsland.PlayConfettiParticle();
                            }
                        }

                    });

                    await Task.Delay(TimeSpan.FromSeconds(_durationBetweenMovableObjects));

                    RemoveHumanFromSourceIsland(movableObject.gameObject);

                    targetIsland.CalculateIslandFullSlots();
                }

                this.SetInitializeIslandState();
            }
            else
            {
                PathSignals.Instance.onClearPath?.Invoke();
            }
        }

        public IslandState GetIslandState()
        {
            return islandState;
        }

        public void IsCompletedIsland(bool state)
        {
            _isCompleted = state;
        }

        private void RemoveHumanFromSourceIsland(GameObject human)
        {
            foreach (var slot in GetSlotList())
            {
                if (slot.GetSlotState() == SlotState.Full)
                {
                    if (slot.GetCharacteObject() == human)
                    {
                        slot.SetSlotState(SlotState.Empty);
                        break; 
                    }
                }
            }
        }

        public bool IsFirstObject(bool isFirst = false)
        {
            return _isFirstObject;
        }

        public IslandController GetIslandController()
        {
            return this;
        }

        public bool IsTargetIslandFullAndSameColor()
        {
            if (islandState == IslandState.Full)
            {
                if (_fullSlots.Count > 0)
                {
                    CharacterColor targetColor = _fullSlots[0].GetSlotColorType();

                    foreach (var slot in _fullSlots)
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

                if ((k + 1) % 4 == 0) 
                {
                    t = (t + 1) % GetSlotList().Count;
                }
            }

        }
    }
}