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

    private Vector2 _startPosition;
    private Vector2 _endPosition;

    private IslandController _firstSelectedObject;
    private IslandController _secondSelectedObject;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if(!hit.collider.TryGetComponent(out IInteractable interactable))
                {
                    return;
                }

                if (interactable.GetIslandController() != null)
                {
                    _clickedObjectList.Add(interactable.GetIslandController());

                    if (_firstSelectedObject == null)
                    {
                        if (!interactable.IsInteractable())
                        {
                            _clickedObjectList.Clear();
                            return;
                        }

                        _secondSelectedObject = null;

                        PathSignals.Instance.onClearPath?.Invoke();

                        _firstSelectedObject = interactable.GetIslandController();

                        _firstSelectedObject.IsFirstObject(true);

                        _startPosition = _firstSelectedObject.GetPathPosition();
                    }
                    else
                    {
                        _secondSelectedObject = interactable.GetIslandController();
                        if (_firstSelectedObject == _secondSelectedObject)
                        {
                            _firstSelectedObject = null;
                            _secondSelectedObject = null;
                            return;
                        }
                        _endPosition = _secondSelectedObject.GetPathPosition();

                        pathfinder.PathFind(_startPosition, _endPosition);

                        _firstSelectedObject.ChooseCharactersForMovement(pathList: pathfinder.GetPathPositionList(), _secondSelectedObject);

                        _firstSelectedObject = null;

                        _clickedObjectList.Clear();
                    }
                }
            }
            else
            {
                _firstSelectedObject = null;
                _secondSelectedObject = null;

                _clickedObjectList.Clear();

                PathSignals.Instance.onClearPath?.Invoke();
            }
        }
    }
}
