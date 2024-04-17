using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Vocab.Core.Entities
{
    public class User()
    {
        public required Guid Id { get; init; }
        public required string Username { get; init; }
        public required string Firstname { get; init; }
        public string? Lastname { get; init; }
        public required string Email { get; init; }
        public required bool IsEmailVerified { get; init; }
        public required ReadOnlyCollection<string> Roles { get; init; }

        public bool IsInRole(string roleName) => Roles.Contains(roleName);

        [SetsRequiredMembers]
        public User(Guid id, string username, string firstname, string? lastname, string email, bool isEmailVerified, ReadOnlyCollection<string> roles) : this()
        {
            Id = id;
            Username = username;
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
            IsEmailVerified = isEmailVerified;
            Roles = roles;
        }
    }
}
