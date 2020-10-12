using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    public GameObject GameDialog;
    public Text Title;
    public Text Info;
    public GameObject RetryButton;
    public GameObject ContinueButton;

    public void ShowDialog(bool value, GameplayState gameplayState = GameplayState.Planning)
    {
        GameDialog.SetActive(value);

        if (value)
        {
            switch (gameplayState)
            {
                case GameplayState.Finished:
                    Title.text = "Congrats!";
                    Info.text = "You've completed the level!";
                    RetryButton.SetActive(false);
                    ContinueButton.SetActive(true);
                    break;
                case GameplayState.FallenOver:
                    Title.text = "Too bad!";
                    Info.text = "Snake fell over...";
                    RetryButton.SetActive(true);
                    ContinueButton.SetActive(false);
                    break;
                case GameplayState.OutOfEnergy:
                default:
                    Title.text = "Almost there!";
                    Info.text = "Ran out of Energy...";
                    RetryButton.SetActive(true);
                    ContinueButton.SetActive(false);
                    break;
            }
        }
    }

    public void OnContinue()
    {
        Game._.Restart();
    }

    public void OnRestart()
    {
        Game._.Restart();
    }
}
