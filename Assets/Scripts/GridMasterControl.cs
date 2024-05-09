using System;
using System.Collections.Generic;
using TMPro;
//using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
//using TMPro;

public class GridMasterControl : MonoBehaviour
{
    public int _gridSizeX;
    public int _gridSizeY;

    public GameObject _startGameText;
    public TMP_Text _endGameText;

    public GameObject _cellPrefab;
    public GameObject _snakeHeadPrefab;
    public GameObject _snakeBodyPrefab;
    public GameObject _foodPrefab;
    public GameObject _emptyTilePrefab;
    public GameObject _mainCamera;

    public GridCellControl _currentSnakeHeadCell;
    public GridCellControl _currentFoodCell;

    public List<List<GridCellControl>> _cellGridList = new List<List<GridCellControl>>();

    public List<GridCellControl> _snakeBodyList = new List<GridCellControl>();

    Vector2 _playerDirectionVector;

    private bool _gameStart = false;
    public bool _GAMEOVER = false;

    private float _timer = 0;
    public float _totalGameTime = 0;
    public float _updateTimer = 1;
    public float _timerMultiplier = 0.97f;

    public float _foodCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        _GenerateCellGridList();
        _mainCamera.transform.position = new Vector3((_gridSizeX * .5f) - .5f, (_gridSizeY * .5f) - .5f, -10);
    }

    // Update is called once per frame
    void Update()
    {
        if (_GAMEOVER)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
            }
        }

        if (!_gameStart)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _gameStart = true;
                _startGameText.SetActive(false);
            }
        }
        else if (_gameStart && !_GAMEOVER)
        {
            _timer += Time.deltaTime;
            _totalGameTime += Time.deltaTime;

            if (_timer >= _updateTimer)
            {
                _UpdateSnakePosition();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (_currentSnakeHeadCell._cellDirection != GridCellControl._directionList._South)
                {
                    _currentSnakeHeadCell._cellDirection = GridCellControl._directionList._North;
                    _currentSnakeHeadCell._RotateCellToDirection();
                    _UpdateSnakePosition();

                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (_currentSnakeHeadCell._cellDirection != GridCellControl._directionList._North)
                {
                    _currentSnakeHeadCell._cellDirection = GridCellControl._directionList._South;
                    _currentSnakeHeadCell._RotateCellToDirection();
                    _UpdateSnakePosition();
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (_currentSnakeHeadCell._cellDirection != GridCellControl._directionList._West)
                {
                    _currentSnakeHeadCell._cellDirection = GridCellControl._directionList._East;
                    _currentSnakeHeadCell._RotateCellToDirection();
                    _UpdateSnakePosition();
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (_currentSnakeHeadCell._cellDirection != GridCellControl._directionList._East)
                {
                    _currentSnakeHeadCell._cellDirection = GridCellControl._directionList._West;
                    _currentSnakeHeadCell._RotateCellToDirection();
                    _UpdateSnakePosition();
                }
            }
        }
    }

    void _ReduceTimer()
    {
        if (_updateTimer >= 0.1f) _updateTimer *= _timerMultiplier;
    }

    bool _CheckForBoundaries(int _dirX, int _dirY)
    {
        if (_dirX >= 0 && _dirX < _cellGridList.Count && _dirY >= 0 && _dirY < _cellGridList[_dirX].Count)
        {
            return true;
        }
        else return false;
    }

    void _UpdateSnakePosition()
    {
        _timer = 0;

        GridCellControl._directionList _headDirection = _currentSnakeHeadCell._cellDirection;
        bool _hasEaten = false;

        switch (_headDirection)
        {
            case GridCellControl._directionList._North: _playerDirectionVector = new Vector2(0, 1); break;
            case GridCellControl._directionList._South: _playerDirectionVector = new Vector2(0, -1); break;
            case GridCellControl._directionList._West: _playerDirectionVector = new Vector2(-1, 0); break;
            case GridCellControl._directionList._East: _playerDirectionVector = new Vector2(1, 0); break;
        }

        int iX = _currentSnakeHeadCell._cellIndexX + (int)_playerDirectionVector.x;
        int iY = _currentSnakeHeadCell._cellIndexY + (int)_playerDirectionVector.y;

        if (_CheckForBoundaries(iX, iY))
        {
            GridCellControl _targetCell;
            GridCellControl _previousCell;

            //foreach (GridCellControl _cell in _snakeBodyList)
            //{
            _targetCell = _cellGridList
                [_currentSnakeHeadCell._cellIndexX + (int)_playerDirectionVector.x]
                [_currentSnakeHeadCell._cellIndexY + (int)_playerDirectionVector.y];
            _previousCell = _currentSnakeHeadCell;

            if (_targetCell._thisCellType == GridCellControl._cellTypes._Food)
            {
                _hasEaten = true;
                _foodCounter++;
                _ReduceTimer();
            }
            else if (_targetCell._thisCellType == GridCellControl._cellTypes._SnakeBody)
            {
                _EndGame();
                return;
            }

            _targetCell._cellDirection = _currentSnakeHeadCell._cellDirection;
            _currentSnakeHeadCell = _targetCell;
            _targetCell._UpdateThisCellContent(GridCellControl._cellTypes._SnakeHead);
            _snakeBodyList.Add(_currentSnakeHeadCell);

            _previousCell._UpdateThisCellContent(GridCellControl._cellTypes._SnakeBody);

            //}
            if (_hasEaten)
            {
                _SetFoodPosition();
            }
            else
            {
                //int n = _snakeBodyList.Count - 1;
                //Debug.Log(_snakeBodyList[0]._thisCellType);
                _snakeBodyList[0]._UpdateThisCellContent(GridCellControl._cellTypes._Empty);
                _snakeBodyList.RemoveAt(0);
            }
        }
        else
        {
            _EndGame();
            return;
        }
    }

    void _GenerateCellGridList()
    {

        for (int iX = 0; iX < _gridSizeX; iX++)
        {
            _cellGridList.Add(new List<GridCellControl>());

            for (int iY = 0; iY < _gridSizeY; iY++)
            {
                GameObject _thisCell = Instantiate(_cellPrefab, this.transform);
                _cellGridList[iX].Add(_thisCell.GetComponent<GridCellControl>());
                _thisCell.transform.position = new Vector2(iX, iY);
                _cellGridList[iX][iY]._gridMasterControl = this;
                _DefineFirstSetupCellType(_cellGridList[iX][iY], iX, iY);
                _cellGridList[iX][iY]._cellIndexX = iX;
                _cellGridList[iX][iY]._cellIndexY = iY;
            }
        }

        _SetFoodPosition();
    }

    void _DefineFirstSetupCellType(GridCellControl _cell, int _posX, int _posY)
    {
        //GameObject _content = new GameObject();

        if (_posX == Mathf.Floor((_gridSizeX * .5f) - 1) && _posY == Mathf.Floor(_gridSizeY * .5f))
        {
            _currentSnakeHeadCell = _cell;
            _cell._UpdateThisCellContent(GridCellControl._cellTypes._SnakeHead);
            _snakeBodyList.Add(_cell);
        }
        else if (_posX == Mathf.Floor((_gridSizeX * .5f) - 2) && _posY == Mathf.Floor(_gridSizeY * .5f))
        {
            _cell._UpdateThisCellContent(GridCellControl._cellTypes._SnakeBody);
            _snakeBodyList.Add(_cell);
        }
        else
        {
            _cell._UpdateThisCellContent(GridCellControl._cellTypes._Empty);
        }
    }

    void _SetFoodPosition()
    {
        List<GridCellControl> _emptyTileCellList = new List<GridCellControl>();

        for (int iX = 0; iX < _cellGridList.Count; iX++)
        {
            for (int iY = 0; iY < _cellGridList[iX].Count; iY++)
            {
                if (_cellGridList[iX][iY]._thisCellType == GridCellControl._cellTypes._Empty)
                {
                    _emptyTileCellList.Add(_cellGridList[iX][iY]);
                }
            }
        }
        if (_emptyTileCellList.Count == 0)
        {
            Debug.Log("No more empty spaces left!");
            return;
        }

        int _randomIndex = (int)MathF.Floor(Random.Range(0, _emptyTileCellList.Count) - 1);
        if (_randomIndex < 0) _randomIndex = 0;

        //Debug.Log(_randomIndex);

        _currentFoodCell = _emptyTileCellList[_randomIndex];
        _currentFoodCell._UpdateThisCellContent(GridCellControl._cellTypes._Food);
    }

    void _EndGame()
    {
        _GAMEOVER = true;

        _endGameText.text = "Game Over\r\nFinal Score: " + _FinalScore();
        _endGameText.gameObject.SetActive(true);
    }

    float _FinalScore()
    {
        float score = _foodCounter * 10 - Mathf.Ceil(_totalGameTime * 0.3f);

        if (score < 0) score = 0;
        return _foodCounter + score;
    }
}