using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum GridSurface
{
    XY,
    XZ,
    YZ,
}

public enum GridType
{
    Rect,
    Circ,
}
public class GridManager : MonoBehaviour
{
    #region Serializable Variables

    #region Rectangular Variables

    [SerializeField] private GridType gridType;
    [ShowIf("gridType", GridType.Rect)]
    [SerializeField] private GridSurface gridSurface;
    [ShowIf("gridType", GridType.Rect)]
    [SerializeField] private Vector2 gridSize;
    [ShowIf("gridType", GridType.Rect)]
    [SerializeField] private Vector2 gridOffsets;
    [ShowIf("gridType", GridType.Rect)]
    [SerializeField] private int direction;
    [ShowIf("gridType", GridType.Rect)]
    [SerializeField] private bool isRotateObject;

    #endregion

    #region Circular Variables

    [ShowIf("gridType", GridType.Circ)]
    [SerializeField] int gridCount; // Grid sayýsý
    [ShowIf("gridType", GridType.Circ)]
    [SerializeField] float radius; // Dairenin yarýçapý
    [ShowIf("gridType", GridType.Circ)]
    [SerializeField] int heightCount; // Yükseklik sayýsý
    [ShowIf("gridType", GridType.Circ)]
    [SerializeField] float height; // Gridlerin yüksekliði 

    #endregion

    #region Common Variables

    [SerializeField] private GameObject gridObject;
    [SerializeField] private Transform gridPivotTarget;

    #endregion 

    #endregion

    #region Rect Const 

    private const int XYRot = 0;
    private const int XZRot = 90;
    private const int YZRot = 90;

    #endregion

    [SerializeField]
    private List<Slot> slotList = new List<Slot>();

    #region Subs Jobs

    private void OnEnable()
    {
        SubscribeEvents(true);
    }

    private void SubscribeEvents(bool isSub)
    {
        if (isSub)
        {
           
        }
        else
        {
            
        }
    }

    private void OnDisable()
    {
        SubscribeEvents(false);
    }

    #endregion

    private bool ValidateGridSize()
    {
        return gridSize.x > 0 && gridSize.y > 0;
    }

    private bool ValidateGridCount()
    {
        return gridCount > 0;
    }

    private Vector3 CalculatePosition(Vector3 pivot, float modX, float modY, float modZ)
    {
        return new Vector3(
            modX * gridOffsets.x + pivot.x,
            modY * gridOffsets.y + pivot.y,
            modZ * gridOffsets.y + pivot.z // Assuming this is meant to be gridOffsets.y based on original code
        );
    }

    private void CreateAndSetupGridObject(Vector3 position, Quaternion rotation)
    {
        var obj = Instantiate(gridObject);
        var objComponent = obj.GetComponent<Slot>();
        obj.transform.SetParent(this.transform);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        slotList.Add(objComponent);
    }

    #region Create Grid Factories

    private void CreateCircularGrid()
    {
        if (!ValidateGridCount()) return;

        float angleOffset = 360f / gridCount; // Gridler arasýndaki açýsal offset
        float heightStep = height / heightCount; // Yükseklikteki adým miktarý

        for (int i = 0; i < gridCount; i++)
        {
            // Gridin pozisyonunu hesapla
            float angle = i * angleOffset;
            float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = radius * Mathf.Sin(angle * Mathf.Deg2Rad);

            for (int j = 0; j < heightCount; j++)
            {
                float y = height * j + heightStep * 0.5f;
                // Grid'i oluþtur ve pozisyonunu ayarla
                GameObject grid = Instantiate(gridObject, transform);
                var gridComponent = grid.GetComponent<Slot>();
                slotList.Add(gridComponent);
                grid.transform.position = new Vector3(x, y, z);

                // Grid'in rotasyonunu merkeze doðru ayarla
                Vector3 targetPosition = new Vector3(0f, y, 0f);
                Vector3 direction = targetPosition - grid.transform.position;
                if (direction != Vector3.zero)
                {
                    Quaternion rotation = Quaternion.LookRotation(direction);
                    grid.transform.rotation = rotation;
                }
            }
        }
    }

    private void CreateRectangularGrid()
    {
        if (!ValidateGridSize()) return;

        int gridCount = (int)(gridSize.x * gridSize.y);

        // Pivot calculation is not shown in the provided code, assuming it remains the same
        // gridPivotCalculate = CheckPivotPosition(gridCount);

        for (int i = 0; i < gridCount; i++)
        {
            var modX = i % (int)gridSize.x;
            var divide = i / (int)gridSize.x;
            var modY = divide % (int)gridSize.y;
            Vector3 position;
            Quaternion rotation;

            switch (gridSurface)
            {
                case GridSurface.XY:
                    position = CalculatePosition(gridPivotTarget.position, direction * modX, modY, 0);
                    rotation = Quaternion.Euler(0, 0, XYRot);
                    break;

                case GridSurface.XZ:
                    position = CalculatePosition(gridPivotTarget.position, direction * modX, 0 + 0.02f, modY);
                    rotation = Quaternion.Euler(XZRot, 0, 0);
                    break;

                case GridSurface.YZ:
                    position = CalculatePosition(gridPivotTarget.position, 0, direction * modX, modY);
                    rotation = Quaternion.Euler(0, YZRot, 0);
                    break;

                default:
                    throw new ArgumentException("Invalid grid surface.");
            }

            if (!isRotateObject)
            {
                rotation = Quaternion.identity;
            }

            CreateAndSetupGridObject(position, rotation);
        }
    }

    #endregion

    #region For Editor to Inspector
#if UNITY_EDITOR

    /// <summary>
    /// This Attribute is create grid in Editor and see how is lookslike 
    /// </summary>
    /// 

    [Button]

    private void OnCreateGrid()
    {
        ClearAllGrid();
        switch (gridType)
        {
            case GridType.Rect:
                CreateRectangularGrid();
                break;
            case GridType.Circ:
                CreateCircularGrid();
                break;
        }
    }

    [Button]
    private void ClearAllGrid()
    {
        while (transform.childCount > 0)
        {
            GameObject grid = transform.GetChild(0).gameObject;
            DestroyImmediate(grid);
        }
        slotList.Clear();
        slotList.TrimExcess();
    }

#endif 
    #endregion
}