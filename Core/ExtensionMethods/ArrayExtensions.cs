namespace Brupper;

public static class ArrayExtensions
{
    public static int[,] ConvertTo2DArray(this int[][] jaggedArray, int numOfColumns, int numOfRows)
    {
        int[,] temp2DArray = new int[numOfColumns, numOfRows];

        for (int c = 0; c < numOfColumns; c++)
            for (int r = 0; r < numOfRows; r++)
                temp2DArray[c, r] = jaggedArray[c][r];

        return temp2DArray;
    }


    public static int[][] ConvertToJaggedArray(this int[,] multiArray, int numOfColumns, int numOfRows)
    {
        int[][] jaggedArray = new int[numOfColumns][];

        for (int c = 0; c < numOfColumns; c++)
        {
            jaggedArray[c] = new int[numOfRows];
            for (int r = 0; r < numOfRows; r++)
            {
                jaggedArray[c][r] = multiArray[c, r];
            }
        }

        return jaggedArray;
    }
}
