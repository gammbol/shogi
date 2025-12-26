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
            Board[0, 0] = new Piece(PieceType.Lance, Player.Black);
            Board[1, 0] = new Piece(PieceType.Knight, Player.Black);
            Board[2, 0] = new Piece(PieceType.Silver, Player.Black);
            Board[3, 0] = new Piece(PieceType.Gold, Player.Black);
            Board[4, 0] = new Piece(PieceType.King, Player.Black);
            Board[5, 0] = new Piece(PieceType.Gold, Player.Black);
            Board[6, 0] = new Piece(PieceType.Silver, Player.Black);
            Board[7, 0] = new Piece(PieceType.Knight, Player.Black);
            Board[8, 0] = new Piece(PieceType.Lance, Player.Black);

            // 2-й ряд (y=1)
            Board[1, 1] = new Piece(PieceType.Bishop, Player.Black);
            Board[7, 1] = new Piece(PieceType.Rook, Player.Black);

            // 3-й ряд (y=0) - пешки
            for (int x = 0; x < 9; x++)
            {
                Board[x, 2] = new Piece(PieceType.Pawn, Player.Black);
            }

            // Расставляем фигуры белых (сверху, y = 6-8)
            // 7-й ряд (y=6)
            Board[0, 8] = new Piece(PieceType.Lance, Player.White);
            Board[1, 8] = new Piece(PieceType.Knight, Player.White);
            Board[2, 8] = new Piece(PieceType.Silver, Player.White);
            Board[3, 8] = new Piece(PieceType.Gold, Player.White);
            Board[4, 8] = new Piece(PieceType.King, Player.White);
            Board[5, 8] = new Piece(PieceType.Gold, Player.White);
            Board[6, 8] = new Piece(PieceType.Silver, Player.White);
            Board[7, 8] = new Piece(PieceType.Knight, Player.White);
            Board[8, 8] = new Piece(PieceType.Lance, Player.White);

            // 8-й ряд (y=7)
            Board[7, 7] = new Piece(PieceType.Bishop, Player.White);
            Board[1, 7] = new Piece(PieceType.Rook, Player.White);

            // 9-й ряд (y=8) - пешки
            for (int x = 0; x < 9; x++)
            {
                Board[x, 6] = new Piece(PieceType.Pawn, Player.White);
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

            // Проверяем, не оставляет ли ход своего короля под шахом
            if (WouldLeaveKingInCheck(fromX, fromY, toX, toY, piece.Owner))
                return false;

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
                    Console.WriteLine("\n=== КОРОЛЬ ВЗЯТ! ===");
                    Console.WriteLine("Игра окончена.");
                    return;
                }

                Score += GetPieceValue(target.Type);
            }

            // Перемещаем фигуру
            Board[toX, toY] = piece;
            Board[fromX, fromY] = new Piece(PieceType.None, Player.None);

            // Проверяем превращение (автоматическое при входе в зону превращения)
            if (piece != null)
            {
                CheckPromotion(piece, toX, toY);
            }

            // Меняем игрока
            CurrentPlayer = (CurrentPlayer == Player.Black) ? Player.White : Player.Black;

            // Проверяем окончание игры
            CheckGameEnd();
        }

        private void CheckPromotion(Piece piece, int x, int y)
        {
            // Зона превращения: последние 3 ряда для каждого игрока
            bool inPromotionZone = (piece.Owner == Player.Black && y >= 6) ||
                                   (piece.Owner == Player.White && y <= 2);

            if (piece.Type == PieceType.King || piece.Type == PieceType.Gold)
                return;

            if (inPromotionZone && !piece.Promoted)
            {
                piece.Promoted = true;
            }
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

        // ========== НОВЫЕ МЕТОДЫ ДЛЯ ПРОВЕРКИ ШАХА И МАТА ==========

        // 1. Найти позицию короля
        private (int x, int y)? FindKingPosition(Player kingOwner)
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    var piece = Board[x, y];
                    if (piece != null &&
                        piece.Type == PieceType.King &&
                        piece.Owner == kingOwner)
                    {
                        return (x, y);
                    }
                }
            }
            return null; // Король не найден (уже взят)
        }

        // 2. Проверка шаха (король под атакой)
        private bool IsKingInCheck(Player kingOwner)
        {
            var kingPos = FindKingPosition(kingOwner);
            if (!kingPos.HasValue) return false;

            int kingX = kingPos.Value.x;
            int kingY = kingPos.Value.y;

            // Проверяем все фигуры противника
            Player opponent = (kingOwner == Player.Black) ? Player.White : Player.Black;

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    var piece = Board[x, y];
                    if (piece != null && piece.Owner == opponent)
                    {
                        if (PieceLogic.CanMove(Board, piece, x, y, kingX, kingY))
                        {
                            return true; // Шах!
                        }
                    }
                }
            }

            return false;
        }

        // 3. Проверка, не оставляет ли ход своего короля под шахом
        private bool WouldLeaveKingInCheck(int fromX, int fromY, int toX, int toY, Player player)
        {
            // Сохраняем исходное состояние
            Piece originalPiece = Board[fromX, fromY];
            Piece originalTarget = Board[toX, toY];

            // Симулируем ход
            Board[toX, toY] = originalPiece;
            Board[fromX, fromY] = new Piece(PieceType.None, Player.None);

            // Проверяем, остался ли король под шахом
            bool kingInCheck = IsKingInCheck(player);

            // Возвращаем всё на место
            Board[fromX, fromY] = originalPiece;
            Board[toX, toY] = originalTarget;

            return kingInCheck;
        }

        // 4. Проверка всех возможных ходов для игрока
        private bool HasAnyValidMove(Player player)
        {
            // Проверяем все фигуры игрока
            for (int fromX = 0; fromX < 9; fromX++)
            {
                for (int fromY = 0; fromY < 9; fromY++)
                {
                    var piece = Board[fromX, fromY];
                    if (piece != null && piece.Owner == player)
                    {
                        // Проверяем все возможные клетки для хода
                        for (int toX = 0; toX < 9; toX++)
                        {
                            for (int toY = 0; toY < 9; toY++)
                            {
                                // Пропускаем если та же клетка
                                if (fromX == toX && fromY == toY) continue;

                                // Проверяем валидность хода
                                if (IsValidMoveForCheck(fromX, fromY, toX, toY, player))
                                {
                                    return true; // Есть хотя бы один возможный ход
                                }
                            }
                        }
                    }
                }
            }

            return false; // Нет ни одного возможного хода
        }

        // 5. Проверка валидности хода для проверки мата
        private bool IsValidMoveForCheck(int fromX, int fromY, int toX, int toY, Player player)
        {
            // Базовые проверки
            if (!PieceLogic.IsInBounds(fromX, fromY) || !PieceLogic.IsInBounds(toX, toY))
                return false;

            Piece piece = Board[fromX, fromY];
            if (piece == null || piece.Type == PieceType.None || piece.Owner != player)
                return false;

            // Нельзя ходить на свою фигуру
            Piece target = Board[toX, toY];
            if (target != null && target.Type != PieceType.None && target.Owner == player)
                return false;

            // Проверяем логику фигуры
            if (!PieceLogic.CanMove(Board, piece, fromX, fromY, toX, toY))
                return false;

            // Симулируем ход
            Piece originalTarget = Board[toX, toY];
            Board[toX, toY] = piece;
            Board[fromX, fromY] = new Piece(PieceType.None, Player.None);

            // Проверяем остался ли король под шахом после хода
            bool stillInCheck = IsKingInCheck(player);

            // Возвращаем все на место
            Board[fromX, fromY] = piece;
            Board[toX, toY] = originalTarget;

            return !stillInCheck; // Ход валиден если после него нет шаха
        }

        // 6. Проверка мата
        private bool IsCheckmate(Player kingOwner)
        {
            // Если король не под шахом - не мат
            if (!IsKingInCheck(kingOwner))
                return false;

            // Если есть хоть один валидный ход - не мат
            if (HasAnyValidMove(kingOwner))
                return false;

            return true; // Мат!
        }

        // 7. Основная проверка окончания игры
        private void CheckGameEnd()
        {
            // Проверка на мат
            if (IsCheckmate(Player.Black))
            {
                IsFinished = true;
                Console.WriteLine("\n=== МАТ ЧЕРНОМУ КОРОЛЮ! ===");
                Console.WriteLine("Белые победили!");
                return;
            }

            if (IsCheckmate(Player.White))
            {
                IsFinished = true;
                Console.WriteLine("\n=== МАТ БЕЛОМУ КОРОЛЮ! ===");
                Console.WriteLine("Черные победили!");
                return;
            }

            // Старая проверка на взятие короля
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
                    }
                }
            }

            if (!blackKingFound || !whiteKingFound)
            {
                IsFinished = true;
                if (!blackKingFound && !whiteKingFound)
                    Console.WriteLine("\n=== ОБА КОРОЛЯ ВЗЯТЫ! ===");
                else if (!blackKingFound)
                    Console.WriteLine("\n=== ЧЕРНЫЙ КОРОЛЬ ВЗЯТ! ===");
                else
                    Console.WriteLine("\n=== БЕЛЫЙ КОРОЛЬ ВЗЯТ! ===");
                Console.WriteLine("Игра окончена.");
            }
        }
    }
}