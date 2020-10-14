using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            UIController._.MovesController.AddMove(MoveDirection.Left);
        }
        else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
        {
            UIController._.MovesController.AddMove(MoveDirection.Forward);
        }
        else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            UIController._.MovesController.AddMove(MoveDirection.Right);
        }
        else if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            UIController._.MovesController.AddMove(MoveDirection.Back);
        }

        if (Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Return))
        {
            Game._.LevelController.PlaySnake();
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            Game._.LevelController.MapMaker.CreateApple();
        }

        if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetKeyUp(KeyCode.Keypad1))
        {
            Game._.LevelController.MapMaker.CreateStartingCube();
            Game._.LevelController.MapMaker.CreateEndCube();
        }
        if (Input.GetKeyUp(KeyCode.Alpha2) || Input.GetKeyUp(KeyCode.Keypad2))
        {
            Game._.LevelController.MapMaker.CreateHoles();
        }
        if (Input.GetKeyUp(KeyCode.Alpha3) || Input.GetKeyUp(KeyCode.Keypad3))
        {
            Game._.LevelController.MapMaker.CreateApples();
        }
        if (Input.GetKeyUp(KeyCode.Alpha4) || Input.GetKeyUp(KeyCode.Keypad4))
        {
            Game._.LevelController.StartGame();
        }
    }
}
