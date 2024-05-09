using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellControl : MonoBehaviour
{
    public GridMasterControl _gridMasterControl;
    public GameObject _thisCellContent;
    public int _cellIndexX;
    public int _cellIndexY;
    public enum _cellTypes
    {
        _SnakeHead,
        _SnakeBody,
        _Food,
        _Empty
    }

    public _cellTypes _thisCellType = _cellTypes._Empty;
    public enum _directionList
    {
        _East = 0,
        _North = 90,
        _South = 270,
        _West = 180
    }

    public _directionList _cellDirection = _directionList._East;

    public void _UpdateThisCellContent(_cellTypes _type)
    {

        if (_thisCellContent != null)
        {
            Destroy(_thisCellContent.gameObject);
            _thisCellContent = null;
        }

        if (_type == _cellTypes._Empty)
        {
            _thisCellContent = Instantiate(_gridMasterControl._emptyTilePrefab, this.transform);
        }
        else if (_type == _cellTypes._Food)
        {
            _thisCellContent = Instantiate(_gridMasterControl._foodPrefab, this.transform);
        }
        else if (_type == _cellTypes._SnakeBody)
        {
            _thisCellContent = Instantiate(_gridMasterControl._snakeBodyPrefab, this.transform);
        }
        else if (_type == _cellTypes._SnakeHead)
        {
            _thisCellContent = Instantiate(_gridMasterControl._snakeHeadPrefab, this.transform);
        }

        _thisCellType = _type;
        _RotateCellToDirection();
    }

    public void _RotateCellToDirection()
    {
        transform.rotation = Quaternion.Euler(0, 0, (float)_cellDirection);
    }
}
