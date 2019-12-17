/**
* Created by: Victoria Shenkevich
* Created on: 15/12/2019
*/

using System; 
using UnityEngine;

public static class Model 
{
    public const int BORDER_GAP = 100;
    public const int TEXT_GAP = 100;
    public const float ITEM_SCALE = 0.9f;

    public const int ROWS = 7;
    public const int COLS = 7;

    public const int POINTS_PER_ITEM = 10;

    public static int CellSize 
    { 
        get
        {
            int cellWidth = (Screen.width - BORDER_GAP * 2) / COLS;
            int cellHeight = (Screen.height - BORDER_GAP * 2 - TEXT_GAP) / ROWS;

            return Math.Min(cellWidth, cellHeight);
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
