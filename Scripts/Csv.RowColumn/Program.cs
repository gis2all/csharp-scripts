using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Utils;
using Utils.Core;

namespace CSV.RowColumn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var csvList = FileUtils.ReadCsvFile(FileUtils.GetProjectPath() + @"data\cloumn.csv");
            var dateList = FileUtils.GetUniqueDateList(csvList);
            var outList = ConvertColumnToRow(csvList, dateList);
            FileUtils.WriteCsvFile(outList, FileUtils.GetProjectPath() + @"output\row.csv", dateList);
        }

        private static List<CsvRow> ConvertColumnToRow(List<CsvCloumn> csvList, List<string> dateList)
        {
            // 按Label分组
            var groupList = (from csv in csvList group csv by csv.Label).ToList();
            // 获取唯一时间           
            var outList = new List<CsvRow>();
            for (int i = 0; i < groupList.Count; i++)
            {
                var innerList = new List<string>();
                var labelGroup = groupList[i].ToList();

                // 这三个变量是不变的
                var rowCsv = new CsvRow();
                rowCsv.Label = labelGroup.FirstOrDefault().Label;
                rowCsv.Category = labelGroup.FirstOrDefault().Category;
                rowCsv.Image = labelGroup.FirstOrDefault().Image;

                // 获取时间对应的值
                var valueList = new List<string>();
                for (int j = 0; j < labelGroup.Count; j++)
                {
                    var dateTime = labelGroup[j].Date;
                    var value = labelGroup[j].Value;
                    rowCsv[dateTime] = value;
                    valueList.Add(dateTime);
                }

                // 没有值则以0补充                
                for (int p = 0; p < dateList.Count; p++)
                {
                    if (!valueList.Contains(dateList[p]))
                    {
                        rowCsv[dateList[p]] = 0.ToString();
                    }
                }
                // 获取每一行的值
                outList.Add(rowCsv);
            }
            return outList;
        }
    }
}
