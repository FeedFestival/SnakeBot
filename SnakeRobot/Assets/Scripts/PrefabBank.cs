using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabBank : MonoBehaviour
{
    private static PrefabBank _prefabBank;
    public static PrefabBank _ { get { return _prefabBank; } }

    public GameObject GamePrefab;
    public GameObject GreenCube;
    public GameObject Apple;
    public GameObject SnakeBodyPart;
    public GameObject TreeLog;
    public GameObject ForwardArrow;
    public GameObject BackArrow;
    public GameObject LeftArrow;
    public GameObject RightArrow;

    void Awake()
    {
        _prefabBank = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
