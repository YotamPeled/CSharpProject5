using System;
using System.Collections.Generic;
using System.Linq;

namespace Ex05
{
    public class Board
    {
        // Using LinkedList over List as we prioritised fast insertion and removal of elements and don't need efficent index access
        private LinkedList<BoardPosition> m_XPositions = new LinkedList<BoardPosition>();
        private LinkedList<BoardPosition> m_OPositions = new LinkedList<BoardPosition>();
        private LinkedList<BoardPosition> m_EmptyPositions = new LinkedList<BoardPosition>();
        private eTiles m_Turn = eTiles.X;
        private readonly int r_Size;
        private int[,] m_Board;
        private int m_XScore = 0;
        private int m_OScore = 0;
        private eStatus m_eStatus = eStatus.Ongoing;
        private Computer m_Computer;
        private eOpponent m_Opponent;
        private bool m_ComputerSimulation = false;

        private Action<BoardPosition, eTiles> m_UpdatePosition;
        public event Action<BoardPosition, eTiles> UpdatePosition
        {
            add
            {
                if (m_UpdatePosition == null)
                {
                    m_UpdatePosition = value;
                }
                else
                {
                    throw new InvalidOperationException("Multiple observers are not allowed for Execute Item event.");
                }
            }

            remove
            {
                if (m_UpdatePosition != null && m_UpdatePosition.GetInvocationList().Contains(value))
                {
                    m_UpdatePosition -= value;
                }
                else
                {
                    throw new InvalidOperationException("Observer not found for ExecuteItem event.");
                }
            }
        }

        private Action<BoardPosition> m_HighlightHintPosition;
        public event Action<BoardPosition> HighlightHintPosition
        {
            add
            {
                if (m_HighlightHintPosition == null)
                {
                    m_HighlightHintPosition = value;
                }
                else
                {
                    throw new InvalidOperationException("Multiple observers are not allowed for Execute Item event.");
                }
            }

            remove
            {
                if (m_HighlightHintPosition != null && m_HighlightHintPosition.GetInvocationList().Contains(value))
                {
                    m_HighlightHintPosition -= value;
                }
                else
                {
                    throw new InvalidOperationException("Observer not found for ExecuteItem event.");
                }
            }
        }

        private Action m_GameEnd;
        public event Action GameEnd
        {
            add
            {
                if (m_GameEnd == null)
                {
                    m_GameEnd = value;
                }
                else
                {
                    throw new InvalidOperationException("Multiple observers are not allowed for Execute Item event.");
                }
            }

            remove
            {
                if (m_GameEnd != null && m_GameEnd.GetInvocationList().Contains(value))
                {
                    m_GameEnd -= value;
                }
                else
                {
                    throw new InvalidOperationException("Observer not found for ExecuteItem event.");
                }
            }
        }

        public Board(int i_Size)
        {
            this.r_Size = i_Size;
            this.m_Board = new int[r_Size, r_Size];
            initializeBoard();
            m_Computer = new Computer(this);
        }

        private void initializeBoard()
        {
            for (int row = 0; row < r_Size; row++)
            {
                for (int column = 0; column < r_Size; column++)
                {
                    m_Board[row, column] = (int)eTiles.empty;
                    BoardPosition position = new BoardPosition(row, column);

                    m_EmptyPositions.AddLast(position);
                }
            }
        }

        public void doWhenHintIsUsed()
        {
            BoardPosition hintPosition = m_Computer.GiveLocation();
            OnHightlightHintPosition(hintPosition);
        }

        protected virtual void OnHightlightHintPosition(BoardPosition i_HintPosition)
        {
            if (m_HighlightHintPosition != null)
            {
                m_HighlightHintPosition.Invoke(i_HintPosition);
            }
        }

        public void ChangeStartingPlayer() // added for project 5?
        {
            m_Turn = eTiles.O;
        }

        public eStatus Status
        {
            get
            {
                return m_eStatus;
            }
        }

