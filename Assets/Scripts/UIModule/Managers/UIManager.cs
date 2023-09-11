using UnityEngine;
using UIModules.Enums;
using Enums;
using UIModules.Controllers;
using System.Collections.Generic;
using TMPro;
using CoreGameModule.Signals;
using UIModules.Signals;
using Runtime.LevelModule.Signals;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UIModules.Managers
{
    public class UIManager : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField]
        private List<GameObject> panels;   
        [SerializeField]
        private TextMeshProUGUI levelText;

        [SerializeField]
        private Volume volume;

        #endregion

        #region Private Variables

        private UIPanelControllers _uiPanelController;
        private LevelPanelController _levelPanelController;

        #endregion

        #endregion

        private void Awake()
        {
            _uiPanelController = new UIPanelControllers(panels);
            _levelPanelController = new LevelPanelController(levelText);
        }

        #region Event Subscriptions

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            UISignals.Instance.onOpenPanel += OnOpenPanel;
            UISignals.Instance.onClosePanel += OnClosePanel;

            CoreGameSignals.Instance.onPlay += OnPlay;
            CoreGameSignals.Instance.onReset += OnReset;

            LevelSignals.Instance.onLevelInitialize += OnLevelInitialize;
            LevelSignals.Instance.onLevelFailed += OnLevelFailed;
            LevelSignals.Instance.onLevelSuccessful += OnLevelSuccessful;
            
        }

        private void UnsubscribeEvents()
        {
            UISignals.Instance.onOpenPanel -= OnOpenPanel;
            UISignals.Instance.onClosePanel -= OnClosePanel;

            CoreGameSignals.Instance.onPlay -= OnPlay;
            CoreGameSignals.Instance.onReset -= OnReset;

            LevelSignals.Instance.onLevelInitialize -= OnLevelInitialize;
            LevelSignals.Instance.onLevelFailed -= OnLevelFailed;
            LevelSignals.Instance.onLevelSuccessful -= OnLevelSuccessful;
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion

        private void OnOpenPanel(PanelTypes panelParam)
        {
            _uiPanelController.OpenPanel(panelParam);
        }

        private void OnClosePanel(PanelTypes panelParam)
        {
            _uiPanelController.ClosePanel(panelParam);
        }

        private void InitPanels()
        {
            _uiPanelController.CloseAllPanel();
            _uiPanelController.OpenPanel(PanelTypes.LevelPanel);
            _uiPanelController.OpenPanel(PanelTypes.StartPanel);
            volume.enabled = false;
        }

        private void OnReset()
        {
            InitPanels();
        }

        private void OnPlay()
        {
            _uiPanelController.CloseAllPanel();
            _uiPanelController.OpenPanel(PanelTypes.LevelPanel);
        }

        private void OnLevelFailed()
        {
            _uiPanelController.CloseAllPanel();
            _uiPanelController.OpenPanel(PanelTypes.FailedPanel);
        }

        private void OnLevelSuccessful()
        {
            _uiPanelController.CloseAllPanel();
            _uiPanelController.OpenPanel(PanelTypes.WinPanel);
            volume.enabled = true;
        }

        private void OnLevelInitialize()
        {
            InitPanels();
            _levelPanelController.SetLevelText(LevelSignals.Instance.onGetLevelForText.Invoke() + 1);
        }

        public void NextLevelButton()
        {
            OnLevelInitialize();
            LevelSignals.Instance.onNextLevel?.Invoke();
        }

        public void RestartButton()
        {
            _uiPanelController.CloseAllPanel();
            _uiPanelController.OpenPanel(PanelTypes.LevelPanel);
            LevelSignals.Instance.onRestartLevel?.Invoke();
            CoreGameSignals.Instance.onPlay?.Invoke();
        }
    } 
}
