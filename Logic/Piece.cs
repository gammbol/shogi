using System;
using System.Collections.Generic;

namespace shogi  
{
    public enum PieceType
    {
        None = 0,
        Pawn,
        Lance,
        Knight,
        Silver,
        Gold,
        Bishop,
        Rook,
        King
    }

    public enum Player
    {
        None = 0,
        Black,
        White
    }

    public class Piece
    {
        public PieceType Type { get; set; }
        public Player Owner { get; set; }
        public bool Promoted { get; set; }

        public Piece(PieceType type, Player owner)
        {
            Type = type;
            Owner = owner;
            Promoted = false;
        }
    }
}