using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovesController : MonoBehaviour
{
    public RectTransform MovesGrid;
    [SerializeField]
    public MoveDirection CurrentMoveDirection;
    [SerializeField]
    public List<MoveDirection> _moveDirections;
    public List<Vector2Int> MoveDirections;

    public Button ForwardButton;
    public Button BackButton;
    public Button LeftButton;
    public Button RightButton;

    public bool TestMoveDirection;

    // Start is called before the first frame update
    void Start()
    {
        _moveDirections = new List<MoveDirection>();
        MoveDirections = new List<Vector2Int>();

        if (TestMoveDirection)
        {
            AddMove(MoveDirection.Forward);
            AddMove(MoveDirection.Forward);
            AddMove(MoveDirection.Left);
            AddMove(MoveDirection.Forward);
            AddMove(MoveDirection.Forward);
            AddMove(MoveDirection.Forward);
            AddMove(MoveDirection.Forward);
            AddMove(MoveDirection.Right);
            AddMove(MoveDirection.Right);
            AddMove(MoveDirection.Right);
            AddMove(MoveDirection.Back);
            AddMove(MoveDirection.Back);
            AddMove(MoveDirection.Right);
            AddMove(MoveDirection.Back);
            AddMove(MoveDirection.Back);
            AddMove(MoveDirection.Left);
            AddMove(MoveDirection.Left);
            AddMove(MoveDirection.Forward);
            AddMove(MoveDirection.Forward);
            AddMove(MoveDirection.Right);
            AddMove(MoveDirection.Forward);
            AddMove(MoveDirection.Forward);
            AddMove(MoveDirection.Right);
            AddMove(MoveDirection.Forward);
            AddMove(MoveDirection.Forward);
        }
    }

    public void AddMove(int moveDir)
    {
        AddMove((MoveDirection)moveDir);
    }

    public void AddMove(MoveDirection moveDirection)
    {
        if (IsDirectionLocked(moveDirection))
        {
            return;
        }

        CreateMoveDirectionGO(moveDirection);

        _moveDirections.Add(moveDirection);
        MoveDirections.Add(GetDirectionValues(_moveDirections[_moveDirections.Count - 1]));

        LockOppositeDirection(moveDirection);
    }

    private void CreateMoveDirectionGO(MoveDirection moveDirection)
    {
        GameObject prefabRef;
        switch (moveDirection)
        {
            case MoveDirection.Forward:
                prefabRef = PrefabBank._.ForwardArrow;
                break;
            case MoveDirection.Back:
                prefabRef = PrefabBank._.BackArrow;
                break;
            case MoveDirection.Left:
                prefabRef = PrefabBank._.LeftArrow;
                break;
            default:
                prefabRef = PrefabBank._.RightArrow;
                break;
        }
        var go = Instantiate(prefabRef, new Vector3(0, 0, 0), Quaternion.identity);
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(MovesGrid);
        rt.localScale = Vector3.one;
        rt.sizeDelta = Vector3.one;
        rt.localEulerAngles = Vector3.zero;
    }

    public Vector2Int GetDirectionValues(MoveDirection moveDirection)
    {
        var values = new int[2];
        switch (moveDirection)
        {
            case MoveDirection.Forward:
                return new Vector2Int(1, 0);
            case MoveDirection.Back:
                return new Vector2Int(-1, 0);
            case MoveDirection.Left:
                return new Vector2Int(0, -1);
            default:
                return new Vector2Int(0, 1);
        }
    }

    private void LockOppositeDirection(MoveDirection moveDirection)
    {
        ForwardButton.interactable = true;
        BackButton.interactable = true;
        LeftButton.interactable = true;
        RightButton.interactable = true;
        switch (moveDirection)
        {
            case MoveDirection.Forward:
                BackButton.interactable = false;
                break;
            case MoveDirection.Back:
                ForwardButton.interactable = false;
                break;
            case MoveDirection.Left:
                RightButton.interactable = false;
                break;
            default:
                LeftButton.interactable = false;
                break;
        }
    }

    private bool IsDirectionLocked(MoveDirection moveDirection)
    {
        switch (moveDirection)
        {
            case MoveDirection.Forward:
                return !ForwardButton.interactable;
            case MoveDirection.Back:
                return !BackButton.interactable;
            case MoveDirection.Left:
                return !LeftButton.interactable;
            default:
                return !RightButton.interactable;
        }
    }
}

public enum MoveDirection
{
    Forward, Back, Left, Right
}
