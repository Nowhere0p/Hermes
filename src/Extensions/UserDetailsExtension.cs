using System.Text;
using Hermes.src.Models;

namespace Hermes.src.Extensions;

    public static class UserDetailsExtention
    {
        public static UserDetails ToUserDetails(this RegistrationInteraction regInteraction)
        {
            return new UserDetails
            {
                Email = regInteraction.Email,
                Password = HashPassword(regInteraction.Password),
                FirstName = regInteraction.FirstName,
                LastName = regInteraction.LastName,
                Gender = regInteraction.Gender,
                Country = regInteraction.Country,
                UserId=Guid.NewGuid().ToString(),
            };
        }

    private static string HashPassword(string password)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
        }
    }