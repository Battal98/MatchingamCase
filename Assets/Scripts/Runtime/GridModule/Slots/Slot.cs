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
    private CharacterColor slotColor;  

    [SerializeField]
    private List<HumanController> characters = new List<HumanController>();

    private HumanController _activeController;

    private void CloseAllCharacterGameobjects()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].gameObject.SetActive(false);
        }
    }

    public HumanController GetActiveObjectController()
    {
        return _activeController;
    }

    public void SetParentForHuman(GameObject obj)
    {
        characters.Clear();

        obj.transform.SetParent(this.transform);

        var objComponent = obj.GetComponent<HumanController>();

        characters.Add(objComponent);

        _activeController = objComponent;
    }

    public GameObject GetCharacteObject()
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

    public CharacterColor GetSlotColorType()
    {
        return slotColor;
    }

    public void SetSlotColor(CharacterColor colorType)
    {
        slotColor = colorType;
    }

    public void SetCharacterColorInitialize(CharacterColor colorType)
    {
        CloseAllCharacterGameobjects();

        slotState = SlotState.Full;

        characters[(int)colorType].SetHumanActivity(true);

        _activeController = characters[(int)colorType];

        SetSlotColor(colorType);
    }
}
