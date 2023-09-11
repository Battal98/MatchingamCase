using NaughtyAttributes;
using Runtime.LevelModule.Commands;
using Runtime.LevelModule.Datas;
using Runtime.LevelModule.Signals;
using System;
using UnityEngine;

namespace Runtime.LevelModule
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject levelHolder;

        private LevelLoaderCommand _levelLoader;

        private ClearActiveLevelCommand _clearActiveLevel;

        private int _levelID;
        private int _levelIDforText;

        [SerializeField]
        private CD_Level _cD_Level;
        private LevelInfo _levelInfo;
        private GameLevelData _gameLevelData;

        private const string RESOURCES_DATA_PATH = "Datas/CD_Level";
        private const string LOAD_FILENAME = "LevelData";
        private const string LOAD_FOLDERNAME = "Level";

        private void Start()
        {
            InitializeManager();

            LevelSignals.Instance.onLevelInitialize?.Invoke();
        }

        #region Subscription Jobs

        private void OnEnable()
        {
            SubscribeEvents(true);
        }

        private void SubscribeEvents(bool subscribe)
        {
            if (subscribe)
            {
                LevelSignals.Instance.onLevelInitialize += OnInitializeLevel;
                LevelSignals.Instance.onClearActiveLevel += OnClearActiveLevel;

                LevelSignals.Instance.onLevelLoad += OnLoadLevel;

                LevelSignals.Instance.onNextLevel += OnNextLevel;

                LevelSignals.Instance.onSendToData += OnSenToData;

                LevelSignals.Instance.onGetLevel += OnGetLevelID;
                LevelSignals.Instance.onGetLevelForText += OnGetLevelIdForText;
            }
            else
            {
                LevelSignals.Instance.onLevelInitialize -= OnInitializeLevel;
                LevelSignals.Instance.onClearActiveLevel -= OnClearActiveLevel;

                LevelSignals.Instance.onNextLevel -= OnNextLevel;

                LevelSignals.Instance.onLevelLoad -= OnLoadLevel;

                LevelSignals.Instance.onSendToData -= OnSenToData;

                LevelSignals.Instance.onGetLevel -= OnGetLevelID;
                LevelSignals.Instance.onGetLevelForText -= OnGetLevelIdForText;
            }

        }

        private GameLevelData OnSenToData()
        {
            return _gameLevelData;
        }

        private void OnDisable()
        {
            SubscribeEvents(false);
        }

        #endregion

        private void InitializeManager()
        {
            _levelLoader = new LevelLoaderCommand(levelHolder, OnGetLevelID);
            _clearActiveLevel = new ClearActiveLevelCommand(levelHolder);
        }

        private void OnInitializeLevel()
        {
            GetLevelData();

            LevelSignals.Instance.onLevelLoad?.Invoke();

            LevelSignals.Instance.onLevelInitializeDone?.Invoke();
        }

        private int OnGetLevelID()
        {
            return _levelID;
        }

        private int OnGetLevelIdForText()
        {
            return _levelIDforText;
        }

        private void OnNextLevel()
        {
            _levelID++;
            _levelIDforText++;

            if (_levelID >= _cD_Level.GameLevelDatas.Count)
            {
                _levelID = 0;
            }

            LevelSignals.Instance.onClearActiveLevel?.Invoke();

            LevelSignals.Instance.onLevelLoad?.Invoke();
        }

        #region Save Load Level Data

        private LevelInfo LoadLevelData()
        {
            return _levelInfo.LoadDataFromFile<LevelInfo>(folderName: LOAD_FOLDERNAME, fileName: LOAD_FILENAME);
        }

        private void SetLocalVariablesFromData()
        {
            _levelID = _levelInfo.LevelID;
            _levelIDforText = _levelInfo.LevelIDForText;
        }

        private void SetDataVariablesFromLocal()
        {
            _levelInfo.LevelID = _levelID;
            _levelInfo.LevelIDForText = _levelIDforText;
        }

        private void SaveLevelData()
        {
            SetDataVariablesFromLocal();

            _levelInfo.SaveDataToFile(LOAD_FOLDERNAME, LOAD_FILENAME);
        }

        private void GetLevelData()
        {
            _levelInfo = LoadLevelData();

            _cD_Level = Resources.Load<CD_Level>(RESOURCES_DATA_PATH);

            if (_levelInfo is null)
            {
                _levelInfo = _cD_Level.LevelInfo;
            }

            SetLocalVariablesFromData();

            _gameLevelData = _cD_Level.GameLevelDatas[_levelID];

            SaveLevelData();
        }

        #endregion

        #region Load and Clear Level Object

        [Button]
        public void OnLoadLevel()
        {
            _levelLoader.Execute();
        }

        [Button]
        public void OnClearActiveLevel()
        {
            _clearActiveLevel.Execute();
        }

        #endregion

        private void OnApplicationQuit()
        {
            SaveLevelData();
        }
    }
}