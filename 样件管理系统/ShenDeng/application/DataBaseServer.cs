using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;

namespace ShenDeng.application
{
    public class DataBaseServer
    {
        string connString = @"Server=172.20.5.141; DataBase=SDTMESV2DIGITAL; user=newdb; password=abc.123";
        SqlConnection conn;
        public DataBaseServer()
        {
            conn = new SqlConnection(connString);
        }
        //获取下拉菜单数据
        public HashSet<SelectListItem> GetProcs()
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("select * from MESCfg_Process", conn);
            SqlDataReader reader = cmd.ExecuteReader();

            HashSet<SelectListItem> items = new HashSet<SelectListItem>(); 
            while (reader.Read())
            {
                SelectListItem select = new SelectListItem();
                select.Text = reader["proc_name"].ToString();
                select.Value = reader["proc_code"].ToString();
                items.Add(select);
            }

            conn.Close();
            return items;
        }
        //查询FPY数据
        public List<Dictionary<string, string>> GetFPY(string proc_code, string line_id, string createdata)
        {
            conn.Open();
            string _sqlcmd = "select * from MESProc_FPY_Process_HH where processcode='{0}' and lineid='{1}' and createdate='{2}';";
            string sqlcmd = string.Format(_sqlcmd, proc_code, line_id, createdata); //拼接SQL语句
            SqlCommand cmd = new SqlCommand(sqlcmd, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            var ls = new List<Dictionary<string, string>>();

            while (reader.Read())
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict["id"] = reader["id"].ToString();
                dict["processcode"] = reader["processcode"].ToString().Trim();
                dict["lineid"] = reader["lineid"].ToString().Trim();
                dict["hour"] = reader["hour"].ToString();
                dict["scount"] = reader["scount"].ToString();
                dict["passcount"] = reader["passcount"].ToString();
                dict["failcount"] = reader["failcount"].ToString();
                dict["stime"] = reader["stime"].ToString();
                dict["createdate"] = reader["createdate"].ToString();
                dict["processname"] = reader["processname"].ToString();

                ls.Add(dict);
            }

            conn.Close();
            return ls;
        } 
    }
}
