using System;
using System.Collections.Generic;
using shogi.Data;

namespace shogi.Logic
{
    public class GameEngine
    {
        public Piece[,] Board { get; private set; }
        public Player CurrentPlayer { get; private set; }
        public bool IsFinished { get; private set; }
        public int Score { get; private set; }
        private GameStorage Storage;

        // ИСПРАВЛЕНО: Убраны неиспользуемые параметры из конструктора
        public GameEngine()
        {
            Board = new Piece[9, 9];
            CurrentPlayer = Player.Black;
            IsFinished = false;
            Score = 0;
            Storage = new GameStorage("savefile.txt");
        }

        // Добавлен конструктор с параметрами для загрузки
        public GameEngine(Piece[,] board, Player currentPlayer, int score)
        {
            Board = board;
            CurrentPlayer = currentPlayer;
            IsFinished = false;
            Score = score;
            Storage = new GameStorage("savefile.txt");
        }

        public void Init()
        {
            // Очищаем доску
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    Board[x, y] = new Piece(PieceType.None, Player.None);
                }
            }

            // Расставляем фигуры черных (снизу, y = 0-2)
            // 1-й ряд (y=2)
            Board[0, 2] = new Piece(PieceType.Lance, Player.Black);
            Board[1, 2] = new Piece(PieceType.Knight, Player.Black);
            Board[2, 2] = new Piece(PieceType.Silver, Player.Black);
            Board[3, 2] = new Piece(PieceType.Gold, Player.Black);
            Board[4, 2] = new Piece(PieceType.King, Player.Black);
            Board[5, 2] = new Piece(PieceType.Gold, Player.Black);
            Board[6, 2] = new Piece(PieceType.Silver, Player.Black);
            Board[7, 2] = new Piece(PieceType.Knight, Player.Black);
            Board[8, 2] = new Piece(PieceType.Lance, Player.Black);

            // 2-й ряд (y=1)
            Board[1, 1] = new Piece(PieceType.Bishop, Player.Black);
            Board[7, 1] = new Piece(PieceType.Rook, Player.Black);

            // 3-й ряд (y=0) - пешки
            for (int x = 0; x < 9; x++)
            {
                Board[x, 0] = new Piece(PieceType.Pawn, Player.Black);
            }

            // Расставляем фигуры белых (сверху, y = 6-8)
            // 7-й ряд (y=6)
            Board[0, 6] = new Piece(PieceType.Lance, Player.White);
            Board[1, 6] = new Piece(PieceType.Knight, Player.White);
            Board[2, 6] = new Piece(PieceType.Silver, Player.White);
            Board[3, 6] = new Piece(PieceType.Gold, Player.White);
            Board[4, 6] = new Piece(PieceType.King, Player.White);
            Board[5, 6] = new Piece(PieceType.Gold, Player.White);
            Board[6, 6] = new Piece(PieceType.Silver, Player.White);
            Board[7, 6] = new Piece(PieceType.Knight, Player.White);
            Board[8, 6] = new Piece(PieceType.Lance, Player.White);

            // 8-й ряд (y=7)
            Board[7, 7] = new Piece(PieceType.Bishop, Player.White);
            Board[1, 7] = new Piece(PieceType.Rook, Player.White);

