using DG.Tweening;
using Runtime.Abstactions;
using Runtime.Pathfind;
using Runtime.PathfindModule;
using System.Collections.Generic;
using UnityEngine;

public class InputIslands : MonoBehaviour
{
    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private List<IslandController> _clickedObjectList = new List<IslandController>();

    [SerializeField]
    private Pathfinder pathfinder;

    [SerializeField]
    private float offsetY = 0.25f;
    [SerializeField]
    private float duration = 0.5f;

    private Vector2 _startPosition;
    private Vector2 _endPosition;

    private Vector3 _initialPosFirst = Vector3.zero;

    private IslandController _firstSelectedObject;
    private IslandController _secondSelectedObject;

    private bool _isSelectable = true;

    private void OnEnable()
    {
        PathSignals.Instance.onSetIsSelectable += OnSetIsSelected;
    }

    private void OnDisable()
    {
        PathSignals.Instance.onSetIsSelectable += OnSetIsSelected;
    }

    private void Update()
    {
        if (!_isSelectable)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            HandleObjectClick(hit.collider.gameObject);
        }
        else
        {
            ClearSelection();
        }
    }

    private void HandleObjectClick(GameObject clickedObject)
    {
        if (!clickedObject.TryGetComponent(out IInteractable interactable))
        {
            return;
        }

        IslandController islandController = interactable.GetIslandController();

        if (islandController != null)
        {
            _clickedObjectList.Add(islandController);

            if (_firstSelectedObject == null)
            {
                if (!interactable.IsInteractable())
                {
                    PathSignals.Instance.onClearPath?.Invoke();
                    return;
                }

                _secondSelectedObject = null;

                PathSignals.Instance.onClearPath?.Invoke();

                _firstSelectedObject = islandController;

                _firstSelectedObject.IsFirstObject(true);

                _startPosition = _firstSelectedObject.GetPathPosition();

                _initialPosFirst = _firstSelectedObject.gameObject.transform.position;

                float endY1 = _initialPosFirst.y + offsetY;

                _firstSelectedObject.transform.DOMoveY(endY1, duration);
            }
            else
            {
                _secondSelectedObject = islandController;

                if (_firstSelectedObject == _secondSelectedObject)
                {
                    PathSignals.Instance.onClearPath?.Invoke();
                    return;
                }

                _endPosition = _secondSelectedObject.GetPathPosition();

                ResetIslandPositions();

                pathfinder.PathFind(_startPosition, _endPosition);

                _firstSelectedObject.ChooseCharactersForMovement(pathList: pathfinder.GetPathPositionList(), _secondSelectedObject);
            }
        }
        else
        {
            PathSignals.Instance.onClearPath?.Invoke();
        }
    }

    public void ResetIslandPositions()
    {
        if (_firstSelectedObject != null)
        {
            _firstSelectedObject.transform.DOMoveY(_initialPosFirst.y, duration);
        }
    }  

    public void ClearSelection()
    {
        ResetIslandPositions();

        _initialPosFirst = Vector3.zero;

        _firstSelectedObject = null;
        _secondSelectedObject = null;

        _clickedObjectList.Clear();
    }

    private void OnSetIsSelected(bool isSelected)
    {
        _isSelectable = isSelected;
    }
}
