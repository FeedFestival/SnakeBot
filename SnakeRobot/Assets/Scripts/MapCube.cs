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
    public bool IsWalkable;

    public void Init(Vector2Int pos, bool debugThis)
    {
        Pos = pos;
        if (debugThis)
        {
            TextMesh.text = Pos.x + "" + Pos.y;
        }
        else
        {
            TextMesh.gameObject.SetActive(false);
        }
        gameObject.name = TextMesh.text;
        IndexTextMesh.gameObject.SetActive(false);
        _indexQue = "";
        EnergyTextMesh.gameObject.SetActive(false);
        IsWalkable = true;
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

    public void SetWalkable(bool value)
    {
        Obj.SetActive(value);
        IsWalkable = value;
    }

    public override string ToString()
    {
        return Pos.x + ", " + Pos.y + (IsEnd ? " - End" : "");
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