            // 9-й ряд (y=8) - пешки
            for (int x = 0; x < 9; x++)
            {
                Board[x, 8] = new Piece(PieceType.Pawn, Player.White);
            }
        }

        // ИСПРАВЛЕНО: Убрал старый метод Init с параметрами, теперь есть отдельный конструктор
        // Метод для инициализации из сохранения (оставляем для обратной совместимости)
        public void Init(Piece[,] board, Player currentPlayer, int score)
        {
            Board = board;
            CurrentPlayer = currentPlayer;
            Score = score;
        }

        public async Task<bool> TryMakeMove(string input)
        {
            if (IsFinished)
                return false;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            // команды тут больше не обрабатываем
            // SAVE обрабатывается в UI

            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4)
                return false;

            // формат: 5 c 5 d
            if (!int.TryParse(parts[0], out int fromX))
                return false;

            if (!TryParseLetter(parts[1], out int fromY))
                return false;

            if (!int.TryParse(parts[2], out int toX))
                return false;

            if (!TryParseLetter(parts[3], out int toY))
                return false;

            // диапазон
            if (fromX < 1 || fromX > 9 || toX < 1 || toX > 9)
                return false;

            // приводим к индексам массива
            fromX--;
            fromY--;
            toX--;
            toY--;

            if (!IsValidMove(fromX, fromY, toX, toY))
                return false;

            ExecuteMove(fromX, fromY, toX, toY);
            CheckGameEnd();

            return true;
        }

        private bool TryParseLetter(string value, out int number)
        {
            number = 0;

            if (string.IsNullOrWhiteSpace(value) || value.Length != 1)
                return false;

            char c = char.ToLower(value[0]);

            if (c < 'a' || c > 'i')
                return false;

            number = (c - 'a') + 1; // a = 1, b = 2, ..., i = 9
            return true;
        }



        private bool IsValidMove(int fromX, int fromY, int toX, int toY)
        {
            // Базовые проверки
            // ИСПРАВЛЕНО: Заменил | на ||
            if (!PieceLogic.IsInBounds(fromX, fromY) ||
                !PieceLogic.IsInBounds(toX, toY))
                return false;

            Piece piece = Board[fromX, fromY];
            if (piece == null || piece.Type == PieceType.None)
                return false;

            // Не свой ход
            if (piece.Owner != CurrentPlayer)
                return false;

            // Нельзя ходить в ту же клетку
            if (fromX == toX && fromY == toY)
                return false;

            Piece target = Board[toX, toY];
            if (target != null && target.Type != PieceType.None && target.Owner == CurrentPlayer)
                return false;

            // Проверяем логику фигуры
            if (!PieceLogic.CanMove(Board, piece, fromX, fromY, toX, toY))
                return false;

            // Временная проверка шаха (упрощенная)
            // Можно доработать позже
            return true;
        }

        private void ExecuteMove(int fromX, int fromY, int toX, int toY)
        {
            if (IsFinished)
                return;
            Piece piece = Board[fromX, fromY];
            if (piece == null || piece.Type == PieceType.None)
                return;

            // Если взяли фигуру - увеличиваем счет
            Piece target = Board[toX, toY];
            if (target != null && target.Type != PieceType.None && target.Owner != piece.Owner)
            {
                if (target.Type == PieceType.King)
                {
                    IsFinished = true;
                    return;
                }

                Score += GetPieceValue(target.Type);
            }

            // Перемещаем фигуру
            Board[toX, toY] = piece;
            Board[fromX, fromY] = new Piece(PieceType.None, Player.None);

            // Проверяем превращение (автоматическое при входе в зону превращения)
            // ИСПРАВЛЕНО: Добавил проверку на null
            if (piece != null)
            {
                CheckPromotion(piece, toX, toY);
            }

            // Меняем игрока
            CurrentPlayer = (CurrentPlayer == Player.Black) ? Player.White : Player.Black;
        }

        private void CheckPromotion(Piece piece, int x, int y)
        {
            // Зона превращения: последние 3 ряда для каждого игрока
            // ИСПРАВЛЕНО: Заменил | на ||
            bool inPromotionZone = (piece.Owner == Player.Black && y >= 6) ||
                                   (piece.Owner == Player.White && y <= 2);

            if (piece.Type == PieceType.King || piece.Type == PieceType.Gold)
                return;

            if (inPromotionZone && !piece.Promoted)
            {
                piece.Promoted = true;
            }

        }

        private bool CanPromote(PieceType type)
        {
            // Король и золото не превращаются
            return type != PieceType.King && type != PieceType.Gold;
        }

        private int GetPieceValue(PieceType type)
        {
            return type switch
            {
                PieceType.Pawn => 1,
                PieceType.Lance => 3,
                PieceType.Knight => 4,
                PieceType.Silver => 5,
                PieceType.Gold => 6,
                PieceType.Bishop => 8,
                PieceType.Rook => 10,
                PieceType.King => 0, // Короля нельзя взять
                _ => 0
            };
        }

        private void CheckGameEnd()
        {
            // Упрощенная проверка - если взят король
            bool blackKingFound = false;
            bool whiteKingFound = false;

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    var piece = Board[x, y];
                    if (piece != null && piece.Type == PieceType.King)
                    {
                        if (piece.Owner == Player.Black)
                            blackKingFound = true;
                        else if (piece.Owner == Player.White)
                            whiteKingFound = true;

                        if (blackKingFound && whiteKingFound)
                            return;
                    }
                }
            }


            if (!blackKingFound || !whiteKingFound)
            {
                IsFinished = true;
            }
        }
    }
}