using Extentions;
using System;
using UnityEngine.Events;

namespace Runtime.LevelModule.Signals
{
    public class LevelSignals : MonoSingleton<LevelSignals>
    {
        public UnityAction onLevelInitialize = delegate { };
        public UnityAction onLevelInitializeDone = delegate { };
        public UnityAction onClearActiveLevel = delegate { };
        public UnityAction onLevelLoad = delegate { };
        public UnityAction onLevelFailed = delegate { };
        public UnityAction onLevelSuccessful = delegate { };
        public UnityAction onNextLevel = delegate { };
        public UnityAction onRestartLevel = delegate { };
        public UnityAction onCreatePositionHandlerObjects = delegate { };

        public Func<int> onGetLevel = delegate { return 0; };
        public Func<int> onGetLevelForText = delegate { return 0; };
    }
}