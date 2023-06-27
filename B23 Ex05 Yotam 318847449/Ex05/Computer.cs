using System;
using System.Collections.Generic;
using System.Linq;

namespace Ex05
{
    public class Computer
    {
        private LinkedList<ComputerPositionNode> m_CandidateMoves;
        private Board m_Board;

        public Computer(Board i_Board)
        {
            this.m_Board = i_Board;
            this.m_CandidateMoves = new LinkedList<ComputerPositionNode>();
        }

        public BoardPosition GiveLocation()
        {
            // the main Computer function that returns the Computer's chosen position
            bool fullBuildTreeWillTakeYears = this.m_Board.EmptyPositions.Count > 25;

            if (fullBuildTreeWillTakeYears)
            {
                this.buildSemiTree();
            }
            else
            {
                this.buildTree();
            }

            return this.bestCandidateMove();
        }

        private void buildTree()
        {
            for (int i = 0; i < m_Board.Size; i++)
            {
                for (int j = 0; j < m_Board.Size; j++)
                {
                    if (m_Board.BoardState[i, j] == (int)eTiles.empty)
                    {
                        m_CandidateMoves.AddLast(new ComputerPositionNode(i, j, this.miniMax(m_Board.Copy(new BoardPosition(i, j)), 0)));
                        // pass a copied board with the move made to miniMax Function to do it's thing
                    }
                }
            }
        }

        private float calculatePrecentOfFullPositions()
        {
            return (float)(m_Board.OPositions.Count + m_Board.XPositions.Count) / m_Board.EmptyPositions.Count;
        }

        private void buildSemiTree()
        {
            float state = this.calculatePrecentOfFullPositions();

            if (state < 0.33)
            {
                this.m_CandidateMoves.AddLast(this.tieBreak(m_Board.BoardPositionsToComputerPositions));
            }
            else
            {
                Random randomNumber = new Random();
                for (int i = 0; i < m_Board.Size; i++)
                {
                    for (int j = 0; j < m_Board.Size; j++)
                    {
                        bool shouldEvaluatePosition = randomNumber.NextDouble() < 0.15; // To make it so only 15% of positions are calculated and the calculated positions are spread out
                        if (shouldEvaluatePosition || m_CandidateMoves.Count == 0)
                        {
                            if (this.m_Board.BoardState[i, j] == (int)eTiles.empty)
                            {
                                m_CandidateMoves.AddLast(new ComputerPositionNode(i, j, this.miniMax(this.m_Board.Copy(new BoardPosition(i, j)), 0)));
                                // pass a copied board with the move made to miniMax Function to do it's thing
                            }
                        }
                    }
                }
            }
        }

        private int miniMax(Board i_Board, int i_Depth, int i_Alpha = int.MinValue, int i_Beta = int.MaxValue)
        {
            // couldn't make method work with one return
            if (i_Board.Status != eStatus.Ongoing || (m_Board.Size == 4 && i_Depth == 5 && m_Board.EmptyPositions.Count > 10) ||
               (m_Board.Size > 4 && i_Depth == 3 && m_Board.EmptyPositions.Count > 10) ||
               (m_Board.EmptyPositions.Count <= 10 && i_Depth == m_Board.EmptyPositions.Count))// can sacrifise play accuracy for speed
            {
                return this.staticEvaluation(i_Board);
            }

            if (this.maxingTurn(i_Board.Turn))
            {
                int maxEvaluation = int.MinValue;

                foreach (BoardPosition position in i_Board.EmptyPositions)
                {
                    int currentEvaluation = this.miniMax(i_Board.Copy(position), i_Depth + 1, i_Alpha, i_Beta);
                    maxEvaluation = Math.Max(maxEvaluation, currentEvaluation);
                    i_Alpha = Math.Max(i_Alpha, currentEvaluation);

                    if (i_Beta <= i_Alpha)
                    {
                        break;
                    }
                }

                return maxEvaluation;
            }
            else
            {
                int minEvaluation = int.MaxValue;

                foreach (BoardPosition position in i_Board.EmptyPositions)
                {
                    int currentEvaluation = this.miniMax(i_Board.Copy(position), i_Depth + 1, i_Alpha, i_Beta);
                    minEvaluation = Math.Min(minEvaluation, currentEvaluation);
                    i_Beta = Math.Min(i_Beta, currentEvaluation);

                    if (i_Beta <= i_Alpha)
                    {
                        break;
                    }
                }

                return minEvaluation;
            }
        }

        private int staticEvaluation(Board i_Board) // decided to make the static evaluation the two longest streaks added up, probably can be better
        {
            int evaluation = 0;

            if (i_Board.Status == eStatus.OLost)
            {
                if (m_Board.Turn == eTiles.O)
                {
                    evaluation = int.MinValue;
                }
                else
                {
                    evaluation = int.MaxValue;
                }
            }
            else if (i_Board.Status == eStatus.XLost)
            {
                if (m_Board.Turn == eTiles.O)
                {
                    evaluation = int.MaxValue;
                }
                else
                {
                    evaluation = int.MinValue;
                }
            }
            else if (m_Board.Turn == eTiles.O)
            {
                evaluation = i_Board.StreakEvaluation(eTiles.X);
            }
            else
            {
                evaluation = i_Board.StreakEvaluation(eTiles.O);
            }

            return evaluation;
        }

