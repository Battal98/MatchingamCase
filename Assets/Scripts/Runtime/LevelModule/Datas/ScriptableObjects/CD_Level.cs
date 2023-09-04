using System.Collections.Generic;
using UnityEngine;

namespace Runtime.LevelModule.Datas
{
    [CreateAssetMenu(menuName = "Datas", fileName = "CD_Level", order = 0)]
    public class CD_Level : ScriptableObject
    {
        public LevelInfo LevelInfo;
        public List<GameLevelData> GameLevelDatas = new List<GameLevelData>();
    }
}