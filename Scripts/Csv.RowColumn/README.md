# 数据说明

* column.csv : 输入的`csv`文件,需要做以下转换

   >* 文件表头改为 Label,Category,Image,Value,Date,即要和CsvCloumn类属性对应(顺序和大小写不影响)
   >* 日期和值按列排序,日期的格式需要先转换为`yyyy-mm-dd`格式
  
* row.csv : 输出的`csv`文件,其中日期按行排列,对应的值也按行排序