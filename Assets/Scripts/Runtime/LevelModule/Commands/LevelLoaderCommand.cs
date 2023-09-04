using Runtime.Abstactions.Commands;
using System;
using UnityEngine;

namespace Runtime.LevelModule.Commands
{
    public class LevelLoaderCommand : Command
    {
        public LevelLoaderCommand(GameObject levelHolder, Func<int> getLevelID)
            : base(() =>
            {
                int currentLevelID = getLevelID();
                UnityEngine.Object obj = UnityEngine.Object.Instantiate(Resources.Load<GameObject>($"LevelPrefabs/Level {currentLevelID + 1}"),
                    levelHolder.transform);

            })
        {
        }
    } 
}