        public eOpponent Opponent
        {
            get
            {
                return m_Opponent;
            }

            set
            {
                m_Opponent = value;
            }
        }

        internal eTiles Turn
        {
            get
            {
                return m_Turn;
            }
        }

        public int OScore
        {
            get
            {
                return m_OScore;
            }
        }

        public int XScore
        {
            get
            {
                return m_XScore;
            }
        }

        public int Size
        {
            get
            {
                return r_Size;
            }
        }

        public int[,] BoardState
        {
            get
            {
                return m_Board;
            }
        }

        public LinkedList<BoardPosition> EmptyPositions
        {
            get
            {
                return m_EmptyPositions;
            }
        }

        public LinkedList<BoardPosition> XPositions
        {
            get
            {
                return m_XPositions;
            }
        }

        public LinkedList<BoardPosition> OPositions
        {
            get
            {
                return m_OPositions;
            }
        }

        internal LinkedList<ComputerPositionNode> BoardPositionsToComputerPositions
        {
            get
            {
                LinkedList<ComputerPositionNode> emptyComputerPositions = new LinkedList<ComputerPositionNode>();
                foreach (BoardPosition position in m_EmptyPositions)
                {
                    emptyComputerPositions.AddLast(position.ToComputerPositionNode());
                }

                return emptyComputerPositions;
            }
        }

        internal void DoWhenButtonWasClicked(BoardPosition i_Position)
        {
            m_Board[i_Position.Row, i_Position.Column] = (int)m_Turn;
            OnUpdatePosition(i_Position, m_Turn);
            placeTileOnBoard(i_Position);
        }

        public bool CheckIfTileCanBeSetInPosition(BoardPosition i_Position)
        {
            bool validPosition = m_Board[i_Position.Row, i_Position.Column] == (int)eTiles.empty;

            return validPosition;
        }

        public void ComputerTurn()
        {
            m_ComputerSimulation = true;
            BoardPosition computerPosition = m_Computer.GiveLocation();
            m_ComputerSimulation = false;

            m_Board[computerPosition.Row, computerPosition.Column] = (int)m_Turn;
            OnUpdatePosition(computerPosition, m_Turn);
            placeTileOnBoard(computerPosition);
        }

        private void placeTileOnBoard(BoardPosition i_Position)       
        {
            if (m_ComputerSimulation)
            {
                m_Board[i_Position.Row, i_Position.Column] = (int)m_Turn;
            }

            this.addTileToList(i_Position);
            if (this.checkForLose(i_Position))
            {
                this.addScore();
                this.changeeStatusForLose();             
            }
            else if (this.fullBoard())
            {
                this.changeeStatusForTie();                  
            }
            else
            {
                this.swapTurn();
            }

            if (!m_ComputerSimulation && m_eStatus != eStatus.Ongoing)
            {
                OnGameEnd();
            }
        }

        protected virtual void OnUpdatePosition(BoardPosition i_Position, eTiles i_Tile)
        {
            if (m_UpdatePosition != null)
            {
                m_UpdatePosition.Invoke(i_Position, i_Tile);
            }
        }

        protected virtual void OnGameEnd()
        {
            if (m_GameEnd != null)
            {
                m_GameEnd.Invoke();
            }
        }

        private void addTileToList(BoardPosition i_Position)
        {
            if (m_Turn == eTiles.X)
            {
                m_XPositions.AddFirst(i_Position);
            }
            else
            {
                m_OPositions.AddFirst(i_Position);
            }

            m_EmptyPositions.Remove(i_Position);
        }

        private bool checkForLose(BoardPosition i_Position)
        {
            bool didPlayerLose = this.horizontalLose(i_Position) || this.verticalLose(i_Position) ||
                                 this.leftDiagonalLose(i_Position) || this.rightDiagonalLose(i_Position);

            return didPlayerLose;
        }

