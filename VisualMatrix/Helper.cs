using System;

namespace VisualGraph
{
    public static class Helper
    {
        internal static bool All(this bool[,] matrix, Func<bool, bool> func)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (!func(matrix[i, j]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}