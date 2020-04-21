using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Utils.Core;

namespace Utils
{
    public class FileUtils
    {
        public static string GetProjectPath()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return baseDir.Split("bin").FirstOrDefault();
        }

        public static void CreateDir(string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public static List<CsvCloumn> ReadCsvFile(string csvPath)
        {
            List<CsvCloumn> csvCloumns = new List<CsvCloumn>();
            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // 标题大写
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToUpper();
                csvCloumns = csv.GetRecords<CsvCloumn>().ToList();
            }
            return csvCloumns;
        }

        public static void WriteCsvFile(List<CsvRow> csvRowList, string csvPath, List<string> dateList)
        {
            if (File.Exists(csvPath))
            {
                File.Delete(csvPath);
            }
            if (!Directory.Exists(GetProjectPath() + "output"))
            {
                Directory.CreateDirectory(GetProjectPath() + "output");
            }
            // Foreach在list中是按顺序的
            foreach (var csvRow in csvRowList)
            {
                string valueText = null;
                string dateText = null;
                foreach (var date in dateList)
                {

                    if (date == dateList.LastOrDefault())
                    {
                        valueText += csvRow[date];
                        dateText += date;
                    }
                    else
                    {
                        valueText += csvRow[date] + ",";
                        dateText += date + ",";
                    }
                }
                // 写入标题
                if (csvRow == csvRowList.FirstOrDefault())
                {
                    var text = nameof(csvRow.Label) + "," + nameof(csvRow.Category) + "," + nameof(csvRow.Image) + "," + dateText + "\r\n";
                    File.AppendAllText(csvPath, text);
                    Console.WriteLine(text);

                    text = csvRow.Label + "," + csvRow.Category + "," + csvRow.Image + "," + valueText + "\r\n";
                    File.AppendAllText(csvPath, text);
                    Console.WriteLine(text);
                }
                // 写入内容
                else
                {
                    var text = csvRow.Label + "," + csvRow.Category + "," + csvRow.Image + "," + valueText + "\r\n";
                    File.AppendAllText(csvPath, text);
                    Console.WriteLine(text);
                }
            }
        }

        public static List<string> GetUniqueDateList(List<CsvCloumn> csvCloumnList)
        {
            var dateList = new List<string>();
            csvCloumnList.ForEach(p => dateList.Add(p.Date));
            var uniqeDateList = dateList.Distinct().ToList();
            uniqeDateList.Sort();
            return uniqeDateList;
        }
    }
}
