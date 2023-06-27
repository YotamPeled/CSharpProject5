internal struct ComputerPositionNode
{
    private int m_RowPosition;
    private int m_ColumnPosition;
    private int m_StaticEvaluation;

    internal ComputerPositionNode(int i_RowPosition, int i_ColumnPosition, int i_StaticEvaluation)
    {
        m_RowPosition = i_RowPosition;
        m_ColumnPosition = i_ColumnPosition;
        m_StaticEvaluation = i_StaticEvaluation;
    }

    internal int Row
    {
        get { return m_RowPosition; }
    }

    internal int Column
    {
        get { return m_ColumnPosition; }
    }

    internal int Evaluation
    {
        get { return m_StaticEvaluation; }
    }

}
