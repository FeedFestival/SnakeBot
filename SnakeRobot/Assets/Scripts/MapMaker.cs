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
    public bool DebugThis;
    private bool _isAppleCloseToEndPoint;
    private List<PathNode> _testPath;
    private int _currentEnergy;

    public void InitValues(bool debugThis)
    {
        _debugThis = debugThis;
        _height = Game._.LevelController.Y;
        _width = Game._.LevelController.X;
        _startAtX = Game._.LevelController.StartAtX;
        _endAtX = Game._.LevelController.EndAtX;
        _appleIndex = 0;
        _currentEnergy = HiddenSettings._.StartingEnergy;
    }

    public void RandomizeLevelValues(bool debugThis)
    {
        _debugThis = debugThis;
        _height = (int)Random.Range(3, 10);
        _width = (int)Random.Range(3, 10);
        Game._.LevelController.Y = _height;
        Game._.LevelController.X = _width;
        _startAtX = (int)Random.Range(1, _width - 1);
        Game._.LevelController.StartAtX = _startAtX;
        _endAtX = (int)Random.Range(1, _width - 1);
        Game._.LevelController.EndAtX = _endAtX;
        _appleIndex = 0;
        _currentEnergy = HiddenSettings._.StartingEnergy;
    }

    public MapCube[,] CreateMap()
    {
        GameObject go;
        var MapCubes = new MapCube[_height, _width];
        var mcX = _width / 2 - 0.5f;
        var mcY = _height / 2 - 0.5f;
        transform.position = new Vector3(-mcX, 0, -mcY);
        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                go = Instantiate(PrefabBank._.GreenCube, new Vector3(0, 0, 0), Quaternion.identity);
                go.transform.SetParent(transform);
                go.transform.localPosition = new Vector3(x, 0, y);
                MapCubes[y, x] = go.GetComponent<MapCube>();
                MapCubes[y, x].Init(new Vector2Int(x, y), _debugThis);
            }
        }
        return MapCubes;
    }

    public void CreateHoles()
    {
        List<Vector2Int> holes = GetValidHoles();

        foreach (Vector2Int hole in holes)
        {
            Game._.LevelController.MapCubes[hole.x, hole.y].ShowObj(false);
        }
    }

    public List<Vector2Int> GetValidHoles()
    {
        var holes = GenerateRandomHoles();
        if (CanCompleteLevel(holes) == false)
        {
            return GetValidHoles();
        }
        return holes;
    }

    public bool CanCompleteLevel(List<Vector2Int> holes)
    {
        _pathfinding = new Pathfinding(_width, _height, moveDiagonally: false);
        _testPath = new List<PathNode>();

        var endY = MapCubeEnd.Pos.y - 1;
        var startY = MapCubeStart.Pos.y + 1;

        _pathfinding.SetAllWalkable(true);
        _pathfinding.SetSomeIsWalkable(holes, false);

        for (int x = 0; x < _pathfinding.GetGrid().GetWidth(); x++)
        {
            for (int y = 0; y < _pathfinding.GetGrid().GetHeight(); y++)
            {
                if (_pathfinding.GetGrid().GetGridArray()[x, y].isWalkable)
                {
                    Debug.Log(_pathfinding.GetGrid().GetGridArray()[x, y]);
                }
            }
        }
        List<PathNode> path = _pathfinding.FindPath(MapCubeStart.Pos.x, startY, MapCubeEnd.Pos.x, endY);

        if (path == null || path.Count == 0)
        {
            Debug.Log("No Path to End");
            return false;
        }

        if (DebugThis)
        {
            foreach (PathNode pNode in path)
            {
                Debug.Log(pNode.ToString());
                Game._.LevelController.MapCubes[pNode.y, pNode.x].ChangeColor(PathColor.Pink);
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
                if (DebugThis)
                {
                    Debug.Log("_testPath: " + _testPath.Count);
                    int index = 0;
                    foreach (PathNode pNode in _testPath.Distinct().ToList())
                    {
                        Game._.LevelController.MapCubes[pNode.y, pNode.x].ChangeColor(PathColor.Blue);
                        Game._.LevelController.MapCubes[pNode.y, pNode.x].ShowIndex(index);
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
        Vector2Int coord = GetValidAppleCoordinates();
        GameObject go = Instantiate(PrefabBank._.Apple, new Vector3(0, 0, 0), Quaternion.identity);
        go.transform.SetParent(transform);
        Apple apple = go.GetComponent<Apple>();
        apple.PlaceApple(coord);
        apple.gameObject.name = "Apple " + coord;
        Apples.Add(apple);
        _appleIndex++;

        if (DebugThis)
        {
            Game._.LevelController.MapCubes[coord.y, coord.x].ShowEnergy(_currentEnergy);
        }
    }

    private Vector2Int GetValidAppleCoordinates()
    {
        var coord = GetRandomPointOnMap();
        if (CanAddApple(coord) == false)
        {
            return GetValidAppleCoordinates();
        }
        return coord;
    }

    private bool CanAddApple(Vector2Int coord)
    {
        bool canPlaceApple = !(Apples.Any(a => a.Pos.x == coord.x && a.Pos.y == coord.y));

        if (canPlaceApple)
        {
            bool isFirstApple = _appleIndex == 0;
            Vector2Int closeTo = isFirstApple
                ? new Vector2Int(MapCubeStart.Pos.y + 1, MapCubeStart.Pos.x)
                    : new Vector2Int(Apples[_appleIndex - 1].Pos.y, Apples[_appleIndex - 1].Pos.x);

            List<PathNode> path = _pathfinding.FindPath(coord.x, coord.y, closeTo.x, closeTo.y);
            if (path == null || path.Count == 0)
            {
                Debug.Log("Path Not Found");
                return false;
            }
            int energyCost = path.Count;

            if (isFirstApple)
            {
                var isAppleTooFarFromStartPoint = energyCost >= _currentEnergy;
                if (DebugThis)
                {
                    Debug.Log("isAppleTooFarFromStartPoint: " + isAppleTooFarFromStartPoint + " EC " + energyCost + " >= CurEng " + _currentEnergy);
                }
                if (isAppleTooFarFromStartPoint)
                {
                    return false;
                }
            }
            else
            {
                var isAppleTooFarFromPreviousApple = energyCost >= _currentEnergy;
                if (DebugThis)
                {
                    Debug.Log("isAppleTooFarFromPreviousApple: " + isAppleTooFarFromPreviousApple + " EC " + energyCost + " >= CurEng " + _currentEnergy);
                }
                if (isAppleTooFarFromPreviousApple)
                {
                    return false;
                }

                float _minDistance = 1.5f;
                foreach (var apple in Apples)
                {
                    float distance = Vector2Int.Distance(coord, apple.Pos);
                    var isAppleToCloseToOtherApples = distance < _minDistance;
                    if (isAppleToCloseToOtherApples)
                    {
                        if (DebugThis)
                        {
                            Debug.Log(distance + " is the distance from " + apple.ToString() + " to " + coord.ToString());
                        }
                        return false;
                    }
                }
            }
            path.Reverse();
            _testPath.AddRange(path);

            path = _pathfinding.FindPath(coord.x, coord.y, MapCubeEnd.Pos.x, MapCubeEnd.Pos.y - 1);
            if (path == null || path.Count == 0)
            {
                Debug.Log("Path Not Found");
            }
            else
            {
                _isAppleCloseToEndPoint = path.Count <= _currentEnergy;
                if (DebugThis)
                {
                    Debug.Log("isAppleCloseToEndPoint: " + _isAppleCloseToEndPoint + " EC " + path.Count + " >= CurEng " + _currentEnergy);
                }
                if (_isAppleCloseToEndPoint == false)
                {
                    Game._.LevelController.ApplesCount++;
                }
                else
                {
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
        int nrOfHoles = (int)Mathf.Max(nrOfNodes * 0.44f);
        List<Vector2Int> generated = new List<Vector2Int>();
        for (var i = 0; i < nrOfHoles; i++)
        {
            var point = GetRandomPointOnMap();
            generated.Add(point);
            Debug.Log("hole: " + point.ToString());
        }
        return generated.Distinct().ToList();
    }

    public Vector2Int GetRandomPointOnMap()
    {
        bool isFirstApple = _appleIndex == 0;
        int randomY;
        if (isFirstApple)
        {
            randomY = Random.Range(0, _height);
        }
        else
        {
            int biggestY = Apples.Max(a => a.Pos.y);
            randomY = Random.Range(biggestY, _height);
        }
        return new Vector2Int(randomY, Random.Range(0, _width));
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

        Game._.LevelController.MapCubes[_height - 1, _endAtX].IsEnd = true;
    }
}
