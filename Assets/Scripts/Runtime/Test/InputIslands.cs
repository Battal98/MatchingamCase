using DG.Tweening;
using Runtime.Abstactions;
using Runtime.Test;
using System.Collections.Generic;
using UnityEngine;

public class InputIslands : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> objectsToMove = new List<GameObject>();

    private IslandController firstSelectedObject;

    [SerializeField]
    private List<IslandController> _clickedObjectList = new List<IslandController>();

    [SerializeField]
    private TestPathfinder pathfinder;

    private Vector2 _startPosition;
    private Vector2 _endPosition;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                hit.collider.TryGetComponent(out IInteractable interactable);

                _clickedObjectList.Add(interactable.GetIslandController());

                if (interactable.GetIslandController() != null)
                {
                    if (firstSelectedObject == null)
                    {

                        firstSelectedObject = interactable.GetIslandController();

                        interactable.GetIslandController().IsFirstObject(true);

                        _startPosition = interactable.GetPathPosition();
                    }
                    else
                    {
                        // Move objects along the path
                        //MoveObjectsAlongPath(firstSelectedObject, clickedObject);
                        _endPosition = interactable.GetPathPosition();

                        pathfinder.PathFind(_startPosition, _endPosition);

                        firstSelectedObject = null;

                        _clickedObjectList.Clear();
                    }
                }
            }
        }
    }

    private void MoveObjectsAlongPath()
    {
        // Store initial positions of objects and move them along the path
        foreach (GameObject obj in objectsToMove)
        {
            /*objectInitialPositions.Add(obj.transform.position);
            obj.transform.DOPath(pathPositions.ToArray(), 2f, PathType.Linear).OnComplete(()=>
            {
            });*/
        }
    }
}
