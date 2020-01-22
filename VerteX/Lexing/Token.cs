namespace VerteX.Lexing
{
    public class Token
    {
        public TokenType type;
        public string value;

        public Token(TokenType type, string value)
        {
            this.type = type;
            this.value = value;
        }

        public override string ToString()
        {
            if (type == TokenType.String)
            {
                return $"\"{value}\"";
            }
            return value;
        }
    }
}
