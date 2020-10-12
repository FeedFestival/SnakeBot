using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenSettings : MonoBehaviour
{
    static private HiddenSettings _hiddenSettings;
    static public HiddenSettings _
    {
        get
        {
            return _hiddenSettings;
        }
    }

    void Awake()
    {
        _hiddenSettings = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public GameObject PrefabBankPrefab;
    public int StartingEnergy;
    public int AppleEnergy;
    public Color RedColor;
    public Color PinkColor;
    public Color GreyColor;
    public Color BlueColor;
    public Color YellowColor;
    public Color LightBlueColor;
}
