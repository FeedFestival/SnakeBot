using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapMaker : MonoBehaviour
{
    public MapCube MapCubeStart;
    private MapCube MapCubeEnd;
    public List<Apple> Apples;
    private int _height;
    private int _width;
    private int _startAtX;
    private int _endAtX;
    private bool _debugThis;
    private int _appleIndex;
    private Pathfinding _pathfinding;
    public int EnergyTillEnd;
    [Header("Debugging")]
    public bool DebugThis;
    public bool DebugApples;
    public bool DebugPathfinding;
    private bool _isAppleCloseToEndPoint;
    private List<PathNode> _finishedPath;
    private List<PathNode> _testPath;
    private int _currentEnergy;
    private int _maxPathTry;
    private int _maxApplesTry;
    private int _applePointTry;
    private int _walkableCount;
    private List<Vector2Int> _appleCoords;
    private int _maxPointsTry;
    private float _holesRatio;
    public List<Vector2Int> _holes;

    public void InitValues(bool debugThis)
    {
        _debugThis = debugThis;
        _height = Game._.LevelController.Size.y;
        _width = Game._.LevelController.Size.x;
        _startAtX = Game._.LevelController.StartAtX;
        _endAtX = Game._.LevelController.EndAtX;
        if (_endAtX >= Game._.LevelController.Size.x)
        {
            _endAtX = Game._.LevelController.Size.x - 1;
            Game._.LevelController.EndAtX = _endAtX;
        }
        if (_startAtX >= Game._.LevelController.Size.x)
        {
            _startAtX = Game._.LevelController.Size.x - 1;
            Game._.LevelController.StartAtX = _startAtX;
        }
        _holesRatio = Game._.LevelController.HolesRatio;
        if (_holesRatio > 1.1f)
        {
            var ratio = System.Convert.ToDouble("0." + _holesRatio);
            _holesRatio = (float)ratio;
            Game._.LevelController.HolesRatio = _holesRatio;
        }
        _appleIndex = 0;
        _currentEnergy = HiddenSettings._.StartingEnergy;
    }

    public void RandomizeLevelValues(bool debugThis)
    {
        _debugThis = debugThis;
        _height = (int)Random.Range(3, 10);
        _width = (int)Random.Range(3, 10);
        Game._.LevelController.Size.y = _height;
        Game._.LevelController.Size.x = _width;
        _startAtX = (int)Random.Range(1, _width - 1);
        Game._.LevelController.StartAtX = _startAtX;
        _endAtX = (int)Random.Range(1, _width - 1);
        Game._.LevelController.EndAtX = _endAtX;
        if (_endAtX >= Game._.LevelController.Size.x)
        {
            _endAtX = Game._.LevelController.Size.x - 1;
            Game._.LevelController.EndAtX = _endAtX;
        }
        if (_startAtX >= Game._.LevelController.Size.x)
        {
            _startAtX = Game._.LevelController.Size.x - 1;
            Game._.LevelController.StartAtX = _startAtX;
        }
        _holesRatio = (int)Random.Range(20, 50);
        // _holesRatio = Game._.LevelController.HolesRatio;
        if (_holesRatio > 1.1f)
        {
            var ratio = System.Convert.ToDouble("0." + _holesRatio);
            // Debug.Log("ratio: " + ratio);
            _holesRatio = (float)ratio;
            // Debug.Log("_holesRatio: " + _holesRatio);
            Game._.LevelController.HolesRatio = _holesRatio;
        }
        _appleIndex = 0;
        _currentEnergy = HiddenSettings._.StartingEnergy;
    }

    public MapCube[,] CreateMap()
    {
        GameObject go;
        var MapCubes = new MapCube[_width, _height];
        var mcX = _width / 2 - 0.5f;
        var mcY = _height / 2 - 0.5f;
        transform.position = new Vector3(-mcX, 0, -mcY);
        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                go = Instantiate(PrefabBank._.GreenCube, new Vector3(0, 0, 0), Quaternion.identity);
                go.transform.SetParent(transform);
                go.transform.localPosition = new Vector3(x, 0, y);
                MapCubes[x, y] = go.GetComponent<MapCube>();
                MapCubes[x, y].Init(new Vector2Int(x, y), _debugThis);
            }
        }
        return MapCubes;
    }

    public void CreateHoles()
    {
        SetMapCubeWalkable(null, isWalkable: true, all: true);
        _holes = GetValidHoles();
        if (_holes == null)
        {
            return;
        }
        foreach (Vector2Int hole in _holes)
        {
            SetMapCubeWalkable(hole, isWalkable: false);
        }
    }

    public List<Vector2Int> GetValidHoles()
    {
        var holes = GenerateRandomHoles();
        if (CanCompleteLevel(holes) == false)
        {
            _maxPathTry++;
            if (_maxPathTry == HiddenSettings._.MaxPathTryCrash)
            {
                Debug.Log("Can't Create Hole. Try " + _maxPathTry);
                return null;
            }
            return GetValidHoles();
        }
        return holes;
    }

    public bool CanCompleteLevel(List<Vector2Int> holes)
    {
        _pathfinding = new Pathfinding(_width, _height, moveDiagonally: false);
        _finishedPath = new List<PathNode>();
        _testPath = new List<PathNode>();

        var endY = MapCubeEnd.Pos.y - 1;
        var startY = MapCubeStart.Pos.y + 1;

        _pathfinding.SetAllWalkable(true);
        _pathfinding.SetSomeIsWalkable(holes, false);

        List<PathNode> path = _pathfinding.FindPath(MapCubeStart.Pos.x, startY, MapCubeEnd.Pos.x, endY);

        if (path == null || path.Count == 0)
        {
            if (DebugPathfinding)
            {
                Debug.Log("No Path to End");
            }
            return false;
        }

        if (DebugPathfinding)
        {
            foreach (PathNode pNode in path)
            {
                Debug.Log("path: " + pNode.ToString());
                Game._.LevelController.MapCubes[pNode.x, pNode.y].ChangeColor(PathColor.Pink);
            }
        }
        return true;
    }

    public void CreateApples()
    {
        for (var i = 0; i < Game._.LevelController.ApplesCount; i++)
        {
            CreateApple();
            if (_isAppleCloseToEndPoint)
            {
                if (DebugPathfinding)
                {
                    Debug.Log("_finishedPath.Count: " + _finishedPath.Count);
                    int index = 0;
                    foreach (PathNode pNode in _finishedPath)
                    {
                        Game._.LevelController.MapCubes[pNode.x, pNode.y].ChangeColor(PathColor.Blue);
                        Game._.LevelController.MapCubes[pNode.x, pNode.y].ShowIndex(index);
                        index++;
                    }
                }
                break;
            }
        }
    }

    public void CreateApple()
    {
        if (Apples == null)
        {
            Apples = new List<Apple>();
        }
        Vector2Int? coord = GetValidAppleCoordinates();
        if (coord.HasValue == false)
        {
            return;
        }
        GameObject go = Instantiate(PrefabBank._.Apple, new Vector3(0, 0, 0), Quaternion.identity);
        go.transform.SetParent(transform);
        Apple apple = go.GetComponent<Apple>();
        apple.PlaceApple(coord.Value);
        apple.gameObject.name = "Apple " + coord;
        Apples.Add(apple);
        _appleIndex++;

        if (DebugThis)
        {
            Game._.LevelController.MapCubes[coord.Value.x, coord.Value.y].ShowEnergy(_currentEnergy);
        }
    }

    private Vector2Int? GetValidAppleCoordinates()
    {
        var coord = GetRandomApplePoint();

        if (coord.HasValue == false)
        {
            return null;
        }

        if (CanAddApple(coord.Value) == false)
        {
            if (_appleCoords == null)
            {
                if (DebugApples)
                {
                    Debug.Log("------------------------------------------------------------------------");
                }
                _appleCoords = new List<Vector2Int>();
                _walkableCount = _pathfinding.GetWalkableCount();
            }

            if (DebugApples)
            {
                Debug.Log("Can't add apple on " + coord.ToString());
            }
            _appleCoords.Add(coord.Value);

            _maxApplesTry++;
            var reachingMaxTries = _maxApplesTry == HiddenSettings._.MaxApplesTryCrash;
            if (reachingMaxTries || _walkableCount == _appleCoords.Count)
            {
                if (DebugApples)
                {
                    Debug.Log("Can't Place Apple. Try " + _maxApplesTry +
                        " of " + HiddenSettings._.MaxApplesTryCrash +
                        " _walkableCount: " + _walkableCount + ", _appleCoords.Count: " + _appleCoords.Count);
                }
                AddBackACube();
                return GetValidAppleCoordinates();
            }
            return GetValidAppleCoordinates();
        }

        _finishedPath.AddRange(_testPath);
        _finishedPath = _finishedPath.Distinct().ToList();

        _appleCoords = null;
        _maxApplesTry = 0;
        return coord;
    }

    private Vector2Int? GetRandomApplePoint()
    {
        bool isFirstApple = _appleIndex == 0;
        int randomX = Random.Range(0, _width - 1);
        int randomY;
        if (isFirstApple)
        {
            randomY = Random.Range(0, _height);
        }
        else
        {
            int biggestY = Apples.Max(a => a.Pos.y);
            randomY = Random.Range(biggestY, _height - 1);
        }
        var point = new Vector2Int(randomX, randomY);

        if (_appleCoords != null)
        {
            var allreadyIn = _appleCoords.Contains(point);
            if (allreadyIn)
            {
                if (DebugApples)
                {
                    Debug.Log("Apple - allready tried " + point.ToString());
                }

                _applePointTry++;
                if (_applePointTry == HiddenSettings._.ApplesPointsCrash)
                {
                    AddBackACube();
                }
                return GetRandomApplePoint();
            }
        }

        if (IsOnStartOrEnd(point))
        {
            if (DebugApples)
            {
                Debug.Log("Apple isOnStartOrEnd");
            }

            _applePointTry++;
            if (_applePointTry == HiddenSettings._.ApplesPointsCrash)
            {
                AddBackACube();
            }
            return GetRandomApplePoint();
        }

        _applePointTry = 0;
        return point;
    }

    public void AddBackACube()
    {
        var index = (int)Random.Range(0, _holes.Count - 1);
        var randomHole = _holes[index];

        SetMapCubeWalkable(randomHole, isWalkable: true);
        _pathfinding.SetIsWalkable(randomHole, true);

        _holes.RemoveAt(index);

        _appleCoords = new List<Vector2Int>();
        _walkableCount = _pathfinding.GetWalkableCount();
        _applePointTry = 0;
        _maxApplesTry = 0;
    }

    private bool CanAddApple(Vector2Int coord)
    {
        bool isAppleOnTopOfApple = (Apples.Any(a => a.Pos.x == coord.x && a.Pos.y == coord.y));
        if (isAppleOnTopOfApple)
        {
            if (DebugApples)
            {
                Debug.Log("isAppleOnTopOfApple: " + isAppleOnTopOfApple);
            }
            return false;
        }

        bool canPlaceApple = Game._.LevelController.MapCubes[coord.x, coord.y].IsWalkable;
        if (canPlaceApple)
        {
            bool isFirstApple = _appleIndex == 0;
            Vector2Int closeTo = isFirstApple
                ? new Vector2Int(MapCubeStart.Pos.x, MapCubeStart.Pos.y + 1)
                    : new Vector2Int(Apples[_appleIndex - 1].Pos.x, Apples[_appleIndex - 1].Pos.y);

            List<PathNode> path = _pathfinding.FindPath(coord.x, coord.y, closeTo.x, closeTo.y, reverse: false);

            if (path == null || path.Count == 0)
            {
                if (DebugPathfinding)
                {
                    Debug.Log(coord + " Path Not Found");
                }
                return false;
            }

            if (isFirstApple == false)
            {
                path.RemoveAt(0);
            }

            if (DebugPathfinding)
            {
                foreach (PathNode pNode in path)
                {
                    Debug.Log("path: " + pNode.ToString());
                }
            }

            int snakeLength = 3 + Apples.Count;
            int checkedCount = 1;
            var reversedPath = _finishedPath;
            reversedPath.Reverse();
            foreach (PathNode pNode in reversedPath)
            {
                if (snakeLength == checkedCount)
                {
                    break;
                }

                Debug.Log("reversedPath: " + pNode.ToString());

                if (path.Contains(pNode))
                {
                    if (DebugPathfinding)
                    {
                        Debug.Log(pNode.ToString() + " - Can't Path on the same road");
                    }
                    return false;
                }

                checkedCount++;
            }

            int energyCost = path.Count;

            if (isFirstApple)
            {
                var isAppleTooFarFromStartPoint = energyCost >= _currentEnergy;
                if (isAppleTooFarFromStartPoint)
                {
                    if (DebugApples)
                    {
                        Debug.Log(coord + " isAppleTooFarFromStartPoint: " + isAppleTooFarFromStartPoint + " Cost " + energyCost + " >= CurEng " + _currentEnergy);
                    }
                    return false;
                }
            }
            else
            {
                var isAppleTooFarFromPreviousApple = energyCost >= _currentEnergy;
                if (isAppleTooFarFromPreviousApple)
                {
                    if (DebugApples)
                    {
                        Debug.Log(coord + " isAppleTooFarFromPreviousApple: " + isAppleTooFarFromPreviousApple + " Cost " + energyCost + " >= CurEng " + _currentEnergy);
                    }
                    return false;
                }

                float _minDistance = 1.5f;
                foreach (var apple in Apples)
                {
                    float distance = Vector2Int.Distance(coord, apple.Pos);
                    var isAppleToCloseToOtherApples = distance < _minDistance;
                    if (isAppleToCloseToOtherApples)
                    {
                        if (DebugApples)
                        {
                            Debug.Log(distance + " is the distance from " + apple.ToString() + " to " + coord.ToString());
                        }
                        return false;
                    }
                }
            }
            _testPath = path;

            path = _pathfinding.FindPath(coord.x, coord.y, MapCubeEnd.Pos.x, MapCubeEnd.Pos.y - 1);
            if (path == null || path.Count == 0)
            {
                Debug.Log(coord + " Path Not Found");
                return false;
            }
            else
            {
                _isAppleCloseToEndPoint = path.Count <= _currentEnergy;
                if (_isAppleCloseToEndPoint == false)
                {
                    if (DebugApples)
                    {
                        Debug.Log(coord + " isAppleCloseToEndPoint: " + _isAppleCloseToEndPoint +
                            " ----- [Cost " + path.Count + " >= CurEng " + _currentEnergy + "]");
                    }
                    Game._.LevelController.ApplesCount++;
                }
                else
                {
                    // Here no test of same path??
                    _testPath.AddRange(path);
                }
            }
            _currentEnergy = _currentEnergy - energyCost + HiddenSettings._.AppleEnergy;
            return true;
        }
        return false;
    }

    public List<Vector2Int> GenerateRandomHoles()
    {
        float nrOfNodes = _height * _width;
        int nrOfHoles = (int)Mathf.Max(nrOfNodes * Game._.LevelController.HolesRatio);
        List<Vector2Int> generated = new List<Vector2Int>();
        for (var i = 0; i < nrOfHoles; i++)
        {
            var point = GetRandomPointOnMap();
            generated.Add(point);
            // Debug.Log("hole: " + point.ToString());
        }
        return generated.Distinct().ToList();
    }

    private Vector2Int GetRandomPointOnMap()
    {
        int randomX = Random.Range(0, _width - 1);
        int randomY = Random.Range(0, _height - 1);
        var point = new Vector2Int(randomX, randomY);
        if (IsOnStartOrEnd(point))
        {

            _maxPointsTry++;
            if (_maxPointsTry == HiddenSettings._.MaxPointsTryCrash)
            {
                Debug.Log("Can't find Points. Try " + _maxPointsTry);
                throw new System.Exception();
            }

            Debug.Log("isOnStartOrEnd");
            return GetRandomPointOnMap();
        }
        return point;
    }

    private bool IsOnStartOrEnd(Vector2Int point)
    {
        return ((MapCubeStart.Pos.x == point.x && (MapCubeStart.Pos.y + 1) == point.y)
            || (MapCubeEnd.Pos.x == point.x && (MapCubeEnd.Pos.y - 1) == point.y));
    }

    private void SetMapCubeWalkable(Vector2Int? pos, bool isWalkable, bool all = false)
    {
        if (all)
        {
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    if (Game._.LevelController.MapCubes[x, y].IsWalkable == !isWalkable)
                    {
                        Game._.LevelController.MapCubes[x, y].SetWalkable(isWalkable);
                    }
                }
            }
            return;
        }

        Game._.LevelController.MapCubes[pos.Value.x, pos.Value.y].SetWalkable(isWalkable);
    }

    public void CreateStartingCube()
    {
        GameObject go = Instantiate(PrefabBank._.GreenCube, new Vector3(0, 0, 0), Quaternion.identity);
        go.transform.SetParent(transform);
        go.transform.localPosition = new Vector3(_startAtX, 0, -1);
        MapCubeStart = go.GetComponent<MapCube>();
        // MapCubeStart.Init(_startAtX, -1, _debugThis);
        MapCubeStart.Init(new Vector2Int(_startAtX, -1), _debugThis);
        go = Instantiate(PrefabBank._.TreeLog, new Vector3(0, 0, 0), Quaternion.identity);
        go.transform.SetParent(MapCubeStart.transform);
        go.transform.localPosition = Vector3.zero;
    }

    public void CreateEndCube()
    {
        GameObject go = Instantiate(PrefabBank._.GreenCube, new Vector3(0, 0, 0), Quaternion.identity);
        go.transform.SetParent(transform);
        go.transform.localPosition = new Vector3(_endAtX, 0, _height);

        MapCubeEnd = go.GetComponent<MapCube>();
        // MapCubeEnd.Init(_endAtX, _height, _debugThis);
        MapCubeEnd.Init(new Vector2Int(_endAtX, _height), _debugThis);
        go = Instantiate(PrefabBank._.TreeLog, new Vector3(0, 0, 0), Quaternion.identity);
        go.transform.SetParent(MapCubeEnd.transform);
        go.transform.localEulerAngles = new Vector3(0, 180, 0);
        go.transform.localPosition = Vector3.zero;

        Game._.LevelController.MapCubes[_endAtX, _height - 1].IsEnd = true;
    }
}