        private int spreadMatchFunction(int i_RowJumps, int i_ColumnJumps, BoardPosition i_Position, bool markPositionAsLost = false)
        {
            int isMatch = 0;
            bool tileToCheckIsInBounds = i_Position.Row < r_Size && i_Position.Row >= 0 &&
                                         i_Position.Column < r_Size && i_Position.Column >= 0;

            if (tileToCheckIsInBounds)
            {
                isMatch = this.checkIfTileListIncludes(i_Position);
            }

            if (isMatch == 1 && markPositionAsLost)
            {
                this.markPositionAsPartOfLosingStreak(i_Position);
            }

            return isMatch == 1 ? isMatch + this.spreadMatchFunction(i_RowJumps, i_ColumnJumps,
                                  new BoardPosition(i_Position.Row + i_RowJumps, i_Position.Column + i_ColumnJumps), markPositionAsLost) : 0;
        }


        private int checkIfTileListIncludes(BoardPosition i_Position)
        {
            bool match = false;

            if (!m_EmptyPositions.Contains(i_Position))
            {
                if (m_Turn == eTiles.X)
                {
                    match = m_XPositions.Contains(i_Position);
                }
                else
                {
                    match = m_OPositions.Contains(i_Position);
                }
            }

            return match ? 1 : 0;
        }

        private int updatePotential(int i_RowJumps, int i_ColumnJumps, BoardPosition i_Position, eTiles i_TileType)
        {
            int returnedValue = 0;
            int isMatch = 0;
            bool tileToCheckIsInBounds = i_Position.Row < r_Size && i_Position.Row >= 0 &&
                                         i_Position.Column < r_Size && i_Position.Column >= 0;

            if (tileToCheckIsInBounds)
            {
                isMatch = this.checkIfTileListIncludesForPotential(i_Position, i_TileType);
            }

            if (isMatch == 1 || (tileToCheckIsInBounds && isMatch == 0))
            {
                returnedValue = isMatch + this.updatePotential(i_RowJumps, i_ColumnJumps, new BoardPosition(i_Position.Row + i_RowJumps, i_Position.Column + i_ColumnJumps), i_TileType);
            }
            else
            {
                returnedValue = isMatch;
            }

            return returnedValue;
        }

        private int checkIfTileListIncludesForPotential(BoardPosition i_Position, eTiles i_TileType)
        {
            int returnedValue = 0; // empty = 0, bad match = minValue, good match = 1
            bool badMatch = false;
            bool emptyTile = true;

            if (!m_EmptyPositions.Contains(i_Position))
            {
                emptyTile = false;
                if (i_TileType == eTiles.X)
                {
                    badMatch = m_OPositions.Contains(i_Position);
                }
                else
                {
                    badMatch = m_XPositions.Contains(i_Position);
                }
            }

            if (emptyTile)
            {
                returnedValue = 0;
            }
            else if (badMatch)
            {
                returnedValue = int.MinValue;
            }
            else
            {
                returnedValue = 1;
            }

            return returnedValue;
        }

        private bool horizontalLose(BoardPosition i_Position, bool markAsLoss = false)
        {
            int matches = this.spreadMatchFunction((int)eDirections.None, (int)eDirections.Right, new BoardPosition(i_Position.Row, i_Position.Column + (int)eDirections.Right)) +
                          this.spreadMatchFunction((int)eDirections.None, (int)eDirections.Left, new BoardPosition(i_Position.Row, i_Position.Column + (int)eDirections.Left)) + 1; // +1 to include current tile
            bool lose = matches >= r_Size;

            if (lose) // mark eTiles for losing "animation"
            {
                this.markPositionAsPartOfLosingStreak(i_Position);
                this.spreadMatchFunction((int)eDirections.None, (int)eDirections.Right, new BoardPosition(i_Position.Row, i_Position.Column + (int)eDirections.Right), lose);
                this.spreadMatchFunction((int)eDirections.None, (int)eDirections.Left, new BoardPosition(i_Position.Row, i_Position.Column + (int)eDirections.Left), lose);
            }

            return lose;
        }

