using System;

namespace AzureDevOpsRest
{
    public struct PersonalAccessToken : IEquatable<PersonalAccessToken>
    {
        private readonly string? _token;
        
        private PersonalAccessToken(string? token)
        {
            if (!string.IsNullOrEmpty(token) && token.Length != 52)
            {
                throw new ArgumentException("Token is expected to be empty for public projects or having length of 52 for private projects", nameof(token));
            }
            
            _token = token;
        }

        public static PersonalAccessToken Empty { get; } 
            = new PersonalAccessToken(string.Empty);
        public static implicit operator string(PersonalAccessToken token) 
            => token.ToString();
        public static implicit operator PersonalAccessToken(string token) 
            => FromString(token);
        public static PersonalAccessToken FromString(string? token) 
            => new PersonalAccessToken(token);
        public override string ToString() => _token ?? string.Empty;
        public override int GetHashCode() 
            => ToString().GetHashCode(StringComparison.Ordinal);
        public override bool Equals(object? obj) 
            => obj is PersonalAccessToken other && Equals(other);
        public bool Equals(PersonalAccessToken other)
            => ToString().Equals(other.ToString(), StringComparison.Ordinal);
        public static bool operator ==(PersonalAccessToken left, PersonalAccessToken right) 
            => left.Equals(right);
        public static bool operator !=(PersonalAccessToken left, PersonalAccessToken right) 
            => !(left == right);
    }
}