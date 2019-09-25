using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ShenDeng.Framework.Tools
{
    public static class tool
    {
        // 读取Excel文件到DateSet
        public static DataTable Excel2DataTable(string excelPath)
        {
            using (FileStream stream = new FileStream(excelPath, FileMode.Open, FileAccess.Read))//定义文件流
            {
                //首先判断传入的.xls文件还是xlsx文件
                int index = excelPath.LastIndexOf('.');//获取文件扩展名前‘.’的位置
                string extensionName = excelPath.Substring(index + 1);
                IExcelDataReader excelReader;
                if (extensionName == "xls")
                {
                    //传入的xls文件---->97-2003format
                    excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (extensionName == "xlsx")
                {
                    //传入的是xlsx文件---->2007 format
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                else
                {
                    throw new Exception("文件格式错误!");
                }
                DataTable result = excelReader.AsDataSet().Tables[0];
                return result;
            }
        }
    }
}
