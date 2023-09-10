using System;
using Extentions;
using UnityEngine.Events;
using UnityEngine;
using Enums;

namespace CoreGameModule.Signals
{
    public class CoreGameSignals : MonoSingleton<CoreGameSignals>
    {
        public UnityAction<GameStates> onChangeGameState = delegate { };
        public UnityAction<Transform> onSetCameraTarget = delegate { };
        public UnityAction onReset = delegate { };
        public UnityAction onPlay = delegate { };
    } 
}
