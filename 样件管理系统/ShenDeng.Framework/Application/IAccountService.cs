using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShenDeng.Framework.Domain;


namespace ShenDeng.Framework.Application
{
   public interface IAccountService
   {
       IAccountCommand CreateAccount(string username, string jobnumber);
       IEnumerable<Account> GetAllAccount();
       Account GetOneAccount(AccountIdentifier id);
       Account GetOneAccountByjobnumber(string jobnumber);

       bool Verify(string username, string password);
       void Delete(AccountIdentifier id);
       void ResetPassword(string username);
       void ChangePassword(string oldpwd, string newpwd);
       IAccountCommand EditAccount(AccountIdentifier id);
       Image GetImage();
       void Commit();
   }
}
