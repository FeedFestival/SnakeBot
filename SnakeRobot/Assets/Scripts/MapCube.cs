using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCube : MonoBehaviour
{
    public Vector2Int Pos;
    public TextMesh TextMesh;
    public TextMesh IndexTextMesh;
    public TextMesh EnergyTextMesh;
    public bool IsEnd;
    private string _indexQue;
    public GameObject Obj;

    public void Init(Vector2Int pos, bool debugThis)
    {
        Pos = pos;
        if (debugThis)
        {
            TextMesh.text = Pos.y + "" + Pos.x;
        }
        else
        {
            TextMesh.gameObject.SetActive(false);
        }
        gameObject.name = TextMesh.text;
        IndexTextMesh.gameObject.SetActive(false);
        _indexQue = "";
        EnergyTextMesh.gameObject.SetActive(false);
    }

    public void ShowIndex(int index)
    {
        IndexTextMesh.gameObject.SetActive(true);
        _indexQue += (string.IsNullOrEmpty(_indexQue) ? "" : "-") + index;
        IndexTextMesh.text = _indexQue;
    }

    public void ShowEnergy(int energy)
    {
        EnergyTextMesh.gameObject.SetActive(true);
        EnergyTextMesh.text = energy.ToString();
    }

    public void ChangeColor(PathColor pathColor)
    {
        switch (pathColor)
        {
            case PathColor.Pink:
                TextMesh.color = HiddenSettings._.PinkColor;
                break;
            case PathColor.Red:
                TextMesh.color = HiddenSettings._.RedColor;
                break;
            case PathColor.Blue:
                TextMesh.color = HiddenSettings._.BlueColor;
                break;
            case PathColor.Yellow:
                TextMesh.color = HiddenSettings._.YellowColor;
                break;
            case PathColor.LightBlue:
                TextMesh.color = HiddenSettings._.LightBlueColor;
                break;
            default:
                TextMesh.color = HiddenSettings._.GreyColor;
                break;
        }
    }

    public void ShowObj(bool value)
    {
        Obj.SetActive(false);
    }

    public override string ToString()
    {
        return Pos.y + ", " + Pos.x + (IsEnd ? " - End" : "");
    }
}

public enum PathColor
{
    Grey,
    Pink,
    Red,
    Blue,
    Yellow,
    LightBlue
}