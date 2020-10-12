using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiDataController : MonoBehaviour
{
    public Text EnergyText;
    public Text AppleText;

    public void UpdateText(int value, UiDataType uiDataType)
    {
        switch (uiDataType)
        {
            case UiDataType.Energy:
                EnergyText.text = value.ToString();
                break;
            case UiDataType.Apple:
                AppleText.text = value.ToString();
                break;
            default:
                break;
        }
    }
}

public enum UiDataType
{
    Energy, Apple
}
