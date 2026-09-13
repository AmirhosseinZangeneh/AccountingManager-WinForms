using AccountingManager.Business.Security;
using AccountingManager.DataLayer;
using AccountingManager.DataLayer.Context;
using System;
using System.Linq;

namespace AccountingManager.Business
{
    public class AuthenticationService
    {
        public bool Authenticate(string userName, string password)
        {
            string normalizedUserName = (userName ?? string.Empty).Trim();

            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                Login login = unitOfWork.LoginRepository
                    .Get(item => item.UserName == normalizedUserName)
                    .SingleOrDefault();

                if (login == null || !PasswordHasher.Verify(password, login.Password))
                {
                    return false;
                }

                if (!PasswordHasher.IsHashed(login.Password))
                {
                    login.Password = PasswordHasher.Hash(password);
                    unitOfWork.LoginRepository.Update(login);
                    unitOfWork.Save();
                }

                return true;
            }
        }

        public string GetUserName()
        {
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                Login login = unitOfWork.LoginRepository.Get().FirstOrDefault();
                return login == null ? string.Empty : login.UserName;
            }
        }

        public void UpdateCredentials(string userName, string newPassword)
        {
            string normalizedUserName = (userName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalizedUserName))
            {
                throw new ArgumentException("A username is required.", nameof(userName));
            }

            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                Login login = unitOfWork.LoginRepository.Get().FirstOrDefault();
                if (login == null)
                {
                    login = new Login();
                    unitOfWork.LoginRepository.Insert(login);
                }

                login.UserName = normalizedUserName;
                login.Password = PasswordHasher.Hash(newPassword);
                unitOfWork.Save();
            }
        }
    }
}
