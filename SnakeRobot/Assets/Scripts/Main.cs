using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public bool IsThisTheLoadingScene;
    public GameObject HiddenSettingsPrefab;

    [Header("GOs")]
    public LevelController LevelController;
    public EnergyController EnergyController;

    private Game _game;
    private IEnumerator _waitForLevelLoad;

    void Awake()
    {
        GameObject go;
        var settings = FindObjectOfType<HiddenSettings>();
        if (settings == null)
        {
            go = Instantiate(HiddenSettingsPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            settings = go.GetComponent<HiddenSettings>();
        }

        var prefabBank = FindObjectOfType<PrefabBank>();
        if (prefabBank == null)
        {
            go = Instantiate(settings.PrefabBankPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            prefabBank = go.GetComponent<PrefabBank>();
        }

        _game = FindObjectOfType<Game>();
        if (_game == null)
        {
            go = Instantiate(prefabBank.GamePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            _game = go.GetComponent<Game>();
        }

        if (IsThisTheLoadingScene == false)
        {
            if (_game.LevelController == null)
            {
                _game.LevelController = LevelController;
            }

            var snake = FindObjectOfType<Snake>();
            if (_game.Snake == null)
            {
                _game.Snake = snake;
            }
        }
    }

    void Start()
    {
        if (IsThisTheLoadingScene)
        {
            _waitForLevelLoad = WaitForLevelLoad();
            StartCoroutine(_waitForLevelLoad);
        }
        else
        {
            _game.Init();
            UIController._.Init();
            EnergyController.Init();
            _game.EnergyController = EnergyController;
        }
    }

    private IEnumerator WaitForLevelLoad()
    {
        yield return new WaitForSeconds(_game != null && _game.RestartLevel ? 0.5f : 2f);

        _game.LoadWaitedLevel();
        StopCoroutine(_waitForLevelLoad);
        _waitForLevelLoad = null;
    }
}
