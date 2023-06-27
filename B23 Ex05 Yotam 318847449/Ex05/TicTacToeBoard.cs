using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Ex05
{
    class TicTacToeBoard : Form
    {
        private Button m_HintButton;
        private Label m_Player1;
        private Label m_Player2;
        private Board m_Board;
        private List<TicTacToeButton> m_BoardButtonsList;
        private const int k_ButtonSize = 100;
        bool m_HintWasJustUsed = false;

        public TicTacToeBoard()
        {
            m_Board = new Board(3);
            m_BoardButtonsList = new List<TicTacToeButton>(m_Board.Size);
            m_Board.Opponent = eOpponent.Computer;

            m_Board.UpdatePosition += m_BoardButtonList_UpdatePosition;
            m_Board.GameEnd += m_BoardButtonList_GameEnd;
            m_Board.HighlightHintPosition += m_BoardButtonList_HighlightHintPosition;

            initializeBoardComponents();
        }

        private void initializeBoardComponents()
        {
            initializeBoardUI(m_Board.Size);
            this.StartPosition = FormStartPosition.CenterScreen;
            int boardHeight = k_ButtonSize * m_Board.Size;

            m_HintButton = new Button
            {
                Name = "hintButton",
                Text = "Hint",
                Size = new System.Drawing.Size(100, 30),
                Location = new Point(this.Left + 10, boardHeight + 15),
                TabStop = false               
            };

            m_Player1 = new Label
            {
                Name = "player1",
                Text = "Player 1",
                AutoSize = true,
                Width = 100,
                Location = new Point(m_HintButton.Right + 10, boardHeight + 23),
            };

            m_Player2 = new Label
            {
                Name = m_Board.Opponent == eOpponent.Friend ? "player2" : "Computer",
                Text = "Player 2",
                AutoSize = true,
                Width = 100,
                Location = new Point(m_Player1.Right, boardHeight + 23)
            };

            if (m_Board.Opponent == eOpponent.Friend)
            {
                m_HintButton.Enabled = false;
            }
            else
            {
                m_HintButton.Click += m_HintButton_Click;
            }

            updateLabels();
            Controls.Add(m_HintButton);
            Controls.Add(m_Player1);
            Controls.Add(m_Player2);
        }

        private void m_HintButton_Click(object sender, EventArgs e)
        {
            m_HintWasJustUsed = true;
            m_HintButton.Enabled = false;

            m_Board.doWhenHintIsUsed();
        }

        private void initializeBoardUI(int i_Size)
        {
            int boardWidth = i_Size * k_ButtonSize + 20;
            int boardHeight = i_Size * k_ButtonSize + 50;
            this.ClientSize = new Size(boardWidth, boardHeight);

            for (int row = 0; row < i_Size; row++)
            {
                for (int column = 0; column < i_Size; column++)
                {
                    TicTacToeButton boardButton = new TicTacToeButton(row, column);
                    boardButton.Size = new Size(k_ButtonSize, k_ButtonSize);
                    boardButton.Location = new Point(column * k_ButtonSize + 10, row * k_ButtonSize + 10);
                    boardButton.Font = new Font(boardButton.Font.FontFamily, 20);
                    boardButton.Click += boardButton_Click;
                    boardButton.TabStop = false;

                    m_BoardButtonsList.Add(boardButton);
                    this.Controls.Add(boardButton);
                }
            }
        }

        private void boardButton_Click(object sender, EventArgs e)
        {
            BoardPosition buttonPosition = (sender as TicTacToeButton).Position;

            m_Board.DoWhenButtonWasClicked(buttonPosition);

            if (m_Board.Opponent == eOpponent.Computer && m_Board.Turn == eTiles.O)
            {
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
                    button.BackColor = default(Color);
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
            initializeMessageBox();
        }

        private void initializeMessageBox()
        {
            eStatus endStatus = m_Board.Status;
            string caption = string.Empty;

            if (endStatus == eStatus.XLost)
            {
                caption = "X Lost!";
            }
            else if (endStatus == eStatus.OLost)
            {
                caption = "O Lost!";
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
                m_HintButton.Enabled = true;
            }
        }

        private void updateLabels()
        {
            resetBoldLabel();
            string player2 = m_Board.Opponent == eOpponent.Friend ? "player2" : "Computer";
            m_Player1.Text = $@"Player 1: {m_Board.XScore}";
            m_Player2.Text = $@"{player2}: {m_Board.OScore}";
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

            i_Button.BackColor = default(Color);
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
