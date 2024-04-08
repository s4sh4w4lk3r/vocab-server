using System.Collections.ObjectModel;

namespace Vocab.Core.Entities
{
    public class User
    {
        public required Guid Id { get; init; }
        public required string Username { get; init; }
        public required string Firstname { get; init; }
        public required string Lastname { get; init; }
        public required string Email { get; init; }
        public required bool IsEmailVerified { get; init; }
        public required ReadOnlyCollection<string> Roles { get; init; }

        public bool IsInRole(string roleName) => Roles.Contains(roleName);
    }
}
