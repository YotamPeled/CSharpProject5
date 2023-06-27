public struct BoardPosition
{
    private int m_RowPosition;
    private int m_ColumnPosition;

    internal BoardPosition(int i_RowPosition, int i_ColumnPosition)
    {
        m_RowPosition = i_RowPosition;
        m_ColumnPosition = i_ColumnPosition;
    }

    internal ComputerPositionNode ToComputerPositionNode(int i_Evaluation = 0)
    {
        return new ComputerPositionNode(m_RowPosition, m_ColumnPosition, i_Evaluation);
    }

    public int Row
    {
        get { return m_RowPosition; }
    }

    public int Column
    {
        get { return m_ColumnPosition; }
    }

    public static BoardPosition InvalidPosition()
    {
        return new BoardPosition(-1, -1);
    }

}
