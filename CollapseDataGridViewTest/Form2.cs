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
            var dataStr = File.ReadAllText(System.Environment.CurrentDirectory+@"\Data.json");
            var obj = JObject.Parse(dataStr);
            var reportKeys = (JObject)obj["ReportKeys"];
            var lowObject = (JObject)reportKeys["Low"];
            this.cdgv_abs.AddColumns((JObject)lowObject["One"].First);
            this.cdgv_abs.CellEndEdit += Cdgv_abs_CellEndEdit;
            cdgv_abs.Columns.Insert(0, SetCheckBoxColumn("LoneCheckBox", "Check"));
            cdgv_abs.Columns.Insert(1, SetButtonColumn("loadOneWithTCS", "Load(WithTCS)", "Load(WithTCS)"));
            cdgv_abs.Columns.Insert(2, SetButtonColumn("loadOneWithOutTCS", "Load(WithOutTCS)", "Load(WithOutTCS)"));
            cdgv_abs.AddData((JArray)lowObject["One"],3,3);
        }

        private void Cdgv_abs_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var row = this.cdgv_abs.Rows[e.RowIndex];
            if (!(row is CollDataGridViewRow))
            {
                var fatherRowIndex = 0;
                for (int i = e.RowIndex-1; i >=0; i--)
                {
                    if (this.cdgv_abs.Rows[i] is CollDataGridViewRow)
                    {
                        fatherRowIndex = i;
                        break;
                    }
                }
                var fatherRow = this.cdgv_abs.Rows[fatherRowIndex] as CollDataGridViewRow;
                double fatherValue=0;
                foreach (var item in fatherRow.Rows)
                {

                    if (item.Cells[e.ColumnIndex].Value != null&&!string.IsNullOrEmpty(item.Cells[e.ColumnIndex].Value.ToString()))
                    {
                        fatherValue += Convert.ToDouble(item.Cells[e.ColumnIndex].Value.ToString());
                    }
                }
                fatherRow.Cells[e.ColumnIndex].Value = fatherValue;
            }
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
