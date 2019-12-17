/**
 * Created by: Victoria Shenkevich
 * Created on: 15/12/2019
 */

using System;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    #region Constants
    private const int MATCH_MIN = 3;
    private const int EMPTY = 0;    
    #endregion


    #region Private Fields
	private int toDestroy;
	private int destroyed;
    private CellData[,] boardMatrix;
	#endregion


    #region Links
    [SerializeField] private Transform board;
    [SerializeField] private GameObject[] itemsPrefabs;
    #endregion


    #region Unity Methods
    void Awake()
    {
        Init();
    }
    #endregion


    #region Private Methods
    private void Init ()
    {
        InitBoard();

        CheckForMatches();
    }

    private void InitBoard()
    {
        boardMatrix = new CellData[Model.COLS, Model.ROWS];

        for (int i = 0; i < Model.COLS; i++)
        {
            for (int j = 0; j < Model.ROWS; j++)
            {
                FillCell(i, j);
            }
        }

        PrintMatrix("InitBoard");
    }

    private void FillCell(int x, int y)
    {
        boardMatrix[x, y] = new CellData(itemsPrefabs, board, new Vector2(x, y), MoveItems, StopMoveItems, OnItemDestroyed);
    }

    private void MoveItems(Vector2 touchedCoordinates, Vector2 delta)
    {
        if (Math.Abs(delta.x) > Math.Abs(delta.y))
        {
            int y = (int)touchedCoordinates.y;

            if (delta.x/Model.CellSize > 0)
            {
                MoveColumnRight (y);
            }
            else
            {                
                MoveColumnLeft (y);
            }
        }
        else
        {
            int x = (int)touchedCoordinates.x;

            if (delta.y/Model.CellSize > 0)
            {
                MoveColumnUp (x);
            }
            else
            {                
                MoveColumnDown (x);
            }
        }
    }

    private void MoveColumnUp(int columnID)
    {
        CellData cellData = boardMatrix[columnID, Model.ROWS - 1];

        for (int i = Model.ROWS - 1; i > 0; i--)
        {
            boardMatrix[columnID, i] = boardMatrix[columnID, i - 1];
            boardMatrix[columnID, i].item.UpdateCoordinates(new Vector2(columnID, i));
        }

        boardMatrix[columnID, 0] = cellData;
        boardMatrix[columnID, 0].item.UpdateCoordinates(new Vector2(columnID, 0));
    }

    private void MoveColumnDown(int columnID)
    {
        CellData cellData = boardMatrix[columnID, 0];

        for (int i = 0; i < Model.ROWS - 1; i++)
        {
            boardMatrix[columnID, i] = boardMatrix[columnID, i + 1];
            boardMatrix[columnID, i].item.UpdateCoordinates(new Vector2(columnID, i));
        }

        boardMatrix[columnID, Model.ROWS - 1] = cellData;
        boardMatrix[columnID, Model.ROWS - 1].item.UpdateCoordinates(new Vector2(columnID, Model.ROWS - 1));
    }

    private void MoveColumnRight(int rowID)
    {
        CellData cellData = boardMatrix[Model.COLS - 1, rowID];

        for (int i = Model.COLS - 1; i > 0; i--)
        {
            boardMatrix[i, rowID] = boardMatrix[i - 1, rowID];
            boardMatrix[i, rowID].item.UpdateCoordinates(new Vector2(i, rowID));
        }

        boardMatrix[0, rowID] = cellData;
        boardMatrix[0, rowID].item.UpdateCoordinates(new Vector2(0, rowID));
    }

    private void MoveColumnLeft(int rowID)
    {
        CellData cellData = boardMatrix[0, rowID];

        for (int i = 0; i < Model.COLS - 1; i++)
        {
            boardMatrix[i, rowID] = boardMatrix[i + 1, rowID];
            boardMatrix[i, rowID].item.UpdateCoordinates(new Vector2(i, rowID));
        }

        boardMatrix[Model.COLS - 1, rowID] = cellData;
        boardMatrix[Model.COLS - 1, rowID].item.UpdateCoordinates(new Vector2(Model.COLS - 1, rowID));
    }    

    private void StopMoveItems()
    {
        CheckForMatches();
    }

    private void OnItemDestroyed()
    {
        destroyed++;     

        if (toDestroy == destroyed)
        {
            toDestroy = destroyed = 0;
            PrintMatrix("OnItemDestroyed");
            FillEmptyCells();
        }
    }

    private void FillEmptyCells()
    {
        for (int i = 0; i < Model.COLS; i++)
        {
            for (int j = 0; j < Model.ROWS; j++)
            {
                if (boardMatrix[i, j].isEmpty())
                {
                    int next = EMPTY;
                    int yOfNext = j;

                    while (next == EMPTY && yOfNext < Model.ROWS-1)
                    {
                        next = boardMatrix[i, ++yOfNext].type;
                    }

                    if (next != EMPTY) 
                    {
                        DropItem(i, j, yOfNext);
                    }
                    else //there is no more items in this vertical
                    {
                        for (int k = j; k < Model.ROWS; k++)
                        {
                            FillCell(i, k);
                        }
                    }
                }
            }
        }

        PrintMatrix("FillEmptyCells");

        CheckForMatches();
    }

    private void DropItem(int x, int y, int yOfNext)
    {
        boardMatrix[x, y] = boardMatrix[x, yOfNext];
        boardMatrix[x, y].item.UpdateCoordinates(new Vector2(x, y));

        boardMatrix[x, yOfNext].Empty();
    }

    private void CheckForMatches ()
    {
        for (int i = 0; i < Model.COLS; i++)
        {
            for (int j = 0; j < Model.ROWS; j++)
            {
                int onCheck = boardMatrix[i, j].type;

                if (onCheck != EMPTY)
                {
                    int numberOfSameInCol = CountSameInColumn(i, j, onCheck);
                    int numberOfSameInRow = CountSameInRow(i, j, onCheck);

                    if (numberOfSameInCol >= MATCH_MIN)
                    {
                        DestroyMatchInColumn(i, j, numberOfSameInCol);
                    }

                    if (numberOfSameInRow >= MATCH_MIN)
                    {
                        if (numberOfSameInCol >= MATCH_MIN)
                        {
                            DestroyMatchInRow(i, j + 1, numberOfSameInRow - 1);
                        }
                        else
                        {
                            DestroyMatchInRow(i, j, numberOfSameInRow);
                        }
                    }
                }
            }
        }
    }

    private int CountSameInRow(int startX, int startY, int onCheck)
    {
        int numberOfSameInCol = 1;

        for (int k = startX + 1; k < Model.COLS; k++)
        {
            if (boardMatrix[k, startY].type == onCheck)
                numberOfSameInCol++;
            else
                break;
        }

        return numberOfSameInCol;
    }

    private int CountSameInColumn(int startX, int startY, int onCheck)
    {
        int numberOfSameInRow = 1;

        for (int k = startY + 1; k < Model.ROWS; k++)
        {
            if (boardMatrix[startX, k].type == onCheck)
                numberOfSameInRow++;
            else
                break;
        }

        return numberOfSameInRow;
    }

    private void DestroyMatchInColumn(int startX, int startY, int numberOfSameInCol)
    {
        for (int n = 0; n < numberOfSameInCol; n++)
        {
            toDestroy++;

            boardMatrix[startX, startY + n].Destroy();

            GameManager.Instance.UpdateScore(Model.POINTS_PER_ITEM);
        }
    }

    private void DestroyMatchInRow(int startX, int startY, int numberOfSameInRow)
    {
        for (int n = 0; n < numberOfSameInRow; n++)
        {
            toDestroy++;

            boardMatrix[startX + n, startY].Destroy();
            
            GameManager.Instance.UpdateScore(Model.POINTS_PER_ITEM);
        }
    }
    
    //For debug usage
    private void PrintMatrix (string methodName)
    {
        string matrixAsString = methodName + "\n";

        //Reversed for seeing it like the UI
        for (int i = Model.ROWS - 1; i >= 0; i--)
        {
            for (int j = 0; j < Model.COLS; j++)
            {
                //Reversed for seeing it like the UI
                matrixAsString += boardMatrix[j, i].type + ", ";
            }

            matrixAsString += "\n";
        }

        Debug.Log(matrixAsString);
    }
    #endregion    


    #region Structs
    public struct CellData
    {
        public int type;
        public Item item;

        public CellData (GameObject[] itemsPrefabs, Transform parent, Vector2 coordinates, Action<Vector2, Vector2> OnMovementCallback, Action OnMovementStopCallback, Action OnDestroyCallback)
        {
            type = UnityEngine.Random.Range(0, itemsPrefabs.Length) + 1;

            item = Instantiate(itemsPrefabs[type - 1], parent).GetComponent<Item>();
            item.Init(coordinates, OnMovementCallback, OnMovementStopCallback, OnDestroyCallback);
        }

        public void Destroy()
        {
            item.Pop();

            Empty();
        }

        public void Empty()
        {
            type = EMPTY;
            item = null;
        }

        public bool isEmpty()
        {
            return type == EMPTY;
        }
    }
    #endregion 
}