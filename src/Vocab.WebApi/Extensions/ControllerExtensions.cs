using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Text.Json;
using Throw;
using Vocab.Core.Entities;

namespace Vocab.WebApi.Extensions
{
    internal static class ControllerExtensions
    {
        public static User GetUserProfile(this ControllerBase controller)
        {
            string realmAccessJson = controller.User.FindFirstValue("realm_access") ?? throw new ArgumentNullException("Не был получен список ролей из токена.");

            User user = new()
            {
                Id = GetUserGuid(controller),
                Email = controller.User.FindFirstValue(ClaimTypes.Email) ?? throw new ArgumentNullException("Не был получен Email из токена."),
                Firstname = controller.User.FindFirstValue(ClaimTypes.GivenName) ?? throw new ArgumentNullException("Не был получено имя из токена."),
                Lastname = controller.User.FindFirstValue(ClaimTypes.Surname),
                Username = controller.User.FindFirstValue("preferred_username") ?? throw new ArgumentNullException("Не был получен Username из токена."),
                IsEmailVerified = Convert.ToBoolean(controller.User.FindFirstValue("email_verified")),
                Roles = GetRoles(realmAccessJson)
        };

            return user;
        }
        public static Guid GetUserGuid(this ControllerBase controller)
        {
            string guidStr = controller.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new ArgumentNullException("Не был получен Guid из токена.");
            return Guid.Parse(guidStr);
        }

        private static ReadOnlyCollection<string> GetRoles(string json)
        {
            using var realmAccess = JsonDocument.Parse(json);

            var rolesJsonArray = realmAccess.RootElement.GetProperty("roles").EnumerateArray();
            var roles = rolesJsonArray.Select(js => js.GetString())
                .ThrowIfNull().IfEmpty().IfHasNullElements()
                .Value.ToList().AsReadOnly();
            return roles!;
        }
    }

}
