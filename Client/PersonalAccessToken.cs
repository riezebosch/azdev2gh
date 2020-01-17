using System;

namespace AzureDevOpsRest
{
    public class PersonalAccessToken
    {
        private readonly string _token;

        private PersonalAccessToken(string token)
        {
            if (token.Length != 0 && token.Length != 52)
            {
                throw new ArgumentException("Token is expected to be empty for public projects or having length of 52 for private projects", nameof(token));
            }
            
            _token = token;
        }

        public static PersonalAccessToken Empty { get; } = new PersonalAccessToken(string.Empty);
        public static implicit operator string(PersonalAccessToken token) => token?.ToString() ?? string.Empty;
        public static implicit operator PersonalAccessToken(string token) => FromString(token);
        public static PersonalAccessToken FromString(string token) => token != null ? new PersonalAccessToken(token) : Empty;
        public override string ToString() => _token;
    }
}