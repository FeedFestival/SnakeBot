using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelController : MonoBehaviour
{
    public bool DebugThis;
    public string LevelName;
    [SerializeField]
    public GameplayState GameplayState;
    public bool RandomLevel;
    public int X;   // max 10
    public int Y;   // max 10
    public int ApplesCount;
    public int StartAtX;
    public int EndAtX;
    public MapCube[,] MapCubes;
    private IEnumerator _playCo;
    public MapMaker MapMaker;

    public void Init()
    {
        if (RandomLevel)
        {
            MapMaker.RandomizeLevelValues(DebugThis);
        }
        else
        {
            MapMaker.InitValues(DebugThis);
        }
        MapCubes = MapMaker.CreateMap();
        MapMaker.CreateStartingCube();
        MapMaker.CreateEndCube();
        MapMaker.CreateHoles();
        MapMaker.CreateApples();

        if (Game._.Snake.gameObject.activeSelf == false)
        {
            Game._.Snake.gameObject.SetActive(true);
        }
        Game._.Snake.Init(MapMaker.MapCubeStart);
    }

    private MapCube GetToWhatCubeToMove(Vector2Int to)
    {
        var x = Game._.Snake.SnakeHeadPos.x + to.x;
        var y = Game._.Snake.SnakeHeadPos.y + to.y;
        if (y == Y || x == X || y < 0 || x < 0)
        {
            if (Game._.Snake.SnakeIsOnEndCube)
            {
                ChangeGameState(GameplayState.Finished);
                return null;
            }
            else
            {
                ChangeGameState(GameplayState.FallenOver);
                return null;
            }
        }
        return MapCubes[y, x];
    }

    private bool MoveSnake(Vector2Int to, Vector2Int next)
    {
        var bodyPartMove = new Move(to, next);
        bodyPartMove.ToMapCube = GetToWhatCubeToMove(to);

        if (bodyPartMove.ToMapCube == null)
        {
            return false;
        }

        if (next != null)
        {
            var futureY = Game._.Snake.SnakeHeadPos.y + to.y;
            var futureX = Game._.Snake.SnakeHeadPos.x + to.x;
            if (futureY == (Y - 1) || futureX == (X - 1)
                || futureY == 0 || futureX == 0)
            {
                // End of the Line
            }
            else
            {
                var nextY = futureY + next.y;
                var nextX = futureX + next.x;
                bodyPartMove.NextMapCube = MapCubes[nextY, nextX];
            }
        }
        Game._.Snake.MoveSnake(bodyPartMove);
        return true;
    }

    public bool PlaySnake()
    {
        if (UIController._.MovesController.MoveDirections.Count == Game._.Snake.SnakeHeadIndex)
        {
            if (Game._.Snake.SnakeIsOnEndCube)
            {
                ChangeGameState(GameplayState.Finished);
                return false;
            }
            ChangeGameState(GameplayState.OutOfEnergy);
            return false;
        }

        Vector2Int moveDirection = UIController._.MovesController.MoveDirections[Game._.Snake.SnakeHeadIndex];
        var nextIndex = Game._.Snake.SnakeHeadIndex + 1;
        Vector2Int nextMoveDirection;
        bool didSnakeMove = false;
        if (UIController._.MovesController.MoveDirections.Count > nextIndex)
        {
            nextMoveDirection = UIController._.MovesController.MoveDirections[nextIndex];
            didSnakeMove = MoveSnake(moveDirection, nextMoveDirection);
        }

        Game._.Snake.SnakeHeadIndex++;
        return didSnakeMove;
    }

    public void PlaySnakeContinuous()
    {
        if (_playCo != null)
        {
            StopCoroutine(_playCo);
            _playCo = null;
        }
        else
        {
            // StartedPlaying
            ChangeGameState(GameplayState.DuringPlay);
        }
        _playCo = PlaySnakeCo();
        StartCoroutine(_playCo);
    }

    IEnumerator PlaySnakeCo()
    {
        yield return new WaitForSeconds(0.4f);
        var continuePlaying = PlaySnake();
        if (continuePlaying)
        {
            PlaySnakeContinuous();
        }
        else
        {
            StopCoroutine(_playCo);
            _playCo = null;
        }
    }

    public void ChangeGameState(GameplayState gameplayState)
    {
        GameplayState = gameplayState;
        switch (gameplayState)
        {
            case GameplayState.DuringPlay:
                // Debug.Log("During Play: You can't issue any more actions");
                UIController._.ShowAllActions(false);
                break;
            case GameplayState.Finished:
            case GameplayState.FallenOver:
            case GameplayState.OutOfEnergy:
                UIController._.DialogController.ShowDialog(true, gameplayState);
                break;
            default:
                Debug.Log("Starting Level...");
                break;
        }
    }

    public void Restart()
    {
        Game._.Restart();
    }
}

public enum GameplayState
{
    Starting, Planning, DuringPlay, Finished, FallenOver, OutOfEnergy
}
