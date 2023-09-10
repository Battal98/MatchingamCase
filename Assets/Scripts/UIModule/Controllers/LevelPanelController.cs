using TMPro;

namespace UIModules.Controllers
{
    public class LevelPanelController
    {

        #region Self Variables

        #region Private Variables

        private readonly TextMeshProUGUI _levelText;

        #endregion

        #endregion

        public LevelPanelController(TextMeshProUGUI levelText)
        {
            _levelText = levelText;
        }

        public void SetLevelText(int levelValue)
        {
            _levelText.text = "LEVEL " + levelValue.ToString();
        }
    }
}
