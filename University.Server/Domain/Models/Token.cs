namespace University.Server.Domain.Models
{
    public class Token
    {
        public string Value { get; set; }

        public Token(string token)
        {
            Value = token;
        }
    }
}