        private bool verticalLose(BoardPosition i_Position, bool markAsLoss = false)
        {
            int matches = this.spreadMatchFunction((int)eDirections.Up, (int)eDirections.None, new BoardPosition(i_Position.Row + (int)eDirections.Up, i_Position.Column)) +
                          this.spreadMatchFunction((int)eDirections.Down, (int)eDirections.None, new BoardPosition(i_Position.Row + (int)eDirections.Down, i_Position.Column)) + 1;
            bool lose = matches >= r_Size;

            if (lose)
            {
                this.markPositionAsPartOfLosingStreak(i_Position);
                this.spreadMatchFunction((int)eDirections.Up, (int)eDirections.None, new BoardPosition(i_Position.Row + (int)eDirections.Up, i_Position.Column), lose);
                this.spreadMatchFunction((int)eDirections.Down, (int)eDirections.None, new BoardPosition(i_Position.Row + (int)eDirections.Down, i_Position.Column), lose);
            }

            return lose;
        }

        private bool leftDiagonalLose(BoardPosition i_Position, bool markAsLoss = false)
        {
            int matches = this.spreadMatchFunction((int)eDirections.Down, (int)eDirections.Left, new BoardPosition(i_Position.Row + (int)eDirections.Down, i_Position.Column + (int)eDirections.Left)) +
                      this.spreadMatchFunction((int)eDirections.Up, (int)eDirections.Right, new BoardPosition(i_Position.Row + (int)eDirections.Up, i_Position.Column + (int)eDirections.Right)) + 1;
            bool lose = matches >= r_Size;

            if (lose)
            {
                this.markPositionAsPartOfLosingStreak(i_Position);
                this.spreadMatchFunction((int)eDirections.Down, (int)eDirections.Left, new BoardPosition(i_Position.Row + (int)eDirections.Down, i_Position.Column + (int)eDirections.Left), lose);
                this.spreadMatchFunction((int)eDirections.Up, (int)eDirections.Right, new BoardPosition(i_Position.Row + (int)eDirections.Up, i_Position.Column + (int)eDirections.Right), lose);
            }

            return lose;
        }

        private bool rightDiagonalLose(BoardPosition i_Position)
        {
            int matches = this.spreadMatchFunction((int)eDirections.Up, (int)eDirections.Left, new BoardPosition(i_Position.Row + (int)eDirections.Up, i_Position.Column + (int)eDirections.Left)) +
                          this.spreadMatchFunction((int)eDirections.Down, (int)eDirections.Right, new BoardPosition(i_Position.Row + (int)eDirections.Down, i_Position.Column + (int)eDirections.Right)) + 1;
            bool lose = matches >= r_Size;

            if (lose)
            {
                this.markPositionAsPartOfLosingStreak(i_Position);
                this.spreadMatchFunction((int)eDirections.Up, (int)eDirections.Left, new BoardPosition(i_Position.Row + (int)eDirections.Up, i_Position.Column + (int)eDirections.Left), lose);
                this.spreadMatchFunction((int)eDirections.Down, (int)eDirections.Right, new BoardPosition(i_Position.Row + (int)eDirections.Down, i_Position.Column + (int)eDirections.Right), lose);
            }

            return lose;
        }


        private void swapTurn()
        {
            if (m_Turn == eTiles.X)
            {
                m_Turn = eTiles.O;
            }
            else
            {
                m_Turn = eTiles.X;
            }
        }

        private void addScore()
        {
            if (m_Turn == eTiles.X)
            {
                m_OScore++;
            }
            else
            {
                m_XScore++;
            }
        }

        private bool fullBoard()
        {
            return m_EmptyPositions.Count == 0;
        }

        private void changeeStatusForLose()
        {
            if (m_Turn == eTiles.X)
            {
                m_eStatus = eStatus.XLost;
            }
            else
            {
                m_eStatus = eStatus.OLost;
            }
        }

