using System.Collections;
using UnityEngine;

namespace Runtime.IslandModule.Controller
{
    public class HumanController : MonoBehaviour
    {
        [SerializeField]
        private CharacterColor characterColor;

        public void SetHumanActivity(bool isActive)
        {
            this.gameObject.SetActive(isActive);
        }
    }
}