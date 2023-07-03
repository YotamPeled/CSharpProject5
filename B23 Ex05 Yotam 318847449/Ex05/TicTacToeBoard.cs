using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Ex05
{
    class TicTacToeBoard : Form
    {
        private Button m_ButtonNewGame;
        private Button m_ButtonChangeSize;
        private Button m_ButtonHint;
        private Button m_ButtonChangeStartingPlayer;
        private string m_Player1Name;
        private string m_Player2Name;
        private Label m_Player1 = new Label();
        private Label m_Player2 = new Label();
        private Board m_Board;
        private List<TicTacToeButton> m_BoardButtonsList;
        private const int k_ButtonSize = 80;
        bool m_HintWasJustUsed = false;
        bool m_Initialized;

        public Board GameBoard
        {
            get
            {
                if (m_Board == null)
                {
                    throw new NullReferenceException("game board is not initialized");
                }

                return m_Board;
            }
        }
        public TicTacToeBoard()
        {
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Text = "TicTacToeMisere";
        }



        private void setGameSpecifications(string i_Player1Name, string i_Player2Name, eOpponent i_Opponent, int i_BoardSize)
        {
            if (i_Player1Name == string.Empty)
            {
                m_Player1Name = "Player1";
            }
            else
            {
                m_Player1Name = i_Player1Name;
            }

            if (i_Player2Name == string.Empty)
            {
                m_Player2Name = "Player2";
            }
            else
            {
                m_Player2Name = i_Player2Name;
            }

            if (!m_Initialized)
            {
                m_Board = new Board(i_BoardSize);
            }
            else
            {
                m_Board.SetNewBoardSize(i_BoardSize);
            }
            m_Board.Opponent = i_Opponent;             
        }

        public void initialize(string i_Player1Name, string i_Player2Name, eOpponent i_Opponent, int i_BoardSize)
        {
            this.Controls.Clear();

            setGameSpecifications(i_Player1Name, i_Player2Name, i_Opponent, i_BoardSize);

            m_BoardButtonsList = new List<TicTacToeButton>(m_Board.Size);

            if (!m_Initialized)
            {
                m_Board.UpdatePosition += m_BoardButtonList_UpdatePosition;
                m_Board.GameEnd += m_BoardButtonList_GameEnd;
                m_Board.HighlightHintPosition += m_BoardButtonList_HighlightHintPosition;
            }

            initializeBoardComponents();
        }

        private void initializeBoardComponents()
        {
            initializeTicTacToeButtonsAndBoard(m_Board.Size);
            this.StartPosition = FormStartPosition.CenterScreen;
            int boardHeight = (k_ButtonSize + 10) * m_Board.Size - 5;

            m_ButtonHint = new Button
            {
                Text = "Hint",
                Size = new Size(k_ButtonSize, 30),
                Location = new Point(10, boardHeight + 15),
                TabStop = false               
            };

            m_ButtonChangeStartingPlayer = new Button
            {
                Text = "Change Starting Player",
                Size = new Size(k_ButtonSize * 3, 30),
                Location = new Point(10, boardHeight + 50),
                TabStop = false
            };

            m_ButtonChangeSize = new Button
            {
                Text = "Change Size",
                Size = new Size(k_ButtonSize - 10, 40),
                Location = new Point(m_BoardButtonsList[m_Board.Size - 1].Left + 5, m_ButtonHint.Location.Y - 7),
                TabStop = false
            };

            m_ButtonNewGame = new Button
            {
                Text = "New Game",
                Size = new Size(k_ButtonSize - 10, 40),
                Location = new Point(m_BoardButtonsList[m_Board.Size - 1].Left + 5, m_ButtonChangeStartingPlayer.Location.Y - 2),
                TabStop = false
            };

            m_Player1.Name = "player1";
            m_Player1.AutoSize = true;
            m_Player1.Width = k_ButtonSize;
            m_Player1.Location = new Point(m_ButtonHint.Right + 10, boardHeight + 23);

            m_Player2.Name = m_Board.Opponent == eOpponent.Friend ? "player2" : "Computer";
            m_Player2.AutoSize = true;
            m_Player2.Width = k_ButtonSize;
            m_Player2.Location = new Point(m_Player1.Right, boardHeight + 23);
            
            if (m_Board.Opponent == eOpponent.Friend)
            {
                m_ButtonHint.Enabled = false;
            }
            else
            {
                m_ButtonHint.Click += m_HintButton_Click;
            }

            m_ButtonChangeStartingPlayer.Click += m_ButtonChangeStartingPlayer_Click;
            m_ButtonChangeSize.Click += m_ButtonChangeSize_Click;
            m_ButtonNewGame.Click += m_ButtonNewGame_Click;

            updateLabels();
            Controls.Add(m_ButtonHint);
            Controls.Add(m_ButtonChangeStartingPlayer);
            Controls.Add(m_ButtonChangeSize);
            Controls.Add(m_ButtonNewGame);
            Controls.Add(m_Player1);
            Controls.Add(m_Player2);
            m_Initialized = true;
        }

        private void m_ButtonNewGame_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            this.Close();
        }

        private void m_ButtonChangeSize_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Retry;
            this.Close();
        }

        private void m_ButtonChangeStartingPlayer_Click(object sender, EventArgs e)
        {
            m_ButtonChangeStartingPlayer.Enabled = false;
            m_ButtonChangeSize.Enabled = false;

            m_Board.ChangeStartingPlayer();
            if (m_Board.Opponent == eOpponent.Friend)
            {
                swapBoldLabels();
            }
            else
            {
                m_Board.ComputerTurn();
            }
        }

        private void m_HintButton_Click(object sender, EventArgs e)
        {
            m_HintWasJustUsed = true;
            m_ButtonHint.Enabled = false;

            m_Board.doWhenHintIsUsed();
        }

        private void initializeTicTacToeButtonsAndBoard(int i_Size)
        {
            int spaceBetweenButtons = k_ButtonSize + 10;
            int boardWidth = i_Size * spaceBetweenButtons+ 20;
            int boardHeight = i_Size * spaceBetweenButtons + 85;
            this.ClientSize = new Size(boardWidth - 10, boardHeight);

            for (int row = 0; row < i_Size; row++)
            {
                for (int column = 0; column < i_Size; column++)
                {
                    TicTacToeButton boardButton = new TicTacToeButton(row, column);
                    boardButton.Size = new Size(k_ButtonSize, k_ButtonSize);
                    boardButton.Location = new Point(column * spaceBetweenButtons + 10, row * spaceBetweenButtons + 10);
                    boardButton.Font = new Font(boardButton.Font.FontFamily, 20);
                    boardButton.Click += tictactoeButton_Click;
                    boardButton.TabStop = false;

                    m_BoardButtonsList.Add(boardButton);
                    this.Controls.Add(boardButton);
                }
            }
        }

        private void tictactoeButton_Click(object sender, EventArgs e)
        {
            m_ButtonChangeStartingPlayer.Enabled = false;
            m_ButtonChangeSize.Enabled = false;

            BoardPosition buttonPosition = (sender as TicTacToeButton).Position;

            m_Board.DoWhenButtonWasClicked(buttonPosition);

            if (m_Board.Opponent == eOpponent.Computer && m_Board.Turn == eTiles.O)
            {
                this.Refresh();
                m_Board.ComputerTurn();
            }
        }

        private void m_BoardButtonList_UpdatePosition(BoardPosition i_Position, eTiles i_Tile)
        {
            int matchClickedAndMostRecent = 0;

            foreach(TicTacToeButton button in m_BoardButtonsList)
            {
                if (button.Position.Equals(i_Position))
                {
                    button.BackColor = Color.LightBlue;
                    button.Text = i_Tile.ToString();
                    button.Enabled = false;
                    matchClickedAndMostRecent++;
                }
                else if (button.BackColor == Color.LightBlue)
                {
                    button.BackColor = default;
                    matchClickedAndMostRecent++;
                }
            }

            if (m_HintWasJustUsed)
            {
                updateVisualsForEntireBoard();
                m_HintWasJustUsed = false;
            }

            if (m_Board.Opponent == eOpponent.Friend)
            {
                swapBoldLabels();
            }       
        }

        private void m_BoardButtonList_HighlightHintPosition(BoardPosition i_Position)
        {
            foreach (TicTacToeButton button in m_BoardButtonsList)
            {
                if (button.Position.Equals(i_Position))
                {
                    button.BackColor = Color.LightGreen;
                }
            }
        }

        private void swapBoldLabels()
        {
            if (m_Player1.Font.Bold)
            {
                m_Player2.Font = new Font(Label.DefaultFont, FontStyle.Bold);
                m_Player1.Font = new Font(Label.DefaultFont, FontStyle.Regular);
            }
            else
            {
                m_Player1.Font = new Font(Label.DefaultFont, FontStyle.Bold);
                m_Player2.Font = new Font(Label.DefaultFont, FontStyle.Regular);
            }
        }

        private void resetBoldLabel()
        {
            m_Player1.Font = new Font(Label.DefaultFont, FontStyle.Bold);
            m_Player2.Font = new Font(Label.DefaultFont, FontStyle.Regular);
        }


        private void m_BoardButtonList_GameEnd()
        {
            updateVisualsForEntireBoard();
            this.Refresh();
            initializeMessageBox();
        }

        private void initializeMessageBox()
        {
            eStatus endStatus = m_Board.Status;
            string caption = string.Empty;

            if (endStatus == eStatus.XLost)
            {
                caption = $"{m_Player1Name} Lost!";
            }
            else if (endStatus == eStatus.OLost)
            {
                caption = $"{m_Player2Name} Lost!";
            }
            else if (endStatus == eStatus.Tie)
            {
                caption = "A Tie!";
            }

            string message = $@"{caption}
Would you like to play another round?";

            DialogResult result = MessageBox.Show(ActiveForm, message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            ActiveForm.StartPosition = this.StartPosition;

            if (result == DialogResult.Yes)
            {
                restartUI();
            }
            else if (result == DialogResult.No)
            {
                this.Close();
            }
        }

        private void restartUI()
        {
            m_Board.Restart();
            updateVisualsForEntireBoard();
            updateLabels();
            if (m_Board.Opponent == eOpponent.Computer)
            {
                m_ButtonHint.Enabled = true;
            }

            m_ButtonChangeSize.Enabled = true;
            m_ButtonChangeStartingPlayer.Enabled = true;
        }

        private void updateLabels()
        {
            resetBoldLabel();
 
            m_Player1.Text = $@"{m_Player1Name}: {m_Board.XScore}";
            m_Player2.Text = $@"{m_Player2Name}: {m_Board.OScore}";
        }

        private void setButtonVisuals(TicTacToeButton i_Button)
        {
            int TileType = m_Board.BoardState[i_Button.Position.Row, i_Button.Position.Column];

            if (TileType == (int)eTiles.empty)
            {
                i_Button.Text = string.Empty;
                i_Button.Enabled = true;
            }
            else if (TileType == (int)eTiles.X)
            {
                i_Button.Text = "X";
                i_Button.Enabled = false;
            }
            else if (TileType == (int)eTiles.O)
            {
                i_Button.Text = "O";
                i_Button.Enabled = false;
            }
            else if (TileType == (int)eTiles.XLost)
            {
                i_Button.Text = "X";
                i_Button.BackColor = Color.Red;
                i_Button.Enabled = false;
            }
            else if (TileType == (int)eTiles.OLost)
            {
                i_Button.Text = "O";
                i_Button.BackColor = Color.Red;
                i_Button.Enabled = false;
            }

            if (TileType != (int)eTiles.OLost && TileType != (int)eTiles.XLost && i_Button.Enabled == true)
            {
                i_Button.BackColor = Color.White;
            }
        }

        private void updateVisualsForEntireBoard()
        {
            foreach (TicTacToeButton button in m_BoardButtonsList)
            {
                setButtonVisuals(button);
            }
        }
    }
}
