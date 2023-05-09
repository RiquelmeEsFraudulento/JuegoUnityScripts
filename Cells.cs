using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public int Row { get; set; }
    public int Column { get; set; }
    public bool IsVisited { get; set; }
    public bool IsEmpty { get; set; }
    public bool IsRock { get; set; }
    public bool IsLava { get; set; }
    public float Height { get; set; }
    public List<Cell> Neighbors { get; set; } // Updated line

    public Cell(int row, int column)
    {
        Row = row;
        Column = column;
        IsVisited = false;
        IsEmpty = true;
        IsRock = false;
        IsLava = false;
        Height = 0f;
        Neighbors = new List<Cell>(); // Initialize neighbors list
    }

    public Cell(int row, int column, bool isLava, float height)
    {
        Row = row;
        Column = column;
        IsVisited = false;
        IsEmpty = true;
        IsRock = false;
        IsLava = isLava;
        Height = height;
        Neighbors = new List<Cell>(); // Initialize neighbors list
    }

    public Cell(int row, int column, float height)
    {
        Row = row;
        Column = column;
        IsVisited = false;
        IsEmpty = true;
        IsRock = false;
        IsLava = false;
        Height = height;
        Neighbors = new List<Cell>(); // Initialize neighbors list
    }

    public int GetCellIndex(int numRows)
    {
        return Row * numRows + Column;
    }

    public bool IsCellInBounds(int numRows, int numColumns)
    {
        return Row >= 0 && Row < numRows && Column >= 0 && Column < numColumns;
    }

    public void PlaceRock()
    {
        IsRock = true;
        IsEmpty = false;
    }


    public void ConnectNeighbors(Cell[,] grid)
    {
        List<Cell> neighbors = new List<Cell>();
        bool includeDiagonals = true;

        // Check neighboring cells to the left, right, top, and bottom
        if (IsCellInBounds(grid.GetLength(0), grid.GetLength(1) - 1))
        {
            if (grid[Row, Column - 1] != null) Neighbors.Add(grid[Row, Column - 1]); // Left
            if (grid[Row, Column + 1] != null) Neighbors.Add(grid[Row, Column + 1]); // Right
            if (grid[Row - 1, Column] != null) Neighbors.Add(grid[Row - 1, Column]); // Top
            if (grid[Row + 1, Column] != null) Neighbors.Add(grid[Row + 1, Column]); // Bottom
        }

        if (includeDiagonals)
        {
            // Check neighboring cells diagonally
            if (IsCellInBounds(grid.GetLength(0) - 1, grid.GetLength(1) - 1))
            {
                if (grid[Row - 1, Column - 1] != null) Neighbors.Add(grid[Row - 1, Column - 1]); // Top Left
                if (grid[Row - 1, Column + 1] != null) Neighbors.Add(grid[Row - 1, Column + 1]); // Top Right
                if (grid[Row + 1, Column - 1] != null) Neighbors.Add(grid[Row + 1, Column - 1]); // Bottom Left
                if (grid[Row + 1, Column + 1] != null) Neighbors.Add(grid[Row + 1, Column + 1]); // Bottom Right
            }
        }
    }

}

