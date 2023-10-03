namespace MarketPlaceAdmin.Bussiness.Security
{
    /// <summary>
    /// The Token class represents a token and its expiry time.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// The token value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// The expiry time of the token.
        /// </summary>
        public DateTime Expiry { get; }

        /// <summary>
        /// Initializes a new instance of the Token class.
        /// </summary>
        /// <param name="token">The token value.</param>
        /// <param name="expiry">The expiry time of the token.</param>
        public Token(string token, DateTime expiry)
        {
            Value = token;
            Expiry = expiry;
        }
    }
}
