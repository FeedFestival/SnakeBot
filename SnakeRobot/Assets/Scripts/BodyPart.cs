using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public MoveDirection MoveDirection;
    public GameObject KeepDirection;
    public GameObject CurveRight;
    public GameObject CurveLeft;
    public Ondulation Ondulation;
    public Move PreviousMove;

    public void SetOndulation(Ondulation ondulation)
    {
        Ondulation = ondulation;
        switch (Ondulation)
        {
            case Ondulation.Right:
                CurveRight.SetActive(true);
                CurveLeft.SetActive(false);
                KeepDirection.SetActive(false);
                break;
            case Ondulation.Left:
                CurveRight.SetActive(false);
                CurveLeft.SetActive(true);
                KeepDirection.SetActive(false);
                break;
            default:
                CurveRight.SetActive(false);
                CurveLeft.SetActive(false);
                KeepDirection.SetActive(true);
                break;
        }
    }
}

public enum Ondulation
{
    Keep,
    Right,
    Left
}
