using UnityEngine;

namespace Runtime.Test
{
    public class ClickableObject : MonoBehaviour
    {
        [SerializeField]
        private Transform[] pathTargets;

        [SerializeField]
        private GameObject targetObject;

        [SerializeField]
        private bool isFirstObject = false;

        private void OnEnable()
        {
            SetTargetObjectActive(false);
        }

        private void OnMouseDown()
        {
            Debug.Log("clicked me : " + this.gameObject.name);
        }

        public bool IsFirstObject(bool isFirst = false)
        {
            return isFirstObject;
        }

        public Transform[] GetPathTargets()
        {
            return pathTargets;
        }

        public void SetTargetObjectActive(bool isActive)
        {
            targetObject.SetActive(isActive);
        }
    }
}