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

    private void FillCell(int i, int j)
    {
        boardMatrix[i, j] = new CellData(itemsPrefabs, board, new Vector2(i, j), OnItemDestroyed);
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
                    int jOfNext = j;

                    while (next == EMPTY && jOfNext < Model.COLS-1)
                    {
                        next = boardMatrix[i, ++jOfNext].type;
                    }

                    if (next != EMPTY) 
                    {
                        DropItem(i, j, jOfNext);
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

    private void DropItem(int i, int j, int jOfNext)
    {
        boardMatrix[i, j] = boardMatrix[i, jOfNext];
        boardMatrix[i, j].item.UpdateCoordinates(new Vector2(i, j));

        boardMatrix[i, jOfNext].Empty();
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
                    int countH = CountSameH(i, j, onCheck);
                    int countV = CountSameV(i, j, onCheck);

                    if (countH >= MATCH_MIN)
                    {
                        DestroyMatchH(i, j, countH);
                    }

                    if (countV >= MATCH_MIN)
                    {
                        if (countH >= MATCH_MIN)
                        {
                            DestroyMatchV(i, j + 1, countV - 1);
                        }
                        else
                        {
                            DestroyMatchV(i, j, countV);
                        }
                    }
                }
            }
        }
    }

    private int CountSameH(int i, int j, int onCheck)
    {
        int countH = 1;

        for (int k = i + 1; k < Model.ROWS; k++)
        {
            if (boardMatrix[k, j].type == onCheck)
                countH++;
            else
                break;
        }

        return countH;
    }

    private int CountSameV(int i, int j, int onCheck)
    {
        int countV = 1;

        for (int k = j + 1; k < Model.COLS; k++)
        {
            if (boardMatrix[i, k].type == onCheck)
                countV++;
            else
                break;
        }

        return countV;
    }

    private void DestroyMatchH(int i, int j, int countH)
    {
        for (int n = 0; n < countH; n++)
        {
            toDestroy++;

            boardMatrix[i + n, j].Destroy();
        }
    }

    private void DestroyMatchV(int i, int j, int countV)
    {
        for (int n = 0; n < countV; n++)
        {
            toDestroy++;

            boardMatrix[i, j + n].Destroy();
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