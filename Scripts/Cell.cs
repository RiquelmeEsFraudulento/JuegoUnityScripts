using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public int Row { get; set; }
    public int Column { get; set; }
    public bool IsVisited { get; set; }
    public bool IsEmpty { get; set; }
    public bool IsRock { get; set; }
    public bool isLava { get; set; }
    public float Height { get; set; }
    public List<Cell> Neighbors { get; set; } // Updated line

    public Cell(int row, int column)
    {
        Row = row;
        Column = column;
        IsVisited = false;
        IsEmpty = true;
        IsRock = false;
        isLava = false;
        Height = 0f;
        Neighbors = new List<Cell>(); // Initialize neighbors list
    }

    public Cell(int row, int column, bool IsLava, float height)
    {
        Row = row;
        Column = column;
        IsVisited = false;
        IsEmpty = true;
        IsRock = false;
        isLava = IsLava;
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
        isLava = false;
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
            if (Column > 0 && grid[Row, Column - 1] != null) neighbors.Add(grid[Row, Column - 1]); // Left
            if (Column < grid.GetLength(1) - 1 && grid[Row, Column + 1] != null) neighbors.Add(grid[Row, Column + 1]); // Right
            if (Row > 0 && grid[Row - 1, Column] != null) neighbors.Add(grid[Row - 1, Column]); // Top
            if (Row < grid.GetLength(0) - 1 && grid[Row + 1, Column] != null) neighbors.Add(grid[Row + 1, Column]); // Bottom
        }

        if (includeDiagonals)
        {
            // Check neighboring cells diagonally
            if (IsCellInBounds(grid.GetLength(0) - 1, grid.GetLength(1) - 1))
            {
                if (Row > 0 && Column > 0 && grid[Row - 1, Column - 1] != null) neighbors.Add(grid[Row - 1, Column - 1]); // Top Left
                if (Row > 0 && Column < grid.GetLength(1) - 1 && grid[Row - 1, Column + 1] != null) neighbors.Add(grid[Row - 1, Column + 1]); // Top Right
                if (Row < grid.GetLength(0) - 1 && Column > 0 && grid[Row + 1, Column - 1] != null) neighbors.Add(grid[Row + 1, Column - 1]); // Bottom Left
                if (Row < grid.GetLength(0) - 1 && Column < grid.GetLength(1) - 1 && grid[Row + 1, Column + 1] != null) neighbors.Add(grid[Row + 1, Column + 1]); // Bottom Right
            }
        }

        Neighbors = neighbors;
    }

 

}

