public enum eTiles
{
    empty,
    X,
    O,
    XLost,
    OLost
}

public enum eStatus
{
    Ongoing,
    OLost,
    XLost,
    Tie
}

internal enum eDirections
{
    None = 0,
    Right = 1,
    Left = -1,
    Up = 1,
    Down = -1
}

public enum eOpponent
{
    Friend = 1,
    Computer = 2,
}