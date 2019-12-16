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
    private const int GAP = 100;
    #endregion


    #region Private Fields
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float gap;

	private int cellSize;
	private int toDestroy;
	private int destroyed;

    private int[,] boardMatrix;
    private GameObject[,] boardMatrixItems;
	#endregion


    #region Links
    [SerializeField] private Transform board;
    [SerializeField] private GameObject[] items;
    #endregion


    #region Unity Methods
    void Awake()
    {
        int cellWidth = (Screen.width - GAP)/ width;
        int cellHeight = (Screen.height - GAP)/ height;
        cellSize = Math.Min(cellWidth, cellWidth);
        gap = GAP * 1.5f;

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
        boardMatrix = new int[width, height];
        boardMatrixItems = new GameObject[width, height];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                FillCell(i, j);
            }
        }

        PrintMatrix("InitBoard");
    }

    private void FillCell(int i, int j)
    {
        int itemID = GetNewItem(items.Length);

        GameObject item = Instantiate(items[itemID - 1], board);
        InitItem(i, j, item);

        boardMatrix[i, j] = itemID;
        boardMatrixItems[i, j] = item;
    }

    private void InitItem(int i, int j, GameObject item)
    {
        item.name = "[" + i + "," + j + "]";
        item.transform.position = new Vector2(i * cellSize + gap, j * cellSize + gap);
        item.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize * 0.9f, cellSize * 0.9f);
        item.GetComponent<Item>().Init(OnItemDestroyed);
    }

    private static int GetNewItem(int itemsNumber)
    {
        return UnityEngine.Random.Range(0, itemsNumber) + 1;
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
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (boardMatrix[i, j] == EMPTY)
                {
                    int next = EMPTY;
                    int jOfNext = j;

                    while (next == EMPTY && jOfNext < height-1)
                    {
                        next = boardMatrix[i, ++jOfNext];
                    }

                    if (next != EMPTY) 
                    {
                        DropItem(i, j, jOfNext);
                    }
                    else //there is no more items in this vertical
                    {
                        for (int k = j; k < height; k++)
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

        GameObject item = boardMatrixItems[i, jOfNext];
        boardMatrixItems[i, j] = item;

        item.name = "[" + i + "," + j + "]";
        item.transform.position = new Vector2(i * cellSize + gap, j * cellSize + gap);

        boardMatrix[i, jOfNext] = EMPTY;
        boardMatrixItems[i, jOfNext] = null;
    }

    private void CheckForMatches ()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int onCheck = boardMatrix[i, j];

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

        for (int k = i + 1; k < width; k++)
        {
            if (boardMatrix[k, j] == onCheck)
                countH++;
            else
                break;
        }

        return countH;
    }

    private int CountSameV(int i, int j, int onCheck)
    {
        int countV = 1;

        for (int k = j + 1; k < height; k++)
        {
            if (boardMatrix[i, k] == onCheck)
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
            boardMatrixItems[i + n, j].GetComponent<Item>().Pop();

            boardMatrix[i + n, j] = EMPTY;
            boardMatrixItems[i + n, j] = null;
        }
    }

    private void DestroyMatchV(int i, int j, int countV)
    {
        for (int n = 0; n < countV; n++)
        {
            toDestroy++;
            boardMatrixItems[i, j+n].GetComponent<Item>().Pop();

            boardMatrix[i, j+n] = EMPTY;
            boardMatrixItems[i, j+n] = null;
        }
    }

    //For debug usage
    private void PrintMatrix (string methodName)
    {
        string matrixAsString = methodName + "\n";

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                matrixAsString += boardMatrix[i, j] + ", ";
            }

            matrixAsString += "\n";
        }

        Debug.Log(matrixAsString);
    }
    #endregion    
}