/**
 * Created by: Victoria Shenkevich
 * Created on: 15/12/2019
 */

using System;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    #region Constants
    private const int EMPTY = 0;
    private const int GAP = 100;
    #endregion


    #region Private Fields
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float gap;

	private int cellSize;
	
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
    }

    private static int GetNewItem(int itemsNumber)
    {
        return UnityEngine.Random.Range(0, itemsNumber) + 1;
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