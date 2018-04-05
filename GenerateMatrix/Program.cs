using System;
using System.Collections.Generic;

namespace GenerateMatrix
{
    internal class Program
    {
        private static int rank = 0;
        private static Dictionary<int, string> keys = new Dictionary<int, string>();

        private static void Main(string[] args)
        {
            string alphabet = "";
            Console.WriteLine("Введите значение k");
            int.TryParse(Console.ReadLine(), out rank);
            bool isGray = false;
            Console.WriteLine("Использовать Код Грея? (y/n)");
            if (Console.ReadLine() == "y")
            {
                isGray = true;
            }
            Console.WriteLine("Введите алфавит множества через запятую, для пропуска шага, нажмите Enter");
            alphabet = Console.ReadLine();
            if (string.IsNullOrEmpty(alphabet))
            {
                for (int i = 0; i < rank; i++)
                {
                    keys.Add(i, Convert.ToString((char)(97 + i)));
                }
            }
            else
            {
                var alphakeys = alphabet.Split(',');
                if (alphakeys.Length != rank)
                {
                    Console.WriteLine("Длина алфавита не соответствует рангу");
                    return;
                }
                for (int i = 0; i < rank; i++)
                {
                    keys.Add(i, alphakeys[i]);
                }
            }

            int last = Convert.ToInt32(Math.Pow(2, rank));
            for (int i = 0; i < last; i++)
            {
                if (isGray)
                {
                    Write(UintToGray((uint)i));
                }
                else
                {
                    Write(i);
                }
            }
            Console.ReadLine();
        }

        private static void Write(int value)
        {
            string local = Convert.ToString(value, 2);
            string subset = "";
            bool emptySubsetFlag = true;
            while (local.Length < rank)
            {
                local = local.Insert(0, "0");
            }
            for (int i = 0; i < local.Length; i++)
            {
                if (local[i] == '1')
                {
                    if (emptySubsetFlag)
                    {
                        emptySubsetFlag = false;
                    }
                    subset += "{" + keys[i] + "} ";
                }
            }
            if (emptySubsetFlag)
            {
                subset = "{ Empty Subset }";
            }
            Console.WriteLine("{0} : {1}", local, subset);
        }

        private static void Write(uint value)
        {
            string local = Convert.ToString(value, 2);
            string subset = "";
            bool emptySubsetFlag = true;
            while (local.Length < rank)
            {
                local = local.Insert(0, "0");
            }
            for (int i = 0; i < local.Length; i++)
            {
                if (local[i] == '1')
                {
                    if (emptySubsetFlag)
                    {
                        emptySubsetFlag = false;
                    }
                    subset += "{" + keys[i] + "} ";
                }
            }
            if (emptySubsetFlag)
            {
                subset = "{ Empty Subset }";
            }
            Console.WriteLine("{0} : {1}", local, subset);
        }

        private static uint UintToGray(uint num)
        {
            return (num >> 1) ^ num;
        }
    }
}