using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace ShenDeng.Application
{
    public class ShowCPK
    {
        Helper.ISqlHelper sql359 = Helper.HelperFactory.SqlHeler_35_9("MachineOutPutData");
        // 查询数据
        public DataTable GetCPKs(string time, string field)
        {
            string sqlstring = @"select processcode, lineid, hour, createdate, {0} from Test_Process_HH where createdate = '{1}' and processcode='SPICPKCheck' and factoryname='Digital'";
            string sql = string.Format(sqlstring, field.Trim(), time.Trim());
            DataTable dt = sql359.GetDataTable(sql, CommandType.Text);
            return dt;
        }
        // 处理数据
        public List<Dictionary<string, string>> GetResult(DataTable dt, string field)
        {
            var result = new List<Dictionary<string, string>>();
            var line = from r in dt.AsEnumerable()        // 获得线体名
                       select r["lineid"].ToString();
            var lineKey = new HashSet<string>(line);
            foreach (var k in lineKey)     // 为每个线体创建字典
            {
                var dict = new Dictionary<string, string>();
                dict["lineid"] = k;
                for (int i = 1; i <= 24; i++)  // 赋初值
                    dict[i.ToString()] = "";
                result.Add(dict);
            }
            for (int i = 0; i < dt.Rows.Count; i++)   // 获得每个线体的各时间段
            {
                var row = dt.Rows[i];
                var dict = result.Find(x => x["lineid"].ToString() == row["lineid"].ToString());
                dict[row["hour"].ToString().Trim()] = row[field].ToString().Trim();
            }
            return result;
        }
    }
}