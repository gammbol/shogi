using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shogi.UI
{
    public class ConsoleRenderer
    {
        private readonly Board _board;

        public ConsoleRenderer(Board board)
        {
            _board = board;
        }

        public void Render()
        {
            Console.WriteLine("   a b c d e f g h i");
            for (int row = 0; row < Board.Size; row++)
            {
                Console.Write($"{row + 1} ");
                for (int col = 0; col < Board.Size; col++)
                    Console.Write($" {_board.Cells[col, row]}");
                Console.WriteLine();
            }
        }
    }
}
