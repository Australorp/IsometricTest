using System;
using System.Collections.Generic;
using System.Linq;
using Types;

namespace PieceMoves {

    class PieceMoveGen {

        

        public static List<MoveRule> GetPawnMoves(Side side) {
            int y = side switch {
                Side.White => 1,
                Side.Black => -1
            };
            return new List<MoveRule>() {
                    new MoveRule(MoveType.Slide, new Coord(0,(1 * y)), CaptureType.NoCapture, false, 1),
                    new MoveRule(MoveType.Slide, new Coord(0,(1 * y)), CaptureType.NoCapture, true, 2),
                    new MoveRule(MoveType.Slide, new Coord(1,(1 * y)), CaptureType.OnlyCapture, false, 1),
                    new MoveRule(MoveType.Slide, new Coord(-1,(1 * y)), CaptureType.OnlyCapture, false, 1)
            };
        }

        public static List<MoveRule> GetKnightMoves() {
            List<MoveRule> output = new List<MoveRule>();
            foreach ((int, int) pair in new List<(int, int)>() { (1,2),(-1,2),(1,-2),(-1,-2),(2,1),(-2,1),(2,-1),(-2,-1) }) {
                output.Add(new MoveRule(MoveType.Hop, new Coord(pair.Item1, pair.Item2), CaptureType.CanCapture, false, 1));
            }
            return output;
        }

        public static List<MoveRule> GetBishopMoves() {
            return new List<MoveRule>() {
                new MoveRule(MoveType.Slide, new Coord(1,1), CaptureType.CanCapture, false, 8),
                new MoveRule(MoveType.Slide, new Coord(1,-1), CaptureType.CanCapture, false, 8),
                new MoveRule(MoveType.Slide, new Coord(-1,-1), CaptureType.CanCapture, false, 8),
                new MoveRule(MoveType.Slide, new Coord(-1,1), CaptureType.CanCapture, false, 8)
            };
        }

        public static List<MoveRule> GetRookMoves() {
            return new List<MoveRule>() {
                new MoveRule(MoveType.Slide, new Coord(0,1), CaptureType.CanCapture, false, 8),
                new MoveRule(MoveType.Slide, new Coord(0,-1), CaptureType.CanCapture, false, 8),
                new MoveRule(MoveType.Slide, new Coord(-1,0), CaptureType.CanCapture, false, 8),
                new MoveRule(MoveType.Slide, new Coord(1,0), CaptureType.CanCapture, false, 8)
            };
        }

        public static List<MoveRule> GetQueenMoves() {
            return new List<MoveRule>() {
                new MoveRule(MoveType.Slide, new Coord(0,1), CaptureType.CanCapture, false, 8),
                new MoveRule(MoveType.Slide, new Coord(0,-1), CaptureType.CanCapture, false, 8),
                new MoveRule(MoveType.Slide, new Coord(-1,0), CaptureType.CanCapture, false, 8),
                new MoveRule(MoveType.Slide, new Coord(1,0), CaptureType.CanCapture, false, 8),
                new MoveRule(MoveType.Slide, new Coord(1,1), CaptureType.CanCapture, false, 8),
                new MoveRule(MoveType.Slide, new Coord(1,-1), CaptureType.CanCapture, false, 8),
                new MoveRule(MoveType.Slide, new Coord(-1,-1), CaptureType.CanCapture, false, 8),
                new MoveRule(MoveType.Slide, new Coord(-1,1), CaptureType.CanCapture, false, 8)
            };
        }

        public static List<MoveRule> GetKingMoves() {
            return new List<MoveRule>() {
                new MoveRule(MoveType.Hop, new Coord(0,1), CaptureType.CanCapture, false, 1),
                new MoveRule(MoveType.Hop, new Coord(0,-1), CaptureType.CanCapture, false, 1),
                new MoveRule(MoveType.Hop, new Coord(-1,0), CaptureType.CanCapture, false, 1),
                new MoveRule(MoveType.Hop, new Coord(1,0), CaptureType.CanCapture, false, 1),
                new MoveRule(MoveType.Hop, new Coord(1,1), CaptureType.CanCapture, false, 1),
                new MoveRule(MoveType.Hop, new Coord(1,-1), CaptureType.CanCapture, false, 1),
                new MoveRule(MoveType.Hop, new Coord(-1,-1), CaptureType.CanCapture, false, 1),
                new MoveRule(MoveType.Hop, new Coord(-1,1), CaptureType.CanCapture, false, 1)
            };
        }

