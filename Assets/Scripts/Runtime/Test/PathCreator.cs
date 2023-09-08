#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    public GameObject islandPrefab; // The island prefab with a scale of 2-1-2.
    public GameObject pathPrefab;   // Prefab for the path segments.
    public float pathOffset = 0.2f; // Offset to avoid islands.

    [SerializeField]
    private List<Transform> selectedIslands = new List<Transform>();
    [SerializeField]
    private List<Transform> pathIslands = new List<Transform>();

    private List<GameObject> pathLines = new List<GameObject>();

    private LineRenderer pathRenderer;
    private Vector3[] pathPoints;

    void Start()
    {
        pathRenderer = pathPrefab.GetComponent<LineRenderer>();
        pathRenderer.positionCount = 0;
    }

    void Update()
    {
        // Detect mouse clicks on islands.
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Transform selectedIsland = hit.transform;
                if (selectedIslands.Count == 0)
                {
                    ClearPath();
                    selectedIslands.Add(selectedIsland);
                }
                else if (selectedIslands.Count == 1)
                {
                    CreatePath(selectedIslands[0], selectedIsland);
                    selectedIslands.Clear();
                }
            }
        }
    }

    private void ClearPath()
    {
        for (int i = 0; i < pathLines.Count; i++)
        {
            Destroy(pathLines[i].gameObject);
        }

        pathLines.Clear();
        pathLines.TrimExcess();
    }

    void CreatePath(Transform startIsland, Transform endIsland)
    {
        Vector3 startPoint = startIsland.position;
        Vector3 endPoint = endIsland.position;

        // Calculate the path with an offset to avoid islands.
        Vector3[] waypoints = CalculatePathWithOffset(startPoint, endPoint);

        // Create a new GameObject for the path.
        GameObject newPath = Instantiate(pathPrefab);
        pathLines.Add(newPath);
        LineRenderer newLineRenderer = newPath.GetComponent<LineRenderer>();

        // Set the path's visual representation.
        newLineRenderer.positionCount = waypoints.Length;
        newLineRenderer.SetPositions(waypoints);

        // Optionally, you can handle other gameplay logic here.
    }

    List<Vector3> AStarPathfinding(Vector3 start, Vector3 end)
    {
        List<Vector3> waypoints = new List<Vector3>();

        // Create a grid-based representation of your game world.
        // Define grid cell size and resolution based on your game's needs.
        // Create a grid of nodes (representing walkable areas and obstacles).
        // Initialize the grid nodes based on your game's terrain.
        // Walkable cells should be marked as such, and obstacles should be avoided.

        // Implement the A* algorithm to find the shortest path.
        // You'll need to use open and closed lists, calculate node costs (G, H, F),
        // and navigate through the grid to find the path.

        // For demonstration purposes, let's add the start and end points directly.
        // Replace this with your A* logic.
        Vector3 currentNode = start;
        waypoints.Add(currentNode);

        while (currentNode != end)
        {
            // Perform A* logic here to find the next node in the path.
            // Update currentNode accordingly.

            waypoints.Add(currentNode);
        }

        return waypoints;
    }

    Vector3[] CalculatePathWithOffset(Vector3 start, Vector3 end)
    {
        // Calculate the path points with an offset.
        List<Vector3> waypoints = new List<Vector3>();
        waypoints.Add(start);

        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);

        // Check for islands along the path.
        RaycastHit[] hits = Physics.RaycastAll(start, direction, distance);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Island"))
            {
                waypoints.Add(hit.point + hit.normal * pathOffset);
            }
        }

        waypoints.Add(end);
        return waypoints.ToArray();
    }
}
#endif