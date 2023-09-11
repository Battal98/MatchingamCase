using Extentions;
using UnityEngine.Events;

namespace Runtime.Pathfind
{
    public class PathSignals : MonoSingleton<PathSignals>
    {
        public UnityAction onClearPath = delegate { };
        public UnityAction<bool> onSetIsSelectable = delegate { };
    }
}