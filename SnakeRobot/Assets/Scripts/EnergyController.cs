using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyController : MonoBehaviour
{
    public int Energy;
    public int Apples;

    public void Init()
    {
        Apples = 0;
        UIController._.UiDataController.UpdateText(Apples, UiDataType.Apple);
        Energy = HiddenSettings._.StartingEnergy;
        UIController._.UiDataController.UpdateText(Energy, UiDataType.Energy);
    }

    public void UseEnergy()
    {
        Energy--;
        UIController._.UiDataController.UpdateText(Energy, UiDataType.Energy);
        if (Energy == 0)
        {
            Game._.LevelController.ChangeGameState(GameplayState.OutOfEnergy);
        }
    }

    public void ConsumeApple()
    {
        Apples++;
        UIController._.UiDataController.UpdateText(Apples, UiDataType.Apple);
        Energy += HiddenSettings._.AppleEnergy;
        UIController._.UiDataController.UpdateText(Energy, UiDataType.Energy);
    }
}
