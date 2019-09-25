using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;
using ShenDeng.Framework.Application;
using ShenDeng.Framework.DataBase;
using ShenDeng.Framework.Domain;


namespace ShenDeng.Framework.Base
{
    public class CreateSessionFactory
    {
        private static ISessionFactory sessionFactory;
        private static ISession session;
        private static bool execute;

        public static void InitDataBase(bool exec)
        {
            GetSession(exec); //初始化表
            IRepository repository = UnityIoC.Get<IRepository>();
            IAccountService accountService = UnityIoC.Get<IAccountService>();
            if (!repository.IsExisted(new Account.By(AccountIdentifier.of("肖斌武")))) //初始化数据
            {
                accountService.CreateAccount("肖斌武", "SDT34200")
                    .SetPassWord("1")
                    .SetRole(Role.All)
                    .SetCanDelete(false);
                accountService.CreateAccount("王旺玲", "SDT02207")
                    .SetPassWord("1")
                    .SetRole(Role.All)
                    .SetCanDelete(false)
                    .Commit();
            }
        }
        public static ISessionFactory GetSessionFactory()
        {
            if (sessionFactory == null)   //单例模式
            {
                Dictionary<string, string> dict = Conn2Split(); //获取连接字符串信息
                sessionFactory = Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql2008.ConnectionString(c => c
                    .Server(dict["server"])
                    .Database(dict["database"]) //数据库必须已存在，表可以根据Map文件新建
                    .Username(dict["username"])
                    .Password(dict["password"])))
                    .Mappings(m => 
                        {
                            m.FluentMappings.AddFromAssembly(Assembly.Load("ShenDeng.Framework"));
                            m.FluentMappings.AddFromAssembly(Assembly.Load("ShenDeng"));
                        }
                    ) //加载Map文件
                    .ExposeConfiguration(BuildSchema)  //建表
                    .BuildSessionFactory();
            }

            return sessionFactory;
        }

        public static ISession GetSession(bool exec)
        {
            if (session == null)   //单例
            {       
                execute = exec;

                session = GetSessionFactory().OpenSession();
                session.BeginTransaction();
                //CurrentSessionContext.Bind(session);//把session添加到线程中，以后通过GetCurrentSession()来获取。？*？还存在问题！报错：未找到UnityContainer.cs
                return session;
            }
            else
            {
                session.BeginTransaction();   //每次transaction.Commit()后，session会关闭；所以要重新BeginTransaction()来打开session
                return session;
            }

        }
        public static void BuildSchema(Configuration config)
        {
            new SchemaExport(config)   //根据模板创建数据库表
                .Execute(true, execute, false);
        }
        //拆分连接字符串
        private static Dictionary<string, string> Conn2Split()
        {
            string connectionstring = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
            string[] splits = connectionstring.Split(new[] { ' ' });
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var i in splits)
            {
                string[] kv = i.Split(new[] { '=' });
                dict[kv[0].ToLower().Trim()] = kv[1].Trim();
            }

            return dict;
        }
    }
}
