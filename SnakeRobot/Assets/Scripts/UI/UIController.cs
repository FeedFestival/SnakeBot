using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private static UIController _uIController;
    public static UIController _ { get { return _uIController; } }
    void Awake()
    {
        _uIController = this;
    }

    public MovesController MovesController;
    public GameObject LeftSideButtons;
    public GameObject RightSideButtons;
    public GameObject ConsumablesAndCoins;
    public DialogController DialogController;
    public UiDataController UiDataController;
    public Image Overlay;

    public void Init()
    {
        DialogController.gameObject.SetActive(true);
        DialogController.ShowDialog(false);
        TransitionOverlay(show: false, instant: true);
    }

    public void ShowGridMoves(bool value)
    {
        MovesController.gameObject.SetActive(value);
    }

    public void ShowAllActions(bool value)
    {
        LeftSideButtons.SetActive(value);
        RightSideButtons.SetActive(value);
        ShowGridMoves(value);
    }

    public void TransitionOverlay(bool show = true, bool instant = false)
    {
        if (instant)
        {
            Overlay.gameObject.SetActive(show);
        }
    }
}
