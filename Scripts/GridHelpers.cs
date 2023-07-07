public static class GridHelpers
{
    public static readonly int[] NeighborX = { 1, 0, -1, 0 };
    public static readonly int[] NeighborY = { 0, 1, 0, -1 };
    
    public static bool IsCellInBounds(int x, int y, int width, int height)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

}