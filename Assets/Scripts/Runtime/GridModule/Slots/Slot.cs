using UnityEngine;

public enum SlotState
{
    Empty,
    Full,
}

public class Slot : MonoBehaviour
{
    [SerializeField]
    private SlotState slotState;

    [SerializeField]
    private Transform weaponTarget;

    public SlotState GetSloteState()
    {
        return slotState;
    }

    public void SnapObject(GameObject obj)
    {
        obj.transform.SetParent(weaponTarget);
        obj.transform.localPosition = Vector3.zero;
        slotState = SlotState.Full;
    }

    public void Reset()
    {
        slotState = SlotState.Empty;
    }
}
