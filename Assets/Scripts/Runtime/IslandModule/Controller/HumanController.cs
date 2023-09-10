using System.Collections;
using UnityEngine;

namespace Runtime.IslandModule.Controller
{
    public enum HumanAnimation
    {
        Idle, Run,
    }

    public class HumanController : MonoBehaviour
    {
        [SerializeField]
        private CharacterColor characterColor;

        [SerializeField]
        private Animator animator;

        public void SetHumanActivity(bool isActive)
        {
            this.gameObject.SetActive(isActive);
        }

        public CharacterColor GetColorType()
        {
            return characterColor;
        }

        public void SetAnimation(HumanAnimation animationType)
        {
            string animationTypeToString = animationType.ToString();
            animator.SetTrigger(animationTypeToString);
        }
    }
}