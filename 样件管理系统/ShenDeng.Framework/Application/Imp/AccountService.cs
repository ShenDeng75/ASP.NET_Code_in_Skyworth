using System;
using System.Collections.Generic;
using ShenDeng.Framework.DataBase;
using ShenDeng.Framework.Domain;
using ShenDeng.Framework.Handle;
using ShenDeng.Framework.App_Start;
using System.Drawing;
using System.Web;
using ShenDeng.Framework.Base;

namespace ShenDeng.Framework.Application.Imp
{
    [RegisterToContainer]
    public class AccountService : IAccountService
    {
        public readonly IRepository repository;
        public PassWord_Handle passwordService;
        private IMySessionService sessionService;

        public AccountService(IRepository repository,
            PassWord_Handle passwordService,
            IMySessionService sessionService)
        {
            this.repository = repository;
            this.passwordService = passwordService;
            this.sessionService = sessionService;
        }
        //增
        public IAccountCommand CreateAccount(string username, string jobnumber)
        {
            if (repository.IsExisted(new Account.By(AccountIdentifier.of(username))))
                 throw new Exception("用户名已存在！");
            if (repository.IsExisted(new Account.ByJobNumber(jobnumber)))
                throw new Exception("工号已存在！");
            var account = new Account(AccountIdentifier.of(username));
            account.Job_Numner = jobnumber;
            repository.Save(account);
            return new AccountCommand(account, repository, passwordService);
        }
        //删
        public void Delete(AccountIdentifier id)
        {
            var account = repository.FindOne(new Account.By(id));
            repository.Delete(account);

        }
        //找s
        public IEnumerable<Account> GetAllAccount()
        {
            return repository.FindAll<Account>();
        }
        //找
        public Account GetOneAccount(AccountIdentifier id)
        {
            return repository.FindOne(new Account.By(id));
        }
        //找，通过工号
        public Account GetOneAccountByjobnumber(string jobnumber)
        {
           return repository.FindOne<Account>(new Account.ByJobNumber(jobnumber));
        }
        //改
        public IAccountCommand EditAccount(AccountIdentifier id)
        {
            var account = GetOneAccount(id);
            repository.Save(account);
            return new AccountCommand(account, repository, passwordService);
        }
        // 重置密码
        public void ResetPassword(string username)
        {
            Account account = GetOneAccount(AccountIdentifier.of(username));
            account.PassWord = passwordService.CreateDbPassword("1");
            Commit();
        }
        // 修改密码
        public void ChangePassword(string oldpwd, string newpwd)
        {
            Account account = sessionService.GetAccount();
            if (passwordService.ComparePassword(oldpwd, account.PassWord))
            {
                account.PassWord = passwordService.CreateDbPassword(newpwd);
                Commit();
            }
            else
            {
                throw new Exception("旧密码错误！");
            }
        }
        //证
        public bool Verify(string username, string password)
        {
            Account account;
            if (username.ToLower().StartsWith("sdt")) // 如果用工号登录 
                account = GetOneAccountByjobnumber(username);
            else
                account = GetOneAccount(AccountIdentifier.of(username));
            return passwordService.ComparePassword(password, account.PassWord);
        }
        //生成验证码
        public Image GetImage()
        {
            //得到背景图片。将相对路径转为绝对路径，注 HttpContext 只在.Web类库中有效。
            string path = HttpContext.Current.Server.MapPath("~/Content/img/bg.jpg");   
            Image img = Image.FromFile(path);
            Graphics g = Graphics.FromImage(img);
            //生成并绘制4位随机验证码
            Random rnd = new Random();
            string str = @"1234567890qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM";
            string ans = "";
            Brush[] colors = new Brush[4];
            colors[0] = Brushes.Black;
            colors[1] = Brushes.Blue;
            colors[2] = Brushes.Brown;
            colors[3] = Brushes.ForestGreen;
            for (int i=1; i<5; i++)
            {
               var s = str.Substring(0 + rnd.Next(61), 1);
                ans += s;
               float x =i * 10 - 4;
                //旋转空间
                g.ResetTransform();
                g.TranslateTransform(x, 1);
                g.RotateTransform(rnd.Next(10));
                g.DrawString(s, new Font("黑体", 25f),colors[rnd.Next(4)], new PointF(x, -5f));
                //重置画笔，不然画笔空间会一直旋转
                g = Graphics.FromImage(img);
            }
            //绘制干扰线条
            g.DrawLine(new Pen(Color.SlateGray, 1), 5, 5, 80, 8);
            g.DrawLine(new Pen(Color.BlueViolet, 1), 5, 15, 80, 22);
            //将验证码保存到session中
            sessionService.SetVefCode(ans);
            return img;
        }
        // 提交
        public void Commit()
        {
            var trancation = repository.session.Transaction;
            trancation.Commit();
        }
        
    }
}
