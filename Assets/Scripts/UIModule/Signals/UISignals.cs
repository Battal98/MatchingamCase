using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extentions;
using UnityEngine.Events;
using UIModules.Enums;

namespace UIModules.Signals
{
    public class UISignals : MonoSingleton<UISignals>
    {
        public UnityAction<PanelTypes> onOpenPanel = delegate { };
        public UnityAction<PanelTypes> onClosePanel = delegate { };
        public UnityAction<int> onUpdateMoneyScore = delegate { };
        public UnityAction<int> onUpdateGemScore = delegate { };

    }
}
