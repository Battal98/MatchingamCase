using Extentions;
using System;
using UnityEngine;
using UnityEngine.Events;

public class GridSignals : MonoSingleton<GridSignals>
{
    public Func<GridManager, int> onGetGridIndex = delegate { return 0; };

    public UnityAction<Vector2> onRequestPosition = delegate { }; 
    public UnityAction<Vector3> onResponsePosition = delegate { };

    public UnityAction<Vector2, GameObject> onSetIslandPathPosition = delegate { };

}
