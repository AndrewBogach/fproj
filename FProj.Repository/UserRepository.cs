using FProj.Api;
using FProj.Data;
using FProj.Repository.Base;
using System.Linq;
using System.Web.Helpers;

namespace FProj.Repository
{
    public class UserRepository : BaseRepository
    {
        public UserRepository(FProjContext context) : base(context)
        {
        }

        public UserApi Default()
        {
            return new UserApi();
        }

        public UserApi Add(UserApi user, string password)
        {
            if (IsEmailUsed(user.Email)) return null;

            var userData = ApiToData.UserApiToData(user);
            var salt = Crypto.GenerateSalt();
            var account = new UserAccount()
            {
                Email = user.Email,
                Password = Crypto.HashPassword(password + salt),
                Salt = salt
            };
            _dbContext.UserAccount.Add(account);
            _dbContext.SaveChanges();

            userData.Id = account.Id;
            _dbContext.User.Add(userData);
            _dbContext.SaveChanges();

            return DataToApi.UserToApi(userData);
        }

        public bool IsEmailUsed(string email)
        {
            return _dbContext.UserAccount.Any(x => x.Email == email);
        }

        public UserApi GetUserByEmail(string email)
        {
            var user = _dbContext.User.FirstOrDefault(x => x.UserAccount.Email == email);

            return user == null ? null : DataToApi.UserToApi(user);
        }

        public UserApi UserVerification(string email, string password)
        {
            var account = _dbContext.UserAccount.FirstOrDefault(x => x.Email == email);

            return account != null && Crypto.VerifyHashedPassword(account.Password, password + account.Salt) ? GetUserByEmail(email) : null;
        }
    }
}
