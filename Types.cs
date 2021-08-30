using System;
using System.Collections.Generic;
using System.Linq;
using static PieceMoves.PieceMoveGen;

namespace Types
{
    enum Side {
        White,
        Black
    }

    enum CaptureType {
        CanCapture,
        NoCapture,
        OnlyCapture
    }

    enum MoveType {
        Hop,
        Slide
    }

    static class SideMethods {
        public static Side FlipSide(this Side side) {
            return side switch {
                Side.White => Side.Black,
                Side.Black => Side.White,
                _ => throw new ArgumentException("Unexpected enum value", nameof(side))
            };
        }
    }

    class Coord : IEquatable<Coord> {
        public int x { get; }
        public int y { get; }

        public Coord(int X, int Y) {
            x = X;
            y = Y;
        }

        public bool Equals(Coord other)
        {
            return this.x == other.x && this.y == other.y;
        }

        public static Coord operator +(Coord a, Coord b) {
            return new Coord(a.x + b.x,  a.y + b.y);
        }

        public static Coord operator -(Coord a, Coord b) {
            return new Coord(a.x - b.x,  a.y - b.y);
        }

        public static Coord operator *(Coord a, int b) {
            return new Coord(a.x * b,  a.y * b);
        }

        public static Boolean operator ==(Coord a, Coord b) {
            return (a.x == b.x && a.y == b.y);
        }

        public static Boolean operator !=(Coord a, Coord b) {
            return !(a.x == b.x && a.y == b.y);
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }

    class Square : IEquatable<Square> {
        public string name { get; }
        public Coord coord { get; }

        public Square(string Name, Coord Coord) {
            name = Name;
            coord = Coord;
        }

        public bool Equals(Square other)
        {
            return (this.name == other.name && this.coord == other.coord);
        }

        public override string ToString()
        {
            return $"{name}, {coord}";
        }
    }

    class MoveRule {
        public Coord coord { get; }
        public CaptureType captureType { get; }
        public bool onlyFromStartSquare { get; } 
        public MoveType moveType { get; }
        public int range { get; }
        public bool onlyCountLast { get; }

        public MoveRule(MoveType MoveType, Coord Coord, CaptureType CaptureType, bool OnlyFromStartSquare, int Range) {
            moveType = MoveType;
            coord = Coord;
            captureType = CaptureType;
            onlyFromStartSquare = OnlyFromStartSquare;
            range = Range;
        } 
    }

    class Piece : IEquatable<Piece> {
        public string name { get; }
        public Char character { get; }
        public Coord coord { get; }
        public Side side { get; }
        public int value { get; }
        public List<MoveRule> moveRules { get; }

        public Piece(string Name, Char Character, Coord Coord, Side Side, int Value, List<MoveRule> MoveRules) {
            name = Name;
            character = Character;
            coord = Coord;
            side = Side;
            value = Value;
            moveRules = MoveRules;
        }

        public bool Equals(Piece other)
        {
            return (this.name == other.name && this.character == other.character && this.coord == other.coord && this.side == other.side && this.value == other.value && this.moveRules == other.moveRules);
        }

        public override string ToString()
        {
            return $"{name}, {coord}, {character}, {side}, [{String.Join(", ", moveRules.Select(mc => mc.ToString()))}]";
        }

        public static List<Piece> operator -(List<Piece> ps, Piece p) {
            return ps.Where(x => x != p).ToList();
        }

        public static List<Piece> operator +(List<Piece> ps, Piece p) {
            return ps.Append(p).ToList();
        }

        public Piece MovedTo(Coord c) {
            return new Piece(
                name,
                character,
                c,
                side,
                value,
                moveRules.Where(mr => !mr.onlyFromStartSquare).ToList()
            );
        }
    }

    class Board : IEquatable<Board> {
        public List<Piece> pieces { get; }
        public List<Square> squares { get; }
        public Side sideToMove { get; }
        public Square? squareLastMovedTo { get; }
        public Piece? pieceLastMoved { get; }

        public float? eval { get; set; }

        public Board(List<Piece> Pieces, List<Square> Squares, Side SideToMove, Square? SquareLastMovedTo = null, Piece? PieceLastMoved = null, float? Eval = null) {
            pieces = Pieces;
            squares = Squares;
            sideToMove = SideToMove;
            squareLastMovedTo = SquareLastMovedTo;
            pieceLastMoved = PieceLastMoved;
            eval = Eval;
        }

        public bool Equals(Board other)
        {
            return (this.pieces == other.pieces && this.squares == other.squares && this.sideToMove == other.sideToMove);
        }

        public override string ToString()
        {
            string output = "_________________\n";
            for (int y = 8; y > 0; y--) {
                for (int x = 1; x < 9; x++) {
                    output += "|";
                    if (pieces.Select(p => p.coord).Contains(new Coord(x, y))) {
                        Piece foundPiece = pieces.Where(p => p.coord == new Coord(x, y)).First();
                        output += foundPiece.character.ToString();
                    }
                    else {
                        output += " ";
                    }
                }
                output += "|";
                output += "\n";
            }
            return output += "_________________";
        }
    }

    class Game {
        public List<Board> boardList { get; }
        
        public Game(List<Board> BoardList) {
            boardList = BoardList;
        }

        public string PGN() {
            string output = "";
            int z = 1;
            for (int i = 1; i <= boardList.Count() / 2; i++) {
                if (z + 1 < boardList.Count()) {
                    string firstPiece = "";
                    if (boardList[z].pieceLastMoved.name != "Pawn") {
                        firstPiece = Char.ToUpper(boardList[z].pieceLastMoved.character).ToString();
                    }
                    string secondPiece = "";
                    if (boardList[z+1].pieceLastMoved.name != "Pawn") {
                        secondPiece = Char.ToUpper(boardList[z+1].pieceLastMoved.character).ToString();
                    }
                    output += $"{i}. {firstPiece}{boardList[z].squareLastMovedTo.name} {secondPiece}{boardList[z+1].squareLastMovedTo.name} ";
                    z = z + 2;
                }
                else if (z <= boardList.Count()) {
                    string firstPiece = "";
                    if (boardList[z].pieceLastMoved.name != "Pawn") {
                        firstPiece = Char.ToUpper(boardList[z].pieceLastMoved.character).ToString();
                    }
                    output += $"{i}. {firstPiece}{boardList[z].squareLastMovedTo.name} ";
                    z = z + 1;
                }
            }
            return output;
        }

        public Board Last() {
            return boardList.Last();
        }

        public void AddBoard(Board board) {
            boardList.Add(board);
        }
    }
}
