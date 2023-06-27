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
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            TicTacToeBoard gameBoard = new TicTacToeBoard();

            gameBoard.initialize(Player1Name, Player2Name, SelectedOpponent, SelectedBoardSize);
            this.Hide();
            gameBoard.ShowDialog();
            this.Close();
        }
    }
}
