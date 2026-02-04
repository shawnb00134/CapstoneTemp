using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using Microsoft.AspNetCore.Identity;


namespace CAMCMSServer.Database.Service
{

    public interface IAuthenticationService
    {
        Task<User?> AuthenticateUser(string username, string password);
    }
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository repository;
        private readonly PasswordHasher<User> passwordHasher;

        public AuthenticationService(IUserRepository repository)
        {
            this.repository = repository;
            this.passwordHasher = new PasswordHasher<User>();
        }

        public async Task<User?> AuthenticateUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }
            var user = await this.repository.GetUserByUsername(username);
            if (user == null)
            {
                return null;
            }

            var result = this.passwordHasher.VerifyHashedPassword(user, user.Password, password);

            return result == PasswordVerificationResult.Success ? user : null;
        }


    }
}
