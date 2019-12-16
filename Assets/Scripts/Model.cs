using System; 
using UnityEngine;
/**
* Created by: Victoria Shenkevich
* Created on: 15/12/2019
*/

public static class Model 
{
    public const int BORDER_GAP = 100;
    public const float ITEM_SCALE = 0.9f;

    public const int ROWS = 7;
    public const int COLS = 7;

    public static int CellSize 
    { 
        get
        {
            int cellWidth = (Screen.width - BORDER_GAP * 2) / ROWS;
            int cellHeight = (Screen.height - BORDER_GAP * 2) / COLS;

            return Math.Min(cellWidth, cellWidth);
        }
    }

    public static float ItemSize 
    { 
        get
        {
            return CellSize * ITEM_SCALE;
        }
    }
}
