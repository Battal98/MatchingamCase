using Extentions;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Pathfind
{
    public class PathSignals : MonoSingleton<PathSignals>
    {
        public UnityAction onClearPath = delegate { };
    }
}