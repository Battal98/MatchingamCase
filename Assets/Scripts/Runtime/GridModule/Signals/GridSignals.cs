using Extentions;
using System;
using UnityEngine;

public class GridSignals : MonoSingleton<GridSignals>
{
    public Func<GridManager, int> onGetGridIndex = delegate { return 0; };

    public Action<Vector2> RequestPosition; 
    public Action<Vector3> ResponsePosition; 
}