        private BoardPosition bestCandidateMove()
        {
            LinkedList<ComputerPositionNode> tieBreakList = new LinkedList<ComputerPositionNode>();
            ComputerPositionNode bestPosition = m_CandidateMoves.First();

            foreach (ComputerPositionNode position in m_CandidateMoves)
            {
                if (bestPosition.Evaluation < position.Evaluation)
                {
                    bestPosition = position;
                }
            }

            foreach (ComputerPositionNode position in m_CandidateMoves)
            {
                if (bestPosition.Evaluation == position.Evaluation)
                {
                    tieBreakList.AddLast(position);
                }
            }

            bestPosition = this.tieBreak(tieBreakList);
            tieBreakList.Clear();
            m_CandidateMoves.Clear();

            return new BoardPosition(bestPosition.Row, bestPosition.Column);
        }

        private ComputerPositionNode tieBreak(LinkedList<ComputerPositionNode> i_MovesList)
        {
            ComputerPositionNode chosenPosition = i_MovesList.First();
            ComputerPositionNode potentialPosition;
            int worstStreak = this.streakPotential(chosenPosition);
            int currentStreak = 0;

            i_MovesList.RemoveFirst();
            while (i_MovesList.Count != 0)
            {
                potentialPosition = i_MovesList.First();
                currentStreak = this.streakPotential(potentialPosition);
                if (currentStreak < worstStreak)
                {
                    chosenPosition = potentialPosition;
                    worstStreak = currentStreak;
                }
                i_MovesList.RemoveFirst();
            }

            return chosenPosition;
        }

        private int streakPotential(ComputerPositionNode i_Position)
        {
            bool onRightToLeftDiagonal = (i_Position.Row + i_Position.Column) == m_Board.Size - 1; // top to bottom
            bool onLeftToRightDiagonal = i_Position.Row == i_Position.Column;
            int rowPotential = 0;
            int rowOpponentPotential = 0;
            int columnPotential = 0;
            int columnOpponentPotential = 0;
            int leftToRightPotential = 0;
            int rightToLeftPotential = 0;
            int leftToRightOpponentPotential = 0;
            int rightToLeftOpponentPotential = 0;

            for (int i = 0; i < m_Board.Size; i++)
            {
                if (m_Board.BoardState[i_Position.Row, i] == (int)m_Board.Turn && i != i_Position.Column)
                {
                    rowPotential++;
                }
                else if (m_Board.BoardState[i_Position.Row, i] != (int)eTiles.empty)
                {
                    rowOpponentPotential++;
                }
            }

            rowPotential = checkIfComputerTileInPositionWillHelpOpponent(rowPotential, rowOpponentPotential);

            for (int i = 0; i < m_Board.Size; i++)
            {
                if (m_Board.BoardState[i, i_Position.Column] == (int)m_Board.Turn && i != i_Position.Row)
                {
                    columnPotential++;
                }
                else if (m_Board.BoardState[i, i_Position.Column] != (int)eTiles.empty)
                {
                    columnOpponentPotential++;
                }
            }

            columnPotential = checkIfComputerTileInPositionWillHelpOpponent(columnPotential, columnOpponentPotential);

            if (onLeftToRightDiagonal)
            {
                for (int i = 0; i < m_Board.Size; i++)
                {
                    if (m_Board.BoardState[i, i] == (int)m_Board.Turn && i != i_Position.Row)
                    {
                        leftToRightPotential++;
                    }
                    else if (m_Board.BoardState[i, i] != (int)eTiles.empty)
                    {
                        leftToRightOpponentPotential++;
                    }
                }

                leftToRightPotential = checkIfComputerTileInPositionWillHelpOpponent(leftToRightPotential, leftToRightOpponentPotential, 1);
            }

            if (onRightToLeftDiagonal)
            {
                // strange calculation, the right to left diagonal increments by 9 from position to position
                for (int i = (m_Board.Size - 1) * 10; i > 0; i = i - 9)
                {
                    if (m_Board.BoardState[i / 10, i % 10] == (int)m_Board.Turn && (i / 10) != i_Position.Row)
                    {
                        rightToLeftPotential++;
                    }
                    else if (m_Board.BoardState[i / 10, i % 10] != (int)eTiles.empty)
                    {
                        rightToLeftOpponentPotential++;
                    }
                }

                rightToLeftPotential = this.checkIfComputerTileInPositionWillHelpOpponent(rightToLeftPotential, rightToLeftOpponentPotential, 1);
            }

            return columnPotential + rowPotential + rightToLeftPotential + leftToRightPotential;
        }

        private int checkIfComputerTileInPositionWillHelpOpponent(int i_MyeTiles, int i_OpponenteTiles, int i_OnMainDiagonal = 0)
        {
            // the point of the tie break is to choose the tile that does least harm and lets opponent self destruct
            int numberToReturn = 0;

            if (i_MyeTiles == 0 && i_OpponenteTiles > 0)
            {
                numberToReturn = i_OpponenteTiles;
            }
            else if (i_MyeTiles != 0 && i_OpponenteTiles == 0)
            {
                if (i_MyeTiles >= m_Board.Size - 2)
                {
                    numberToReturn = int.MaxValue;
                }
                else
                {
                    numberToReturn = i_MyeTiles;
                }
            }

            return numberToReturn + i_OnMainDiagonal;  // the last argument is for bias against putting eTiles on main diagonal
        }

        private bool maxingTurn(eTiles i_Tile)
        {
            return m_Board.Turn == i_Tile;
        }
    }
}