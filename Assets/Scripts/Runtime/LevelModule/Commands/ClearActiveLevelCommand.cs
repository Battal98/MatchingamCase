using Runtime.Abstactions.Commands;
using UnityEngine;

namespace Runtime.LevelModule.Commands
{
    public class ClearActiveLevelCommand : Command
    {
        #region Self Variables

        #region Private Variables

        private GameObject _levelholder;

        #endregion

        #endregion

        public ClearActiveLevelCommand(GameObject levelHolder)
            : base(() =>
            {
                Object.Destroy(levelHolder.transform.GetChild(0).gameObject);
            })
        {
        }
    }
}
