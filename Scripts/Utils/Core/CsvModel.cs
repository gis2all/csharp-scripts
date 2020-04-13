using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.Core
{
    public class CsvCloumn
    {
        [Name("Label")]
        public string Label { get; set; }

        [Name("Category")]
        public string Category { get; set; }

        [Name("Image")]
        public string Image { get; set; }

        [Name("Value")]
        public string Value { get; set; }

        // 注意这里不能用DateTime类，而要用string类，否则读取失败
        [Name("Date")]
        public string Date { get; set; }
    }

    public class CsvRow
    {
        public string Label { get; set; }

        public string Category { get; set; }

        public string Image { get; set; }

        // 动态索引器 年份
        private Dictionary<string, string> _internalData = new Dictionary<string, string>();
        public string this[string propName]
        {
            get
            {
                return _internalData[propName];
            }
            set
            {
                _internalData[propName] = value;
            }
        }
    }
}
