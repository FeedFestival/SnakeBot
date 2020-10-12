using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public BodyPart SnakeHead;
    public Vector2Int SnakeHeadPos;
    public int SnakeHeadIndex;
    public bool SnakeIsOnEndCube;
    public Transform SnakeBodyContainer;
    public List<BodyPart> BodyParts;
    public BodyPart SnakeTail;
    private List<Move> _newBodyPartEntries;

    public void Init(MapCube toMapCube)
    {
        SnakeHead.gameObject.SetActive(true);
        PlaceSnake(toMapCube);

        BodyParts[0].gameObject.SetActive(false);
        SnakeHeadIndex = 0;
        SnakeTail.gameObject.SetActive(false);
    }

    private void PlaceSnake(MapCube toMapCube)
    {
        Game._.Snake.SnakeHead.transform.position = toMapCube.transform.position;
        // Game._.Snake.SnakeHeadPos = new Vector2Int(toMapCube.Y, toMapCube.X);
        Game._.Snake.SnakeHeadPos = toMapCube.Pos;
    }

    public void MoveSnake(Move move)
    {
        SnakeIsOnEndCube = move.ToMapCube.IsEnd;
        Apple theApple = Game._.LevelController.MapMaker.Apples
            .FirstOrDefault(a => a.Pos.x == move.ToMapCube.Pos.x && a.Pos.y == move.ToMapCube.Pos.y);
        MoveCopy moveCopy = MoveBodyPart(move, SnakeHead);
        Game._.EnergyController.UseEnergy();

        foreach (var bodyPart in BodyParts)
        {
            if (moveCopy != null)
            {
                if (bodyPart.gameObject.activeSelf == false)
                {
                    bodyPart.gameObject.SetActive(true);
                }
                moveCopy = CopyMove(moveCopy, bodyPart);
            }
        }

        if (theApple != null)
        {
            theApple.Consume();
            Game._.EnergyController.ConsumeApple();
            AddBodyPart();

            if (SnakeHeadIndex > 1)
            {
                if (BodyParts[BodyParts.Count - 1].gameObject.activeSelf == false)
                {
                    BodyParts[BodyParts.Count - 1].gameObject.SetActive(true);
                }
                moveCopy = CopyMove(moveCopy, BodyParts[BodyParts.Count - 1], isAddingNewBodyPart: true);
            }
        }

        bool canWeSeeTail = !(SnakeHeadIndex == 0);
        if (canWeSeeTail == false)
        {
            return;
        }

        if (moveCopy != null)
        {
            if (SnakeTail.gameObject.activeSelf == false)
            {
                SnakeTail.gameObject.SetActive(true);
            }

            if (_newBodyPartEntries != null)
            {
                _newBodyPartEntries.RemoveAt(_newBodyPartEntries.Count - 1);
                if (_newBodyPartEntries.Count == 0)
                {
                    _newBodyPartEntries = null;
                }
                return;
            }
            CopyMove(moveCopy, SnakeTail);
        }
    }

    private void AddBodyPart()
    {
        GameObject go = Instantiate(PrefabBank._.SnakeBodyPart, new Vector3(0, 0, 0), Quaternion.identity);
        go.transform.SetParent(SnakeBodyContainer);
        go.transform.localPosition = BodyParts[0].transform.localPosition;
        var bodyPart = go.GetComponent<BodyPart>();
        bodyPart.gameObject.SetActive(false);
        BodyParts.Add(bodyPart);
    }

    private MoveCopy MoveBodyPart(Move move, BodyPart bodyPart)
    {
        MoveCopy moveCopy = new MoveCopy();

        moveCopy.Move = move;
        moveCopy.Position = bodyPart.transform.position;
        bodyPart.transform.position = move.ToMapCube.transform.position;

        //  new Vector2Int(move.ToMapCube.Y, move.ToMapCube.X);
        SnakeHeadPos = move.ToMapCube.Pos;

        moveCopy.MoveDirection = bodyPart.MoveDirection;
        bodyPart.MoveDirection = GetHeadDirection(move);

        moveCopy.Ondulation = bodyPart.Ondulation;
        switch (bodyPart.MoveDirection)
        {
            case MoveDirection.Forward:
                bodyPart.transform.eulerAngles = new Vector3(0, 0, 0);
                if (move.NextY == 1)
                {
                    bodyPart.SetOndulation(Ondulation.Keep);
                }
                else if (move.NextX == 1)
                {
                    bodyPart.SetOndulation(Ondulation.Right);
                }
                else
                {
                    bodyPart.SetOndulation(Ondulation.Left);
                }
                break;
            case MoveDirection.Left:
                bodyPart.transform.eulerAngles = new Vector3(0, -90, 0);
                if (move.NextX == -1)
                {
                    bodyPart.SetOndulation(Ondulation.Keep);
                }
                else if (move.NextY == 1)
                {
                    bodyPart.SetOndulation(Ondulation.Right);
                }
                else
                {
                    bodyPart.SetOndulation(Ondulation.Left);
                }
                break;
            case MoveDirection.Right:
                bodyPart.transform.eulerAngles = new Vector3(0, 90, 0);
                if (move.NextX == 1)
                {
                    bodyPart.SetOndulation(Ondulation.Keep);
                }
                else if (move.NextY == 1)
                {
                    bodyPart.SetOndulation(Ondulation.Left);
                }
                else
                {
                    bodyPart.SetOndulation(Ondulation.Right);
                }
                break;
            default:    // Back
                bodyPart.transform.eulerAngles = new Vector3(0, 180, 0);
                if (move.NextY == -1)
                {
                    bodyPart.SetOndulation(Ondulation.Keep);
                }
                else if (move.NextX == 1)
                {
                    bodyPart.SetOndulation(Ondulation.Left);
                }
                else
                {
                    bodyPart.SetOndulation(Ondulation.Right);
                }
                break;
        }
        return moveCopy;
        // Debug.Log(bodyPart.MoveDirection.ToString() + " - to " + bodyPart.Ondulation.ToString());
    }

    private MoveCopy CopyMove(MoveCopy moveCopy, BodyPart bodyPart, bool isAddingNewBodyPart = false)
    {
        MoveCopy returnMoveCopy = null;
        if (isAddingNewBodyPart == false)
        {
            returnMoveCopy = new MoveCopy();
        }

        if (isAddingNewBodyPart == false)
        {
            returnMoveCopy.Position = bodyPart.transform.position;
        }
        bodyPart.transform.position = moveCopy.Position;

        if (isAddingNewBodyPart == false)
        {
            returnMoveCopy.MoveDirection = bodyPart.MoveDirection;
        }
        bodyPart.MoveDirection = moveCopy.MoveDirection;
        switch (bodyPart.MoveDirection)
        {
            case MoveDirection.Forward:
                bodyPart.transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case MoveDirection.Left:
                bodyPart.transform.eulerAngles = new Vector3(0, -90, 0);
                break;
            case MoveDirection.Right:
                bodyPart.transform.eulerAngles = new Vector3(0, 90, 0);
                break;
            default:    // Back
                bodyPart.transform.eulerAngles = new Vector3(0, 180, 0);
                break;
        }

        if (isAddingNewBodyPart == false)
        {
            returnMoveCopy.Ondulation = bodyPart.Ondulation;
        }
        bodyPart.SetOndulation(moveCopy.Ondulation);

        if (isAddingNewBodyPart == false)
        {
            return returnMoveCopy;
        }

        if (_newBodyPartEntries == null)
        {
            _newBodyPartEntries = new List<Move>();
        }
        _newBodyPartEntries.Add(moveCopy.Move);
        return moveCopy;
    }

    private MoveDirection GetHeadDirection(Move move)
    {
        if (move.X == 0 && move.Y == 1)
        {
            return MoveDirection.Forward;
        }
        else if (move.X == -1 && move.Y == 0)
        {
            return MoveDirection.Left;
        }
        else if (move.X == 1 && move.Y == 0)
        {
            return MoveDirection.Right;
        }
        else
        {
            return MoveDirection.Back;
        }
    }
}

public class Move
{
    public int Y;
    public int X;
    public int NextY;
    public int NextX;
    public MapCube ToMapCube;
    public MapCube NextMapCube;
    public PartOfBody PartOfBody;

    public Move(Vector2Int to, Vector2Int next)
    {
        Y = to.y;
        X = to.x;
        if (next != null)
        {
            NextY = next.y;
            NextX = next.x;
        }
    }

    public bool SameCoordinate(Move move)
    {
        return (move.X == X) && (move.Y == Y);
    }
}

public class MoveCopy
{
    public Vector3 Position;
    public Move Move;
    public MoveDirection MoveDirection;
    public Ondulation Ondulation;
}

public enum PartOfBody
{
    Head,
    PartOfBody,
    Tail
}
