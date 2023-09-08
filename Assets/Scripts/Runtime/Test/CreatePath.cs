using DG.Tweening;
using Runtime.Test;
using System.Collections.Generic;
using UnityEngine;

public class CreatePath : MonoBehaviour
{
    [SerializeField]
    private Material pathMaterial;

    [SerializeField]
    private List<GameObject> objectsToMove = new List<GameObject>();

    [SerializeField]
    private GameObject cubePrefab;

    private ClickableObject firstSelectedObject;

    private List<Vector3> pathPositions = new List<Vector3>();
    private List<Vector3> objectInitialPositions = new List<Vector3>();

    [SerializeField]
    private List<GameObject> _createdPathList = new List<GameObject>();

    [SerializeField]
    private List<ClickableObject> _clickedObjectList = new List<ClickableObject>();

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                ClearPathMeshes();

                ClickableObject clickedObject = hit.collider.GetComponent<ClickableObject>();

                _clickedObjectList.Add(clickedObject);

                if (clickedObject != null)
                {
                    if (firstSelectedObject == null)
                    {

                        firstSelectedObject = clickedObject;
                        clickedObject.IsFirstObject(true);
                    }
                    else
                    {
                        // Calculate and store the path positions
                        CalculatePath(firstSelectedObject.GetPathTargets(), clickedObject.GetPathTargets()); ;

                        // Generate and display the path mesh

                        SetTargetObjectActiveIsland(true);

                        GeneratePathObject();

                        // Move objects along the path
                        //MoveObjectsAlongPath(firstSelectedObject, clickedObject);

                        firstSelectedObject = null;
                    }
                }
            }
        }
    }

    private void SetTargetObjectActiveIsland(bool isActive)
    {
        for (int i = 0; i < _clickedObjectList.Count; i++)
        {
            _clickedObjectList[i].SetTargetObjectActive(isActive);
        }

        if (!isActive)
        {
            _clickedObjectList.Clear();
            _clickedObjectList.TrimExcess();
        }

    }

    private void CalculatePath(Transform[] transformsFirstClicked, Transform[] transformsSecondClicked)
    {
        pathPositions.Clear();

        for (int i = 0; i < transformsFirstClicked.Length; i++)
        {
            pathPositions.Add(transformsFirstClicked[i].position);
        }

        for (int i = 0; i < transformsSecondClicked.Length; i++)
        {
            pathPositions.Add(transformsSecondClicked[i].position);
        }
    }

    private void GeneratePathObject()
    {
        // Calculate the midpoint between the two points
        Vector3 midpoint = (pathPositions[1] + pathPositions[3]) / 2f;

        // Calculate the size of the cube or plane based on the distance between the points
        float distance = Vector3.Distance(pathPositions[1], pathPositions[3]);
        float diffDistance = firstSelectedObject.transform.localScale.x;
        Vector3 size = new Vector3(distance, 0.1f, 0.5f); // Adjust the size as needed

        // Calculate the rotation to align the object with the direction between the points
        Vector3 direction = pathPositions[3] - pathPositions[1];
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(0, -90, 0); ;

        // Instantiate the cube or plane at the midpoint with the correct size and rotation
        GameObject pathObject;

        if (cubePrefab != null)
        {
            pathObject = Instantiate(cubePrefab, midpoint, rotation);
            pathObject.transform.localScale = size;
            _createdPathList.Add(pathObject);
        }
    }

    private void ClearPathMeshes()
    {
        if (_clickedObjectList.Count >= 2)
        {
            for (int i = 0; i < _createdPathList.Count; i++)
            {
                Destroy(_createdPathList[i].gameObject);
            }

            _createdPathList.Clear();
            _createdPathList.TrimExcess();

            SetTargetObjectActiveIsland(false);
        }
    }

    private void MoveObjectsAlongPath()
    {
        // Store initial positions of objects and move them along the path
        foreach (GameObject obj in objectsToMove)
        {
            objectInitialPositions.Add(obj.transform.position);
            obj.transform.DOPath(pathPositions.ToArray(), 2f, PathType.Linear).OnComplete(()=>
            {
                //ClearPathMeshes();
            });
        }
    }
}
