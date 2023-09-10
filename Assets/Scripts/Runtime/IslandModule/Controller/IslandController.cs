using DG.Tweening;
using NaughtyAttributes;
using Runtime.Abstactions;
using Runtime.GridModule.Slots;
using Runtime.IslandModule.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
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

        private bool _isIslandFull = false;
        private bool _iHaveACharacter = false;

        private int _maxGridSize;

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
            SetInıtializeCharacters();

            SetInıtializeIslandState();
        }

        private void SetInıtializeIslandState()
        {
            if (GetSlotList().Count > _maxGridSize)
            {
                int emptyCount = 0;
                int fullCount = 0;

                for (int i = 0; i < GetSlotList().Count; i++)
                {
                    if (GetSlotList()[i].GetSlotState() == SlotState.Empty)
                    {
                        emptyCount++;
                    }
                    else if (GetSlotList()[i].GetSlotState() == SlotState.Full)
                    {
                        fullCount++;
                    }
                }

                int totalCount = GetSlotList().Count;

                if (emptyCount == totalCount)
                {
                    islandState = IslandState.Empty;
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
                }
            }
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

                    Vector3 offset = new Vector3(0, (this.transform.localScale.y/2f) - 0.2f, 0);

                    vectorHolder = GetSlotList()[i].gameObject.transform.position + offset;

                    break;
                }
            }
            return vectorHolder;
        }

        public async void ChooseCharactersForMovement(List<Vector3> pathList, IslandController targetIsland)
        {
            int SlotListCount = GetSlotList().Count;

            for (int i = 0; i < SlotListCount; i++)
            {
                //objectInitialPositions.Add(obj.transform.position);
                var obj = GetSlotList()[i].GetCharacterList();

                var lastPosition = targetIsland.GetEmptySlotPosition(obj);

                pathList.Add(lastPosition);

                obj.transform.DOPath(pathList.ToArray(), 2f, PathType.Linear).OnComplete(() =>
                {
                    obj.transform.DORotate(new Vector3(0,180,0),0.2f, RotateMode.LocalAxisAdd);
                });

                await Task.Delay(TimeSpan.FromSeconds(0.15f));
            }

            for (int k = SlotListCount - 1; k >= 0; k--)
            {
                GetSlotList().Remove(GetSlotList()[k]);
                GetSlotList().TrimExcess();
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

        public void SetInıtializeCharacters()
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

                GetSlotList()[k].SetCharacterColor(initializeCharacterDatas[t].ColorType);

                if ((k + 1) % 4 == 0) // Check if we've assigned 4 colors, then move to the next color
                {
                    t = (t + 1) % GetSlotList().Count; // Use modular arithmetic to cycle through colorList
                }
            }

        }
    }
}