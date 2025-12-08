using System;
using shogi.Logic;

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
            _board.Render();
        }

        public void UpdateBoard(Piece[,] pieces)
        {
            _board.UpdateFromGameEngine(pieces);
        }
    }
}