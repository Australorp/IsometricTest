using System;
using Types;
using static FEN.FENReader;
using static PieceMoves.PieceMoveGen;
using static Eval.EvaluateBoard;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpChess
{
    class Program
    {

        public static void DebugPieceMoves(Board board, string startSquare) {
            Piece startPiece = board.pieces.Find(p => p.coord == board.squares.Find(s => s.name == startSquare).coord);
            List<Coord> pieceTargetCoords = GetLegalMoves(board, startPiece).Select(b => b.squareLastMovedTo.coord).ToList();
            Console.WriteLine("_________________");
            for (int y = 8; y > 0; y--) {
                for (int x = 1; x < 9; x++) {
                    Console.Write("|");
                    if (pieceTargetCoords.Contains(new Coord(x, y))) {
                        Console.Write("*");
                    }
                    else if (new Coord(x, y) == startPiece.coord) {
                        Console.Write("#");
                    }
                    else if (board.pieces.Select(p => p.coord).Contains(new Coord(x, y))) {
                        Piece foundPiece = board.pieces.Where(p => p.coord == new Coord(x, y)).First();
                        Console.Write(foundPiece.character.ToString());
                    }
                    else {
                        Console.Write(" ");
                    }
                }
                Console.Write("|");
                Console.Write("\n");
            }
            Console.WriteLine("_________________");
        }

        static void Main()
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            Game game = new(new List<Board>());
            List<Piece> pieces = ReadFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");
            game.AddBoard(new(pieces, CreateSquares(), Side.White));

            bool running = true;
            while (running) {
                Console.Write("Enter a command: ");
                string[] input = Console.ReadLine().Split(' ');

                switch (input[0])
                {
                    case "move":
                        if (input.Count() >= 3) {
                            game.AddBoard(MakeMove(game.Last(), input[1], input[2]));
                            Console.WriteLine(game.Last());
                        }
                        else if (input.Count() == 1) {
                            game.AddBoard(ChooseAlphaBeta(game.Last(), 3));
                            Console.WriteLine(game.Last());
                        }
                        break;
                    case "show-moves":
                        if (input.Count() == 2) {
                            DebugPieceMoves(game.Last(), input[1]);
                        }
                        break;
                    case "PGN":
                        Console.WriteLine(game.PGN());
                        break;
                    case "exit":
                        running = false;
                        break;
                    case "play":
                        for (int i = 1; i < 20; i++) {
                            game.AddBoard(await rootNegaMax(game.Last(), 3));
                            Console.WriteLine(game.Last());
                        }
                        Console.WriteLine(game.PGN());
                        break;
                    case "playTest":
                        for (int i = 1; i < 20; i++) {
                            game.AddBoard(ChooseAlphaBeta(game.Last(), 3));
                            Console.WriteLine(game.Last());
                        }
                        Console.WriteLine(game.PGN());
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
