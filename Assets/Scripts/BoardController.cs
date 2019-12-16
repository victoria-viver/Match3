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

    void Start()
    {
        
    }

    void Update()
    {
        
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
        boardMatrix = new CellData[Model.ROWS, Model.COLS];

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
        boardMatrix[x, y] = new CellData(itemsPrefabs, board, new Vector2(x, y), OnItemDestroyed);
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

                    while (next == EMPTY && yOfNext < Model.COLS-1)
                    {
                        next = boardMatrix[i, ++yOfNext].type;
                    }

                    if (next != EMPTY) 
                    {
                        DropItem(i, j, yOfNext);
                    }
                    else //there is no more items in this vertical
                    {
                        for (int k = j; k < Model.COLS; k++)
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
        for (int i = 0; i < Model.ROWS; i++)
        {
            for (int j = 0; j < Model.COLS; j++)
            {
                int onCheck = boardMatrix[i, j].type;

                if (onCheck != EMPTY)
                {
                    int numberOfSameInRow = CountSameInRow(i, j, onCheck);
                    int numberOfSameInCol = CountSameInColumn(i, j, onCheck);

                    if (numberOfSameInRow >= MATCH_MIN)
                    {
                        DestroyMatchInRow(i, j, numberOfSameInRow);
                    }

                    if (numberOfSameInCol >= MATCH_MIN)
                    {
                        if (numberOfSameInRow >= MATCH_MIN)
                        {
                            DestroyMatchInColumn(i, j + 1, numberOfSameInCol - 1);
                        }
                        else
                        {
                            DestroyMatchInColumn(i, j, numberOfSameInCol);
                        }
                    }
                }
            }
        }
    }

    private int CountSameInRow(int startX, int startY, int onCheck)
    {
        int numberOfSameInRow = 1;

        for (int k = startX + 1; k < Model.ROWS; k++)
        {
            if (boardMatrix[k, startY].type == onCheck)
                numberOfSameInRow++;
            else
                break;
        }

        return numberOfSameInRow;
    }

    private int CountSameInColumn(int startX, int startY, int onCheck)
    {
        int numberOfSameInCol = 1;

        for (int k = startY + 1; k < Model.COLS; k++)
        {
            if (boardMatrix[startX, k].type == onCheck)
                numberOfSameInCol++;
            else
                break;
        }

        return numberOfSameInCol;
    }

    private void DestroyMatchInRow(int startX, int startY, int numberOfSameInRow)
    {
        for (int n = 0; n < numberOfSameInRow; n++)
        {
            toDestroy++;

            boardMatrix[startX + n, startY].Destroy();
        }
    }

    private void DestroyMatchInColumn(int startX, int startY, int numberOfSameInCol)
    {
        for (int n = 0; n < numberOfSameInCol; n++)
        {
            toDestroy++;

            boardMatrix[startX, startY + n].Destroy();
        }
    }
    
    //For debug usage
    private void PrintMatrix (string methodName)
    {
        string matrixAsString = methodName + "\n";

        for (int i = 0; i < Model.ROWS; i++)
        {
            for (int j = 0; j < Model.COLS; j++)
            {
                matrixAsString += boardMatrix[i, j].type + ", ";
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

        public CellData (GameObject[] itemsPrefabs, Transform parent, Vector2 coordinates, Action OnDestroyCallback)
        {
            type = UnityEngine.Random.Range(0, itemsPrefabs.Length) + 1;

            item = Instantiate(itemsPrefabs[type - 1], parent).GetComponent<Item>();
            item.Init(coordinates, OnDestroyCallback);
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