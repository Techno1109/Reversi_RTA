using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GridState
{
    NONE,
    HIGHLIGHT,
    WHITE,
    BLACK
}


public class GridData : MonoBehaviour
{

    private SpriteRenderer _spriteRenderer;

    private Vector2 _gridPos;

    private GridState _state;

    private BoardManager _boardManager;
    
    public bool Init(Vector2 setGrid,BoardManager setBoardManager)
    {
        if (!TryGetComponent(out _spriteRenderer))
        {
            return false;
        }
        
        _state = GridState.NONE;

        _gridPos = setGrid;

        _boardManager = setBoardManager;
        
        return true;
    }

    public GridState GetState()
    {
        return _state;
    }


    public bool SetState(GridState setState)
    {
        _state = setState;
        
        return true;
    }

    public void SetColor(Color setColor)
    {
        if (_spriteRenderer)
        {
            TryGetComponent(out _spriteRenderer);
        }

        _spriteRenderer.color = setColor;
    }

    private void OnMouseDown()
    {
        if (_state==GridState.HIGHLIGHT)
        {
            _boardManager.SetPiece(_gridPos);   
        }
    }
}

