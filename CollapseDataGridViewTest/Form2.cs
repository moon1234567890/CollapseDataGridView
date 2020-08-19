using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CollapseDataGridViewTest
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            var dataStr = File.ReadAllText(@"C:\test\CollapseDataGridViewTest\CollapseDataGridViewTest\Data.json");
            var obj = JObject.Parse(dataStr);
            var reportKeys = (JObject)obj["ReportKeys"];
            var lowObject = (JObject)reportKeys["Low"];
            this.cdgv_abs.AddColumns((JObject)lowObject["One"].First);

            cdgv_abs.Columns.Insert(0, SetCheckBoxColumn("LoneCheckBox", "Check"));
            cdgv_abs.Columns.Insert(1, SetButtonColumn("loadOneWithTCS", "Load(WithTCS)", "Load(WithTCS)"));
            cdgv_abs.Columns.Insert(2, SetButtonColumn("loadOneWithOutTCS", "Load(WithOutTCS)", "Load(WithOutTCS)"));
            cdgv_abs.AddData((JArray)lowObject["One"],3,3);
            //foreach (var item in lowObject["One"])
            //{
            //    var childList = new List<JObject>();
            //    for (int i = 0; i < 3; i++)
            //    {
            //        childList.Add((JObject)item);
            //    }

            //    CollDataGridViewRow collapseRow = new CollDataGridViewRow();
            //    collapseRow.IsCollapse = true;
            //    collapseRow.GroupTag = childList;

            //    DataGridViewCheckBoxCell cell = new DataGridViewCheckBoxCell();
            //    cell.Value = false;
            //    collapseRow.Cells.Add(cell);//不预先添加则无法折叠--相当于插入了空行

            //    for (int i=1; i<4; i++)
            //    {
            //        DataGridViewRow row = new DataGridViewRow();
            //        cell = new DataGridViewCheckBoxCell();
            //        cell.Value = false;
            //        row.Cells.Add(cell);//不预先添加则无法折叠
            //        collapseRow.Rows.Add(row);
            //    }

            //    var rownum = cdgv_abs.Rows.Add(collapseRow);
            //    cdgv_abs.Expand(rownum);

            //    //Add Data
            //    for (int i = rownum; i < rownum+4; i++)
            //    {
            //        if (i == rownum)
            //        {
            //            DataGridViewTextBoxCell cellTextBox = new DataGridViewTextBoxCell();
            //            cellTextBox.Value = "";
            //            this.cdgv_abs.Rows[i].Cells[0]= cellTextBox;
            //            DataGridViewTextBoxCell cellTextBox2 = new DataGridViewTextBoxCell();
            //            cellTextBox2.Value = "";
            //            this.cdgv_abs.Rows[i].Cells[1] = cellTextBox2;
            //            DataGridViewTextBoxCell cellTextBox3 = new DataGridViewTextBoxCell();
            //            cellTextBox3.Value = "";
            //            this.cdgv_abs.Rows[i].Cells[2] = cellTextBox3;
            //        }
            //        int j = 3;
            //        foreach (JProperty item2 in item)
            //        {
            //            this.cdgv_abs.Rows[i].Cells[j].Value = item2.Value;
            //            j++;
            //        }
            //    }
            //}
        }

        private DataGridViewButtonColumn SetButtonColumn(string name, string headerText, string value)
        {
            DataGridViewButtonColumn result = new DataGridViewButtonColumn();
            result.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            result.Width = 100;
            result.Name = name;
            result.HeaderText = headerText;
            result.DefaultCellStyle.NullValue = value;
            return result;
        }

        private DataGridViewCheckBoxColumn SetCheckBoxColumn(string name, string headerText)
        {
            DataGridViewCheckBoxColumn result = new DataGridViewCheckBoxColumn();
            result.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            result.Width = 50;
            result.Name = name;
            result.HeaderText = headerText;
            return result;
        }
    }
}
