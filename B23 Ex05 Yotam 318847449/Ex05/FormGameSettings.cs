using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ex05
{
    public partial class FormGameSettings : Form
    {
        private bool m_NumericUpDownIsWide = false;
        private TicTacToeBoard m_GameBoard = null;

        public string Player1Name
        {
            get
            {
                return textBoxPlayer1.Text;
            }
        }

        public string Player2Name
        {
            get
            {
                return textBoxPlayer2.Text;
            }
        }

        public int SelectedBoardSize
        {
            get
            {
                return (int)numericUpDownRows.Value;
            }
        }

        public eOpponent SelectedOpponent
        {
            get
            {           
                    return checkBoxPlayer2.Checked ? eOpponent.Friend : eOpponent.Computer;      
            }
        }

        public FormGameSettings()
        {
            InitializeComponent();
        }

        private void checkBoxPlayer2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxPlayer2.Checked)
            {
                textBoxPlayer2.Enabled = true;
                textBoxPlayer2.Text = string.Empty;

            }
            else
            {
                textBoxPlayer2.Enabled = false;
                textBoxPlayer2.Text = "Computer";
            }
        }

        private void numericUpDownRows_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownCols.Value = numericUpDownRows.Value;

            resizeNumericUpDownAccordingToValue((int)numericUpDownRows.Value);
        }

        private void numericUpDownCols_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownRows.Value = numericUpDownCols.Value;
        }

        private void resizeNumericUpDownAccordingToValue(int i_Value)
        {
            if (i_Value == 10)
            {
                numericUpDownRows.Width = (int)(numericUpDownRows.Width * 1.35);
                numericUpDownCols.Width = numericUpDownRows.Width;
                m_NumericUpDownIsWide = true;
            }
            else if (m_NumericUpDownIsWide)
            {
                numericUpDownRows.Width = (int)(numericUpDownRows.Width / 1.35);
                numericUpDownCols.Width = numericUpDownRows.Width;
                m_NumericUpDownIsWide = false;
            }
        }
        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (m_GameBoard == null)
            {
                m_GameBoard = new TicTacToeBoard();    
            }

            m_GameBoard.initialize(Player1Name, Player2Name, SelectedOpponent, SelectedBoardSize);

            this.Hide();
            m_GameBoard.ShowDialog();
            if (m_GameBoard.DialogResult == DialogResult.Retry)
            {
                this.textBoxPlayer1.Enabled = false;
                this.textBoxPlayer2.Enabled = false;
                this.checkBoxPlayer2.Enabled = false;     
            }

            this.DialogResult = m_GameBoard.DialogResult;
            this.Close();      
        }
    }
}
