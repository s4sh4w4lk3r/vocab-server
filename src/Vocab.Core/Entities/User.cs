using System.Collections.ObjectModel;

namespace Vocab.Core.Entities
{
    public class User(Guid guid, string username, string firstname, string lastname, string email, bool isEmailVerified, List<string> roles)
    {
        public Guid Guid { get; init; } = guid;
        public string Username { get; init; } = username;
        public string Firstname { get; init; } = firstname;
        public string Lastname { get; init; } = lastname;
        public string Email { get; init; } = email;
        public bool IsEmailVerified { get; init; } = isEmailVerified;
        public ReadOnlyCollection<string> Roles { get; init; } = roles.AsReadOnly();

        public bool IsInRole(string roleName) => Roles.Contains(roleName);
    }
}
