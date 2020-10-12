using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    private static Game _game;
    public static Game _
    {
        get
        {
            return _game;
        }
    }

    public LevelController LevelController;
    public EnergyController EnergyController;
    public Snake Snake;
    [HideInInspector]
    public bool RestartLevel;
    private string LevelToLoad;

    void Awake()
    {
        _game = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void Init()
    {
        LevelController.Init();
    }

    public void Restart()
    {
        RestartLevel = true;
        LevelToLoad = LevelController.LevelName;
        SceneManager.LoadScene("Loading");
    }

    public void LoadWaitedLevel()
    {
        if (RestartLevel)
        {
            RestartLevel = false;
            SceneManager.LoadScene(LevelToLoad);
            return;
        }
    }
}
