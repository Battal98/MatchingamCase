using Extentions;
using System;

public class GridSignals : MonoSingleton<GridSignals>
{
    public Func<GridManager, int> onGetGridIndex = delegate { return 0; };
}
