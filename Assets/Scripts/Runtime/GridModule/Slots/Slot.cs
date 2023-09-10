using Runtime.GridModule.Abstraction;
using Runtime.IslandModule.Controller;
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
    private List<HumanController> characters = new List<HumanController>();

    private void CloseAllCharacterGameobjects()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].gameObject.SetActive(false);
        }
    }

    public void SetParentForHuman(GameObject obj)
    {
        obj.transform.SetParent(this.transform);
    }

    public GameObject GetCharacterList()
    {
        int k = 0;

        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].gameObject.activeInHierarchy)
            {
                k = i;
                break;
            }
        };

        return characters[k].gameObject; ;
    }

    public void SnapObject()
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

    public void SetSlotState(SlotState state)
    {
        slotState = state;
    }

    public void SetCharacterColor(CharacterColor colorType)
    {
        CloseAllCharacterGameobjects();

        slotState = SlotState.Full;

        characters[(int)colorType].SetHumanActivity(true);
    }
}
