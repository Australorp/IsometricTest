using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Types;
using static PieceMoves.PieceMoveGen;

namespace Eval {
    class EvaluateBoard {

        public static int SideRelative(Board board) {
            return board.sideToMove switch {
                Side.White => 1,
                Side.Black => -1
            };
        }

        public static float CenterAdjustMaterial(Piece piece) {
            float output = piece.value;
            if (piece.name != "King") {
                output *= piece.coord.x switch {
                    1 => 0.8f,
                    2 => 0.9f,
                    3 => 1.0f,
                    4 => 1.1f,
                    5 => 1.1f,
                    6 => 1.0f,
                    7 => 0.9f,
                    8 => 0.8f
                };
                if (piece.side == Side.White) {
                    output *= piece.coord.y switch {
                        1 => 0.8f,
                        2 => 0.9f,
                        3 => 1.0f,
                        4 => 1.1f,
                        5 => 1.1f,
                        6 => 1.1f,
                        7 => 1.1f,
                        8 => 1.1f
                    };
                }
                else {
                    output *= piece.coord.y switch {
                        1 => 1.1f,
                        2 => 1.1f,
                        3 => 1.1f,
                        4 => 1.1f,
                        5 => 1.1f,
                        6 => 1.0f,
                        7 => 0.9f,
                        8 => 0.8f
                    };
                }
            }
            return output;
        } 

        public static int CountMaterial(Board board) {
            return board.pieces.Where(p => p.side == Side.White).Select(p => p.value).Sum() - board.pieces.Where(p => p.side == Side.Black).Select(p => p.value).Sum();
        }

        public static float CountAdjustedMaterial(Board board) {
            return board.pieces.Where(p => p.side == Side.White).Select(p => CenterAdjustMaterial(p)).Sum() - board.pieces.Where(p => p.side == Side.Black).Select(p => CenterAdjustMaterial(p)).Sum();
        }

        public static async Task<float> negaMax(Board board, float depth) {
            if (depth == 0) { return CountAdjustedMaterial(board) * SideRelative(board); }
            float max = -9999;
            foreach (Board resultingBoard in board.pieces.Where(p => p.side == board.sideToMove).Select(p => GetLegalMoves(board, p)).Aggregate((x, y) => x.Concat(y.ToList()).ToList())) {
                float score = await negaMax(resultingBoard, depth - 1);
                score = -score;
                if (score > max) {
                    max = score;
                }
            }
            return max;
        }

        public static async Task<Board> rootNegaMax(Board board, float depth) {
            // if (depth == 0) { return CountMaterial(board); }
            float max = -9999;
            Board output = board;
            foreach (Board resultingBoard in board.pieces.Where(p => p.side == board.sideToMove).Select(p => GetLegalMoves(board, p)).Aggregate((x, y) => x.Concat(y.ToList()).ToList())) {
                float score = await negaMax(resultingBoard, depth - 1);
                score = -score;
                if (score > max) {
                    max = score;
                    output = resultingBoard;
                }
            }
            return output;
        }

        public static float alphaBetaMax(Board board, int depthLeft, float alpha = -999f, float beta = 999f) {
            if (depthLeft == 0) { return CountAdjustedMaterial(board) * SideRelative(board); }
            foreach (Board resultingBoard in board.pieces.Where(p => p.side == board.sideToMove).Select(p => GetLegalMoves(board, p)).Aggregate((x, y) => x.Concat(y.ToList()).ToList())) {
                float score = alphaBetaMin(resultingBoard, alpha, beta, depthLeft - 1);
                if (score >= beta) {
                    return beta;
                }
                if (score > alpha) {
                    alpha = score;
                }
            }
            return alpha;
        }

        public static float alphaBetaMin(Board board, float alpha, float beta, int depthLeft) {
            if (depthLeft == 0) { return -(CountAdjustedMaterial(board) * SideRelative(board)); }
            foreach (Board resultingBoard in board.pieces.Where(p => p.side == board.sideToMove).Select(p => GetLegalMoves(board, p)).Aggregate((x, y) => x.Concat(y.ToList()).ToList())) {
                float score = alphaBetaMax(resultingBoard, depthLeft - 1, alpha, beta);
                if (score <= alpha) {
                    return alpha;
                }
                if (score < beta) {
                    beta = score;
                }
            }
            return beta;
        }

        public static Board ChooseAlphaBeta(Board board, int depth) {
            var t = board.pieces.Where(p => p.side == board.sideToMove).Select(p => GetLegalMoves(board, p)).Aggregate((x, y) => x.Concat(y.ToList()).ToList());
            foreach (Board b in t) {
                b.eval = alphaBetaMax(b, depth - 1);
            }
            return t.ToList().OrderByDescending<Board, float>(b => (float)b.eval).Last();
        }

        // return board.pieces.Where(p => p.side == Side.White).Select(p => GetMoves(board, p)).Aggregate((x, y) => x.Concat(y.ToList()).ToList()).Sum();
    }
}