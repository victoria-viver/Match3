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
    public const int NONE = -1;
    #endregion


    #region Private Fields
	private int toDestroy;
	private int destroyed;

	private int currentlyMovingRow = NONE;
	private int currentlyMovingColumn = NONE;

	private int lastMovedRow = NONE;
	private int lastMovedColumn = NONE;

    private int stepsInRow = 0;
	private int stepsInColumn = 0;

    private CellData[,] boardMatrix;
	#endregion


    #region Links
    [SerializeField] private Transform board;
    [SerializeField] private GameObject[] itemsPrefabs;
    #endregion


    #region Unity Methods
    void Awake()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        int width = Model.CellSize * Model.COLS + Model.BORDER_GAP;
        int height = Model.CellSize * Model.ROWS + Model.BORDER_GAP;

        GetComponent<RectTransform>().sizeDelta = new Vector2 (width, height);
        GetComponent<RectTransform>().position = new Vector2 (Model.BORDER_GAP/2 + width/2, Model.BORDER_GAP/2 + height/2);
    }
    #endregion
    

    #region Private Methods
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

        // PrintMatrix("InitBoard");
    }

    private void FillCell(int x, int y)
    {
        boardMatrix[x, y] = new CellData(itemsPrefabs, board, new Vector2(x, y), MoveItems, StopMoveItems, OnItemDestroyed);
    }

    private void MoveItems(Vector2 touchedCoordinates, Vector2 delta)
    {
        if (Math.Abs(delta.x) > Math.Abs(delta.y))
        {
            int row = (int)touchedCoordinates.y;

            if (currentlyMovingColumn == NONE &&
                (currentlyMovingRow == NONE || currentlyMovingRow == row))
            {
                currentlyMovingRow = row;
                lastMovedRow = row;

                if (Math.Abs(delta.x) >= Model.CellSize)
                {
                    if (delta.x > 0)
                    {
                        MoveRowRight (currentlyMovingRow);
                    }
                    else
                    {                
                        MoveRowLeft (currentlyMovingRow);
                    }
                }
                else
                {
                    MovePartiallyRow(currentlyMovingRow, delta.x);
                }                
            }
        }
        else
        {            
            int column = (int)touchedCoordinates.x;

            if (currentlyMovingRow == NONE &&
                (currentlyMovingColumn == NONE || currentlyMovingColumn == column))
            {
                currentlyMovingColumn = column;
                lastMovedColumn = column;
               
                if (Math.Abs(delta.y) >= Model.CellSize)
                {
                    if (delta.y > 0)
                    {
                        MoveColumnUp (currentlyMovingColumn);
                    }
                    else
                    {                
                        MoveColumnDown (currentlyMovingColumn);
                    }
                }
                else
                {
                    MovePartiallyColumn(currentlyMovingColumn, delta.y);
                }   
            }
        }
    }

    private void MovePartiallyRow(int rowID, float x)
    {
        for (int i = 0; i < Model.COLS; i++)
        {
            boardMatrix[i, rowID].item.MovePartially(new Vector2 (x, 0));
        }
    }

    private void MovePartiallyColumn(int columnID, float y)
    {
        for (int i = 0; i < Model.ROWS; i++)
        {
            boardMatrix[columnID, i].item.MovePartially(new Vector2 (0, y));
        }
    }

    private void MoveColumnUp(int columnID)
    {
        stepsInColumn++;

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
        stepsInColumn--;

        CellData cellData = boardMatrix[columnID, 0];

        for (int i = 0; i < Model.ROWS - 1; i++)
        {
            boardMatrix[columnID, i] = boardMatrix[columnID, i + 1];
            boardMatrix[columnID, i].item.UpdateCoordinates(new Vector2(columnID, i));
        }

        boardMatrix[columnID, Model.ROWS - 1] = cellData;
        boardMatrix[columnID, Model.ROWS - 1].item.UpdateCoordinates(new Vector2(columnID, Model.ROWS - 1));
    }

    private void MoveRowRight(int rowID)
    {
        stepsInRow++;

        CellData cellData = boardMatrix[Model.COLS - 1, rowID];

        for (int i = Model.COLS - 1; i > 0; i--)
        {
            boardMatrix[i, rowID] = boardMatrix[i - 1, rowID];
            boardMatrix[i, rowID].item.UpdateCoordinates(new Vector2(i, rowID));
        }

        boardMatrix[0, rowID] = cellData;
        boardMatrix[0, rowID].item.UpdateCoordinates(new Vector2(0, rowID));
    }

    private void MoveRowLeft(int rowID)
    {
        stepsInRow--;

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
        currentlyMovingRow = NONE;
        currentlyMovingColumn = NONE;

        if (IsTurnValid())
        {
            PopMatches();

            stepsInRow = 0;
            stepsInColumn = 0;
        }
        else
        {
            ReverseMovements ();

            lastMovedRow = NONE;
            lastMovedColumn = NONE;
        }
    }

    private void ReverseMovements()
    {
        stepsInRow = stepsInRow % Model.ROWS;
        stepsInColumn = stepsInColumn % Model.COLS;

        while (stepsInRow > 0)
        {
            MoveRowLeft(lastMovedRow);
        }

        while (stepsInRow < 0)
        {
            MoveRowRight(lastMovedRow);
        }

        while (stepsInColumn > 0)
        {
            MoveColumnDown(lastMovedColumn);
        }

        while (stepsInColumn < 0)
        {
            MoveColumnUp(lastMovedColumn);
        }   
    }

    private void OnItemDestroyed()
    {
        destroyed++;     

        if (toDestroy == destroyed)
        {
            GameManager.Instance.BlockScreen (false);

            toDestroy = destroyed = 0;
            // PrintMatrix("OnItemDestroyed");
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

        // PrintMatrix("FillEmptyCells");

        if (IsTurnValid())
        {
            PopMatches();
        }
    }

    private void DropItem(int x, int y, int yOfNext)
    {
        boardMatrix[x, y] = boardMatrix[x, yOfNext];
        boardMatrix[x, y].item.UpdateCoordinates(new Vector2(x, y));

        boardMatrix[x, yOfNext].Empty();
    }

    private bool IsTurnValid ()
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
                        return true;
                    }

                    if (numberOfSameInRow >= MATCH_MIN)
                    {
                        return true;
                    }
                }
            }
        }
        
        return false;
    }

    private void PopMatches ()
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
        GameManager.Instance.BlockScreen (true);

        for (int n = 0; n < numberOfSameInCol; n++)
        {
            toDestroy++;

            boardMatrix[startX, startY + n].Destroy();

            GameManager.Instance.UpdateScore(Model.POINTS_PER_ITEM);
        }
    }

    private void DestroyMatchInRow(int startX, int startY, int numberOfSameInRow)
    {
        GameManager.Instance.BlockScreen (true);

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


    #region Public Methods
    public void Init ()
    {
        InitBoard();

        PopMatches();
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