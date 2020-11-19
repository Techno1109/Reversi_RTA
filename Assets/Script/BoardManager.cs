using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject GridPrefab;
    
    [SerializeField] private GridData[,] _gridDatas = new GridData[8,8];


    [SerializeField] private Color gridColor_1;
    [SerializeField] private Color gridColor_2;
    [SerializeField] private Color whiteColor;
    [SerializeField] private Color blackColor;
    [SerializeField] private Color highlightColor;
   
    [SerializeField] private Text WhiteCount;
    [SerializeField] private Text BlackCount;
    [SerializeField] private Text TurnOwnerText;

    private GridState _nowTurnOwner=GridState.BLACK;
    
    public void SetPiece(Vector2 targetGrid)
    {
        if (targetGrid.x<0||targetGrid.y<0)
        {
            return;
        }

        if (targetGrid.x >= 8 || targetGrid.y >= 8)
        {
            return;
        }

        int xPos = (int) targetGrid.x;
        int yPos = (int) targetGrid.y;
        _gridDatas[xPos,yPos].SetState(_nowTurnOwner);
        StartFlip((int) targetGrid.x, (int) targetGrid.y);

        NextTurn();

        CheckBoardState();
    }

    private void CheckBoardState()
    {
        //黒と白、ハイライトの数をカウント
        int blackCount = 0;
        int whiteCount = 0;
        int highLightCount = 0;
        
        for (int x = 0; x < 8; ++x)
        {
            for (int y = 0; y < 8; ++y)
            {
                switch(_gridDatas[x, y].GetState())
                {
                    case GridState.NONE:
                        break;
                    case GridState.HIGHLIGHT:
                        highLightCount++;
                        break;
                    case GridState.WHITE:
                        whiteCount++;
                        break;
                    case GridState.BLACK:
                        blackCount++;
                        break;
                    default:
                        break;
                }

            }
        }

        if (whiteCount + blackCount == 64)
        {
            //ゲーム終了
        }
        
        
        if (highLightCount == 0)
        {
            NextTurn();
        }

        switch (_nowTurnOwner)
        {

            case GridState.WHITE:
                TurnOwnerText.text = "WhiteTurn";
                break;
            case GridState.BLACK:
                TurnOwnerText.text = "BlackTurn";
                break;
            
            case GridState.NONE:
                break;
            case GridState.HIGHLIGHT:
                break;
            default:
                break;
        }
        
        WhiteCount.text = whiteCount.ToString();
        BlackCount.text = blackCount.ToString();
    }

    void NextTurn()
    {
        switch (_nowTurnOwner)
        {
            
            case GridState.WHITE:
                _nowTurnOwner = GridState.BLACK;
                break;
            case GridState.BLACK:
                _nowTurnOwner = GridState.WHITE;
                break;
            
            
            case GridState.NONE:
                break;
            case GridState.HIGHLIGHT:
                break;
        }
        
        RefreshHighLight();
    }
    
    private void StartFlip(int x_startPos,int y_startPos)
    {
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                FlipPiece(x_startPos + x, y_startPos + y, x , y,false);
            }
        }
        
    }
    
    private bool FlipPiece(int x_Pos,int y_Pos,int x_Vec,int y_Vec,bool highlightMode)
    {
        if (x_Pos<0||y_Pos<0)
        {
            return false;
        }

        if (x_Pos>= 8 || y_Pos >= 8)
        { 
            return false;
        }

        var nowGridState = _gridDatas[x_Pos, y_Pos].GetState();
        
        if (nowGridState==GridState.NONE||nowGridState==GridState.HIGHLIGHT)
        {
            return false;
        }
        
        if (nowGridState==_nowTurnOwner)
        {
            return true;
        }
        else
        {
            if (FlipPiece(x_Pos+x_Vec,y_Pos+y_Vec,x_Vec,y_Vec,highlightMode))
            {
                if (!highlightMode)
                {
                    _gridDatas[x_Pos, y_Pos].SetState(_nowTurnOwner);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        
    }

    private void RefreshHighLight()
    {
        for (int x = 0; x < 8; ++x)
        {
            for (int y = 0; y < 8; ++y)
            {
                if (_gridDatas[x, y].GetState() == GridState.HIGHLIGHT)
                {
                    _gridDatas[x, y].SetState(GridState.NONE);
                }

                if (_gridDatas[x, y].GetState() == GridState.NONE)
                {
                    SetHighlight(x, y);
                }
                
            }
        }
        RefreshBoard();
    }
    
    
    private void SetHighlight(int x_startPos,int y_startPos)
    {
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                int x_nextPos = x_startPos + x;
                int y_nextPos = y_startPos + y;
                if (x_nextPos<0||y_nextPos<0)
                {
                    continue;
                }

                if (x_nextPos>= 8 || y_nextPos >= 8)
                { 
                    continue;
                }
                
                if(_gridDatas[x_nextPos, y_nextPos].GetState()==_nowTurnOwner)
                {
                    continue;
                }
                
                if (FlipPiece(x_nextPos ,y_nextPos , x, y, true))
                {
                    _gridDatas[x_startPos, y_startPos].SetState(GridState.HIGHLIGHT);
                }
            }
        }
    }
    
    private void MakeBoard()
    {
        Vector3 setPisiton=new Vector3(0, 0, 0);
        GridData targetGridData = null;
        GameObject tmp = null;
        for (int x = 0; x < 8; ++x)
        {
            for (int y = 0; y < 8; ++y)
            {
                Instantiate(GridPrefab, new Vector3(x-3.5f,y-3.5f,0), Quaternion.identity).TryGetComponent(out targetGridData);
                targetGridData.Init(new Vector2(x, y), this);
                if((((x%2)+y)%2) >0)
                {
                   targetGridData.SetColor(gridColor_1);
                }
                else
                {
                    targetGridData.SetColor(gridColor_2);
                }

                _gridDatas[x, y] = targetGridData;
            }
        }
        
        _gridDatas[3, 3].SetState(GridState.BLACK);
        _gridDatas[4, 4].SetState(GridState.BLACK);
        _gridDatas[3, 4].SetState(GridState.WHITE);
        _gridDatas[4, 3].SetState(GridState.WHITE);
        
        RefreshBoard();
    }
    
    private void RefreshBoard()
    {
        for (int x = 0; x < 8; ++x)
        {
            for (int y = 0; y < 8; ++y)
            {

                switch (_gridDatas[x,y].GetState())
                {
                    case GridState.NONE:
                    case GridState.HIGHLIGHT:
                        //普通の色に戻す
                        if((((x%2)+y)%2) >0)
                        {
                            _gridDatas[x, y].SetColor(gridColor_1);
                        }
                        else
                        {
                            _gridDatas[x, y].SetColor(gridColor_2);
                        }
                        break;
                    //_gridDatas[x, y].SetColor(highlightColor);
                        //break;
                    
                    case GridState.WHITE:
                        _gridDatas[x, y].SetColor(whiteColor);
                        break;
                    case GridState.BLACK:
                        _gridDatas[x, y].SetColor(blackColor);
                        break;
                    default:
                        Debug.Log("盤面の色更新でエラーが発生しました。");
                        break;
                }
                

            }
        }
    }
    
    
    private void Start()
    {
        MakeBoard();
        RefreshHighLight();
        CheckBoardState();
    }
}
