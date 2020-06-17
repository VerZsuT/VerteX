namespace VerteX.Lexing
{
    /// <summary>
    /// Тип токена (словесной единица языка).
    /// </summary>
    public enum TokenType
    {
        Id,
        Keyword,
        Number,
        String,
        AssignOperator,
        ArithmeticalOperator,
        Dot,
        LogicOperator,
        UndefinedOperator,
        ComandEnd,
        BeginBrace,
        EndBrace,
        BeginParenthesis,
        EndParenthesis,
        BeginSquereBracket,
        EndSquereBracket,
        Expression,
        Comma,
        Colon,
        Undefined
    }
}