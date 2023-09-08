using Runtime.GridModule.Abstraction;
using System.Collections.Generic;
using UnityEngine;

public enum SlotState
{
    Empty,
    Full,
}

public enum CharacterColor
{
    Blue,
    Green,
    Red,
}

public class Slot : MonoBehaviour, ISlotObject
{
    [SerializeField]
    private SlotState slotState;

    [SerializeField]
    private CharacterColor characterColor;

    [SerializeField]
    private List<GameObject> characters = new List<GameObject>();

    private void Awake()
    {
        SetCharacters();
    }

    private void SetCharacters()
    {
        characters.CloseAllListElements();

        if (slotState == SlotState.Full)
        {
            characters[(int)characterColor].SetActive(true);
        }
    }

    public void SnapObject(GameObject obj)
    {
        slotState = SlotState.Full;
    }

    public void Reset()
    {
        slotState = SlotState.Empty;
    }

    public SlotState GetSlotState()
    {
        return slotState;
    }
}
