using System;

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
            // просто вызываем метод Render у Board
            _board.Render();
        }

        // метод для обновления доски из GameEngine
        public void UpdateBoard(Piece[,] pieces)
        {
            _board.UpdateFromGameEngine(pieces);
        }
    }
}