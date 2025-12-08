using System;
using shogi;
using shogi.Logic;

namespace shogi.UI
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

        public void UpdateFromGameEngine(Piece[,] pieces)
        {
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    var piece = pieces[col, row];
                    _cells[row, col] = PieceToSymbol(piece);
                }
            }
        }

        private string PieceToSymbol(Piece piece)
        {
            if (piece == null || piece.Type == PieceType.None)
                return ".";

            char symbol = piece.Type switch
            {
                PieceType.Pawn => 'P',
                PieceType.Lance => 'L',
                PieceType.Knight => 'N',
                PieceType.Silver => 'S',
                PieceType.Gold => 'G',
                PieceType.Bishop => 'B',
                PieceType.Rook => 'R',
                PieceType.King => 'K',
                _ => '.'
            };

            if (piece.Promoted)
                symbol = '+';

            return piece.Owner == Player.Black ?
                   char.ToUpper(symbol).ToString() :
                   char.ToLower(symbol).ToString();
        }

        public void Render()
        {
            Console.WriteLine("  a b c d e f g h i");
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