using System.Windows.Forms;

namespace Ex05
{
    internal class TicTacToeButton : Button
    {
        BoardPosition m_Position;
        internal BoardPosition Position 
        { 
            get
            {
                return m_Position;
            }
        }

        internal TicTacToeButton(int i_Row, int i_Column) : base()
        {
            m_Position = new BoardPosition(i_Row, i_Column);
        }
    }
}