        private void changeeStatusForTie()
        {
            m_eStatus = eStatus.Tie;
        }

        internal void Restart()
        {
            m_EmptyPositions.Clear();
            this.initializeBoard();
            m_eStatus = eStatus.Ongoing;
            m_XPositions.Clear();
            m_OPositions.Clear();
            m_Turn = eTiles.X;
        }

        internal Board Copy(BoardPosition i_Position)
        {
            // Transfer a copied board with a a certain Position input, For computer Minimax calculations
            Board copiedBoard = new Board(r_Size);
            copiedBoard.m_Turn = m_Turn;
            copiedBoard.m_XScore = m_XScore;
            copiedBoard.m_OScore = m_OScore;
            copiedBoard.m_eStatus = m_eStatus;
            copiedBoard.m_Board = (int[,])m_Board.Clone();
            copiedBoard.m_XPositions = new LinkedList<BoardPosition>(m_XPositions);
            copiedBoard.m_OPositions = new LinkedList<BoardPosition>(m_OPositions);
            copiedBoard.m_EmptyPositions = new LinkedList<BoardPosition>(m_EmptyPositions);
            copiedBoard.m_ComputerSimulation = m_ComputerSimulation;
            copiedBoard.placeTileOnBoard(i_Position);
            return copiedBoard;
        }

        private void markPositionAsPartOfLosingStreak(BoardPosition i_Position)
        {
            switch (m_Turn)
            {
                case eTiles.X:
                    m_Board[i_Position.Row, i_Position.Column] = (int)eTiles.XLost;
                    break;
                case eTiles.O:
                    m_Board[i_Position.Row, i_Position.Column] = (int)eTiles.OLost;
                    break;
            }
        }

        public int StreakEvaluation(eTiles i_Tile)
        {
            int calculatedStreak = 0;
            int longestStreak = 0;
            int secondaryStreak = 0;

            for (int i = 0; i < Size; i++)
            {
                // rows and cols
                calculatedStreak = this.updatePotential((int)eDirections.Up, (int)eDirections.None, new BoardPosition(0, i), i_Tile);
                longestStreak = this.updateStreak(longestStreak, calculatedStreak, ref secondaryStreak);

                calculatedStreak = this.updatePotential(0, (int)eDirections.Right, new BoardPosition(i, 0), i_Tile);
                longestStreak = this.updateStreak(longestStreak, calculatedStreak, ref secondaryStreak);
            }

            // main diagonals
            calculatedStreak = this.updatePotential((int)eDirections.Up, (int)eDirections.Right, new BoardPosition(0, 0), i_Tile);
            longestStreak = this.updateStreak(longestStreak, calculatedStreak, ref secondaryStreak);

            calculatedStreak = this.updatePotential((int)eDirections.Up, (int)eDirections.Left, new BoardPosition(0, Size - 1), i_Tile);
            longestStreak = this.updateStreak(longestStreak, calculatedStreak, ref secondaryStreak);

            return longestStreak + secondaryStreak;
        }

        private int updateStreak(int i_CurrentLongestStreak, int i_CurrentCalculatedStreak, ref int o_SecondaryStreak)
        {
            if (i_CurrentLongestStreak > i_CurrentCalculatedStreak && o_SecondaryStreak < i_CurrentCalculatedStreak)
            {
                o_SecondaryStreak = i_CurrentCalculatedStreak;
            }

            if (i_CurrentLongestStreak == i_CurrentCalculatedStreak)
            {
                o_SecondaryStreak = i_CurrentCalculatedStreak;
            }
            else if (i_CurrentCalculatedStreak > i_CurrentLongestStreak)
            {
                o_SecondaryStreak = i_CurrentLongestStreak;
                i_CurrentLongestStreak = i_CurrentCalculatedStreak;
            }

            return i_CurrentLongestStreak;
        }
    }
}