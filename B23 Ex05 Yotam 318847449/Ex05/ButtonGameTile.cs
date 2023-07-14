using System.Windows.Forms;

namespace Ex05
{
    internal class ButtonGameTile : Button
    {
        BoardPosition m_Position;
        internal BoardPosition Position 
        { 
            get
            {
                return m_Position;
            }
        }

        internal ButtonGameTile(int i_Row, int i_Column) : base()
        {
            m_Position = new BoardPosition(i_Row, i_Column);
        }
    }
}
