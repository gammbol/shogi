using System;
using System.Collections.Generic;

namespace shogi.Logic
{
    public static class PieceLogic
    {
        /// Проверяет, может ли фигура переместиться из одной позиции в другую
        public static bool CanMove(Piece[,] board, Piece piece, int fromX, int fromY, int toX, int toY)
        {
            // Проверка базовых условий
            if (!IsInBounds(fromX, fromY) || !IsInBounds(toX, toY))
                return false;

            if (piece == null || piece.Type == PieceType.None)
                return false;

            // Нельзя ходить на свою фигуру
            Piece target = board[toX, toY];
            if (target != null && target.Owner == piece.Owner)
                return false;

            // Выбор логики в зависимости от типа фигуры
            return piece.Type switch
            {
                PieceType.Pawn => CanPawnMove(piece, fromX, fromY, toX, toY),
                PieceType.Lance => CanLanceMove(board, piece, fromX, fromY, toX, toY),
                PieceType.Knight => CanKnightMove(piece, fromX, fromY, toX, toY),
                PieceType.Silver => CanSilverMove(piece, fromX, fromY, toX, toY),
                PieceType.Gold => CanGoldMove(piece, fromX, fromY, toX, toY),
                PieceType.Bishop => CanBishopMove(board, fromX, fromY, toX, toY),
                PieceType.Rook => CanRookMove(board, fromX, fromY, toX, toY),
                PieceType.King => CanKingMove(fromX, fromY, toX, toY),
                _ => false
            };
        }

        // 1. ПЕШКА (Фухё)
        private static bool CanPawnMove(Piece piece, int fromX, int fromY, int toX, int toY)
        {
            int direction = (piece.Owner == Player.Black) ? 1 : -1;
            return toX == fromX && toY == fromY + direction;
        }

        // 2. КОПЬЕ (Кёся)
        private static bool CanLanceMove(Piece[,] board, Piece piece, int fromX, int fromY, int toX, int toY)
        {
            if (toX != fromX)
                return false;

            int direction = (piece.Owner == Player.Black) ? 1 : -1;

            if ((piece.Owner == Player.Black && toY <= fromY) ||
                (piece.Owner == Player.White && toY >= fromY))
                return false;

            return IsPathClear(board, fromX, fromY, toX, toY);
        }

        // 3. КОНЬ (Кэйма)
        private static bool CanKnightMove(Piece piece, int fromX, int fromY, int toX, int toY)
        {
            int direction = (piece.Owner == Player.Black) ? 1 : -1;
            int targetY = fromY + 2 * direction;

            return (toY == targetY && (toX == fromX - 1 || toX == fromX + 1));
        }

        // 4. СЕРЕБРО (Гинсё)
        private static bool CanSilverMove(Piece piece, int fromX, int fromY, int toX, int toY)
        {
            int direction = (piece.Owner == Player.Black) ? 1 : -1;

            int dx = Math.Abs(toX - fromX);
            int dy = toY - fromY;

            if (dx > 1 || Math.Abs(dy) > 1)
                return false;

            if (dx == 0 && dy == -direction)
                return false;

            return true;
        }

        // 5. ЗОЛОТО (Кинсё)
        private static bool CanGoldMove(Piece piece, int fromX, int fromY, int toX, int toY)
        {
            int direction = (piece.Owner == Player.Black) ? 1 : -1;

            int dx = Math.Abs(toX - fromX);
            int dy = toY - fromY;

            if (dx > 1 || Math.Abs(dy) > 1)
                return false;

            if (dx == 1 && dy == -direction)
                return false;

            return true;
        }

        // 6. СЛОН (Какугё)
        private static bool CanBishopMove(Piece[,] board, int fromX, int fromY, int toX, int toY)
        {
            int dx = Math.Abs(toX - fromX);
            int dy = Math.Abs(toY - fromY);

            if (dx != dy)
                return false;

            return IsPathClear(board, fromX, fromY, toX, toY);
        }

        // 7. ЛАДЬЯ (Хися)
        private static bool CanRookMove(Piece[,] board, int fromX, int fromY, int toX, int toY)
        {
            if (fromX != toX && fromY != toY)
                return false;

            return IsPathClear(board, fromX, fromY, toX, toY);
        }

        // 8. КОРОЛЬ (Осё/Гёку)
        private static bool CanKingMove(int fromX, int fromY, int toX, int toY)
        {
            int dx = Math.Abs(toX - fromX);
            int dy = Math.Abs(toY - fromY);

            return dx <= 1 && dy <= 1;
        }

        /// Проверяет, свободен ли путь между двумя клетками
        private static bool IsPathClear(Piece[,] board, int fromX, int fromY, int toX, int toY)
        {
            int dx = Math.Sign(toX - fromX);
            int dy = Math.Sign(toY - fromY);

            int x = fromX + dx;
            int y = fromY + dy;

            while (x != toX || y != toY)
            {
                if (board[x, y] != null && board[x, y].Type != PieceType.None)
                    return false;

                x += dx;
                y += dy;
            }

            return true;
        }

        public static bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < 9 && y >= 0 && y < 9;
        }
    }
}