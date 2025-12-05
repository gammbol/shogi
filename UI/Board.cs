using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shogi
{
    public class Board
    {
        public const int Size = 9;
        private readonly string[,] _cells = new string[Size, Size];
        public string[,] Cells => _cells;

        public void Init()
        {
            for (int row = 0; row < Size; row++)
                for (int col = 0; col < Size; col++)
                    _cells[row, col] = ".";
        }

        public void Render()
        {
            Console.WriteLine("   a b c d e f g h i");
            for (int row = 0; row < Size; row++)
            {
                Console.Write($"{row + 1} ");
                for (int col = 0; col < Size; col++)
                    Console.Write($" {_cells[row, col]}");
                Console.WriteLine();
            }
        }
    }
}
