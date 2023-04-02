namespace University.Server.Domain.Models
{
    public class Token : Base
    {
        public string Value { get; set; }

        public Token(string token)
        {
            Value = token;
        }
    }
}