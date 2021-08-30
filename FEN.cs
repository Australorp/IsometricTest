using System;
using System.Collections.Generic;

using static PieceMoves.PieceMoveGen;
using Types;

namespace FEN {

    class FENReader {

        static Side SideFromCharCase(Char ch) {
            if (Char.IsUpper(ch)) {
                return Side.White;
            }
            else {
                return Side.Black;
            }
        }

        public static List<Square> CreateSquares() {
            List<Square> output = new List<Square>();
            for (int y = 1; y < 9; y++) {
                for (int x = 1; x < 9; x++) {
                    output.Add(new Square(
                        x switch {
                            1 => "a",
                            2 => "b",
                            3 => "c",
                            4 => "d",
                            5 => "e",
                            6 => "f",
                            7 => "g",
                            8 => "h",
                            _ => throw new ArgumentException("Unexpected integer", nameof(x))
                        } + y.ToString(),
                        new Coord(x, y)
                    ));
                }
            }
            return output;
        }

        public static List<Piece> ReadFEN(string fen) {
            List<Piece> output = new List<Piece>();
            int y = 8;
            foreach (string rank in fen.Split('/')) {
                int x = 1;
                foreach (Char ch in rank) {
                    if (Char.IsLetter(ch)) {
                        output.Add(
                            (Char.ToLower(ch)) switch {
                                'p' => new Piece("Pawn", ch, new Coord(x, y), SideFromCharCase(ch), 1, GetPawnMoves(SideFromCharCase(ch))),
                                'n' => new Piece("Knight", ch, new Coord(x, y), SideFromCharCase(ch), 3, GetKnightMoves()),
                                'b' => new Piece("Bishop", ch, new Coord(x, y), SideFromCharCase(ch), 3, GetBishopMoves()),
                                'r' => new Piece("Rook", ch, new Coord(x, y), SideFromCharCase(ch), 5, GetRookMoves()),
                                'q' => new Piece("Queen", ch, new Coord(x, y), SideFromCharCase(ch), 9, GetQueenMoves()),
                                'k' => new Piece("King", ch, new Coord(x, y), SideFromCharCase(ch), 99, GetKingMoves()),
                                _ => throw new ArgumentException("Unhandled Char", nameof(ch))
                            }
                        );
                        x++;
                    }
                    else {
                        if (Char.IsNumber(ch)) {
                            x += (int)Char.GetNumericValue(ch);
                        }
                    }
                }
                y--;
            }
            return output;
        }
    }

}