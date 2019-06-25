using System;

namespace Reversi
{
    public class ReversiEngine
    {
        private int[,] board;

        public int BoardWidth { get; private set; }
        public int BoardHeight { get; private set; }
        public int NextMovePlayerNumber { get; private set; } = 1;

        public ReversiEngine(int startingPlayerNumber, int boardWidth = 8, int boardHeight = 8)
        {
            if (startingPlayerNumber < 1 || startingPlayerNumber > 2)
            {
                throw new Exception("Invalid starting player number");
            }
            BoardWidth = boardWidth;
            BoardHeight = boardHeight;
            board = new int[BoardWidth, BoardHeight];
            ClearBoard();
            NextMovePlayerNumber = startingPlayerNumber;
        }

        public int GetFieldState(int horizontal, int vertical)
        {
            if (!AreFieldCordsCorrect(horizontal, vertical))
            {
                throw new Exception("Invalid field coordinates");
            }
            return board[horizontal, vertical];
        }

        public bool PutPawn(int horizontal, int vertical)
        {
            return PutPawn(horizontal, vertical, false) > 0;
        }

        protected int PutPawn(int horizontal, int vertical, bool onlyTest)
        {
            if (!AreFieldCordsCorrect(horizontal, vertical))
            {
                throw new Exception("Invalid field coordinates");
            }

            if (board[horizontal, vertical] != 0)
            {
                return -1;
            }

            var numOfOccupiedFields = 0;

            for (var directionHorizontal = -1; directionHorizontal <= 1; directionHorizontal++)
            {
                for (var directionVertical = -1; directionVertical <= 1; directionVertical++)
                {
                    if (directionHorizontal == 0 && directionVertical == 0)
                    {
                        continue;
                    }

                    var i = horizontal;
                    var j = vertical;
                    var foundOpponentPawn = false;
                    var foundCurrentPlayerPawn = false;
                    var foundEmptyField = false;
                    var reachBoardEdge = false;

                    do
                    {
                        i += directionHorizontal;
                        j += directionVertical;

                        if (!AreFieldCordsCorrect(i, j))
                        {
                            reachBoardEdge = true;
                        }

                        if (!reachBoardEdge)
                        {
                            if (board[i, j] == NextMovePlayerNumber)
                            {
                                foundCurrentPlayerPawn = true;
                            }

                            if (board[i, j] == 0)
                            {
                                foundEmptyField = true;
                            }

                            if (board[i, j] == OpponentNumber(NextMovePlayerNumber))
                            {
                                foundOpponentPawn = true;
                            }
                        }
                    }
                    while (!(reachBoardEdge || foundCurrentPlayerPawn || foundEmptyField));

                    var isPutPawnPossible = foundOpponentPawn && foundCurrentPlayerPawn && !foundEmptyField;

                    if (isPutPawnPossible)
                    {
                        var maxIndex = Math.Max(Math.Abs(i - horizontal), Math.Abs(j - vertical));

                        if (!onlyTest)
                        {
                            for (var index = 0; index < maxIndex; index++)
                            {
                                board[horizontal + index * directionHorizontal, vertical + index * directionVertical] = NextMovePlayerNumber;
                            }
                        }

                        numOfOccupiedFields += maxIndex - 1;
                    }
                }
            }

            if (numOfOccupiedFields > 0 && !onlyTest)
            {
                ChangeCurrentPlayer();
            }

            return numOfOccupiedFields;
        }

        private static int OpponentNumber(int playerNumber)
        {
            return playerNumber == 1 ? 2 : 1;
        }

        private bool AreFieldCordsCorrect(int horizontal, int vertical)
        {
            return horizontal >= 0 && horizontal < BoardWidth && vertical >= 0 && vertical < BoardHeight;
        }

        private void ClearBoard()
        {
            for (var i = 0; i < BoardWidth; i++)
            {
                for (var j = 0; j < BoardHeight; j++)
                {
                    board[i, j] = 0;
                }
            }
            var centerWidth = BoardWidth / 2;
            var centerHeight = BoardHeight / 2;
            board[centerWidth - 1, centerHeight - 1] = board[centerWidth, centerHeight] = 1;
            board[centerWidth - 1, centerHeight] = board[centerWidth, centerHeight - 1] = 2;
        }

        private void ChangeCurrentPlayer()
        {
            NextMovePlayerNumber = OpponentNumber(NextMovePlayerNumber);
        }
    }
}
