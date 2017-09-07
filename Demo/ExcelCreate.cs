using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using Aspose.Cells;

namespace ETCF
{
    
    /// <summary>
    ///OutFileDao 的摘要说明
    /// </summary>
    public class ExcelCreate
    {
        public ExcelCreate()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }
        /// <summary>
        /// 导出数据到本地
        /// </summary>
        /// <param name="dt">要导出的数据</param>
        /// <param name="tableName">表格标题</param>
        /// <param name="path">保存路径</param>
        public void OutFileToDisk(DataTable dt, string tableName, string path)
        {
            //try
            //{
                Workbook workbook = new Workbook(); //工作簿
                Worksheet sheet = workbook.Worksheets[0]; //工作表
                Cells cells = sheet.Cells;//单元格


                //为标题设置样式    
                Style styleTitle = workbook.Styles[workbook.Styles.Add()];//新增样式
                styleTitle.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                styleTitle.Font.Name = "宋体";//文字字体
                styleTitle.Font.Size = 18;//文字大小
                styleTitle.Font.IsBold = true;//粗体

                //样式2
                Style style2 = workbook.Styles[workbook.Styles.Add()];//新增样式
                style2.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                style2.Font.Name = "宋体";//文字字体
                style2.Font.Size = 10;//文字大小
                style2.Font.IsBold = true;//粗体
                style2.IsTextWrapped = true;//单元格内容自动换行
                style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                //样式3
                Style style3 = workbook.Styles[workbook.Styles.Add()];//新增样式
                style3.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                style3.Font.Name = "宋体";//文字字体
                style3.Font.Size = 9;//文字大小
                style3.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style3.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style3.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style3.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                int Colnum = dt.Columns.Count;//表格列数
                int Rownum = dt.Rows.Count;//表格行数

                //生成行1 标题行   
                //cells.Merge(0, 0, 1, Colnum);//合并单元格
                //cells[0, 0].PutValue(tableName);//填写内容
                //cells[0, 0].SetStyle(styleTitle);
                //cells.SetRowHeight(0, 38);

                //生成行2 列名行
                for (int i = 0; i < Colnum; i++)
                {
                    cells[0, i].PutValue(dt.Columns[i].ColumnName);
                    //if (i == 6)
                    //{
                    //    cells.SetColumnWidth(i, 30);
                    //}
                    cells.SetRowHeight(0, 28);
                }

                //生成数据行
                for (int i = 0; i < Rownum-1; i++)
                {

                    for (int k = 0; k < Colnum; k++)
                    {
                        //if (k == 1)
                        //{
                            cells[1 + i, k].PutValue(dt.Rows[i][k]);
                        //}
                        //else
                        //{
                        //    cells[2 + i, k].PutValue(Convert.ToInt32(dt.Rows[i][k].ToString()));
                        //}

                        //cells[1 + i, k].SetStyle(style3);

                    }
                    cells.SetRowHeight(i, 18);
                }

                workbook.Save(path);
        
        }


        public MemoryStream OutFileToStream(DataTable dt, string tableName)
        {
            Workbook workbook = new Workbook(); //工作簿
            Worksheet sheet = workbook.Worksheets[0]; //工作表
            Cells cells = sheet.Cells;//单元格

            //为标题设置样式    
            Style styleTitle = workbook.Styles[workbook.Styles.Add()];//新增样式
            styleTitle.HorizontalAlignment = TextAlignmentType.Center;//文字居中
            styleTitle.Font.Name = "宋体";//文字字体
            styleTitle.Font.Size = 18;//文字大小
            styleTitle.Font.IsBold = true;//粗体

            //样式2
            Style style2 = workbook.Styles[workbook.Styles.Add()];//新增样式
            style2.HorizontalAlignment = TextAlignmentType.Center;//文字居中
            style2.Font.Name = "宋体";//文字字体
            style2.Font.Size = 12;//文字大小
            style2.Font.IsBold = true;//粗体
            style2.IsTextWrapped = true;//单元格内容自动换行
            style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            //样式3
            Style style3 = workbook.Styles[workbook.Styles.Add()];//新增样式
            style3.HorizontalAlignment = TextAlignmentType.Center;//文字居中
            style3.Font.Name = "宋体";//文字字体
            style3.Font.Size = 9;//文字大小
            style3.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            int Colnum = dt.Columns.Count;//表格列数
            int Rownum = dt.Rows.Count;//表格行数

            //////生成行1 标题行   
            //cells.Merge(0, 0, 1, Colnum);//合并单元格
            //cells[0, 0].PutValue(tableName);//填写内容
            //cells[0, 0].SetStyle(styleTitle);
            //cells.SetRowHeight(0, 38);

            ////生成行2 列名行
            //for (int i = 0; i < Colnum; i++)
            //{
            //    cells[1, i].PutValue(dt.Columns[i].ColumnName);
            //    cells[1, i].SetStyle(style2);
            //    cells.SetRowHeight(1, 30);
            //}

            //生成数据行
            for (int i = 0; i < Rownum; i++)
            {
                for (int k = 0; k < Colnum; k++)
                {
                    cells[2 + i, k].PutValue(dt.Rows[i][k].ToString());
                    cells[2 + i, k].SetStyle(style3);
                }
                cells.SetRowHeight(2 + i, 18);
            }

            MemoryStream ms = workbook.SaveToStream();
            return ms;
        }

    }
}