        public static (List<Board>, bool) VerifyMoveToSquare(Board board, Piece piece, MoveRule moveRule, Coord targetCoord) {
            var output = (new List<Board>(), false);
            // If indicated Square exists
            if (board.squares.Select(s => s.coord).Contains(targetCoord)) {
                // If piece is on that Square
                if (board.pieces.Select(p => p.coord).Contains(targetCoord)) {
                    output.Item2 = true;
                    if (moveRule.captureType != CaptureType.NoCapture) {
                        Piece targetPiece = board.pieces.Find(p => p.coord == targetCoord);

                        // If piece on targetSquare is not of the same side
                        if (targetPiece.side != piece.side) {
                            
                            // If moveRule is not noCapture
                            if (moveRule.captureType != CaptureType.NoCapture) {
                                output.Item1.Add(new Board(
                                    ((board.pieces - piece) - targetPiece) + piece.MovedTo(targetCoord),
                                    board.squares,
                                    board.sideToMove.FlipSide(),
                                    board.squares.Find(s => s.coord == targetCoord),
                                    piece
                                ));
                                
                            }
                        }
                    }
                }
                else {
                    // Adding a move to an empty square, only if the move rule is CanCapture or NoCapture
                    if (moveRule.captureType != CaptureType.OnlyCapture) {
                        output.Item1.Add(new Board(
                            (board.pieces - piece) + piece.MovedTo(targetCoord),
                            board.squares,
                            board.sideToMove.FlipSide(),
                            board.squares.Find(s => s.coord == targetCoord),
                            piece
                        ));
                    }
                }
            }
            return output;
        }

        public static List<Board> GetSlideMoves(Board board, Piece piece, MoveRule moveRule) {
            List<Board> output = new List<Board>();
            for (int i = 1; i <= moveRule.range; i++ ) {
                Coord adjustedCoord = piece.coord + moveRule.coord * i;
                var results = VerifyMoveToSquare(board, piece, moveRule, adjustedCoord);
                output.AddRange(results.Item1);
                if (results.Item2) { i = 999; } // This means that the slide has been blocked by a piece
            }
            return output;
        }

        public static bool IsMoveLegal(Board board) {
            var king = board.pieces.Find(p => p.name == "King" && p.side == board.sideToMove.FlipSide());
            return !board.pieces.Where(p => p.side == board.sideToMove).Select(p => GetMoves(board, p)).Aggregate((x, y) => x.Concat(y.ToList()).ToList()).Select(b => b.squareLastMovedTo.coord).Contains(king.coord);
        } 

        public static List<Board> GetMoves(Board board, Piece piece) {
            List<Board> output = new List<Board>();
            foreach (MoveRule moveRule in piece.moveRules) {
                if (moveRule.moveType == MoveType.Slide) {
                    output.AddRange(GetSlideMoves(board, piece, moveRule));
                }
                else if (moveRule.moveType == MoveType.Hop) {
                    Coord adjustedCoord = piece.coord + moveRule.coord;
                    output.AddRange(VerifyMoveToSquare(board, piece, moveRule, adjustedCoord).Item1);
                }
            }
            return output;
        }

        public static List<Board> GetLegalMoves(Board board, Piece piece) {
            return GetMoves(board, piece).Where(b => IsMoveLegal(b)).ToList();
        }

        public static Board MakeMove(Board board, string fromSquare, string toSquare) {
            List<Piece> activePieces = board.pieces.Where(p => p.side == board.sideToMove).ToList();
            if (activePieces.Count() > 0) {
                List<Square> tryFromSquares = board.squares.Where(s => s.name == fromSquare).ToList();
                if (tryFromSquares.Count() > 0) {
                    Square foundFromSquare = tryFromSquares.First();
                    List<Piece> tryMovingPieces = activePieces.Where(p => p.coord == foundFromSquare.coord).ToList();
                    if (tryMovingPieces.Count() > 0) {
                        Piece foundMovingPiece = tryMovingPieces.First();
                        List<Square> tryToSquares = board.squares.Where(s => s.name == toSquare).ToList();
                        if (tryToSquares.Count() > 0) {
                            Square foundToSquare = tryToSquares.First();
                            List<Board> tryResultingBoards = GetLegalMoves(board, foundMovingPiece).Where(b => b.squareLastMovedTo.coord == foundToSquare.coord).ToList();
                            if (tryResultingBoards.Count() > 0) {
                                return tryResultingBoards.First();
                            }
                            else {
                                Console.WriteLine("Indicated piece cannot move to that square");
                                return board;
                            } 
                        }
                        else {
                            Console.WriteLine("The target square was not found");
                            return board;
                        } 
                    }
                    else {
                        Console.WriteLine("A valid piece was not found on the starting square");
                        return board;
                    } 
                }
                else {
                    Console.WriteLine("The starting square was not found");
                    return board;
                }                
            }
            else {
                Console.WriteLine("No pieces were found to move");
                return board;
            }
            
        }
    }
}
