using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CollapseDataGridViewTest
{
    public partial class CollDataGridView : DataGridView
    {
        public CollDataGridView()
        {
            InitializeComponent();
        }

        public CollDataGridView(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            this.RowHeadersVisible = true;
            this.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.ImageWidth = 15;
            this.ImageHeight = 15;
            this.RowHeadersDefaultCellStyle.Padding = new Padding(this.RowHeadersWidth);
        }

        #region property
        /// <summary>
        /// 行首图片宽度
        /// </summary>
        [
        Category("CollDataGridViewProperties"),
        Description("行首图片宽度"),
        Bindable(true)
        ]
        public int ImageWidth { get; set; }

        /// <summary>
        /// 行首图片高度
        /// </summary>
        [
        Category("CollDataGridViewProperties"),
        Description("行首图片高度"),
        Bindable(true)
        ]
        public int ImageHeight { get; set; }

        /// <summary>
        /// 行首收缩图标
        /// </summary>
        [
        Category("CollDataGridViewProperties"),
        Description("行首收缩图标"),
        Bindable(true)
        ]
        public Image ImgExpand { get; set; }

        /// <summary>
        /// 行首展开图片
        /// </summary>
        [
        Category("CollDataGridViewProperties"),
        Description("行首展开图片"),
        Bindable(true)
        ]
        public Image ImgCollapse { get; set; }
        #endregion

        protected override void OnRowPostPaint(DataGridViewRowPostPaintEventArgs e)
        {
            base.OnRowPostPaint(e);

            DataGridViewRow row = this.Rows[e.RowIndex];
            Rectangle rectBack = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, this.RowHeadersWidth, e.RowBounds.Height - 1);

            Rectangle rectLineBottom = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y + e.RowBounds.Height - 1, this.RowHeadersWidth, 1);

            if (row is CollDataGridViewRow && (row as CollDataGridViewRow).Rows.Count != 0)
            {
                Rectangle rect = new Rectangle(e.RowBounds.Location.X + 4, e.RowBounds.Location.Y + 4, this.ImageWidth, this.ImageHeight);
                Image img = null;
                if ((row as CollDataGridViewRow).IsCollapse)
                {
                    img = this.ImgExpand;
                }
                else
                {
                    img = this.ImgCollapse;
                }
                e.Graphics.DrawImage(img, rect);
            }
        }

        /// <summary>
        /// 增加点击行首折叠功能
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowHeaderMouseClick(DataGridViewCellMouseEventArgs e)
        {
            DataGridViewRow row = this.Rows[e.RowIndex];
            if (row is CollDataGridViewRow)
            {
                if ((row as CollDataGridViewRow).IsCollapse == true)
                {
                    this.Expand(e.RowIndex);
                }
                else
                {
                    this.Collapse(e.RowIndex);
                }
            }
            base.OnRowHeaderMouseClick(e);
        }

        public void Expand(int nRowIndex)
        {
            DataGridViewRow row = this.Rows[nRowIndex];
            if (row is CollDataGridViewRow)
            {
                if ((row as CollDataGridViewRow).IsCollapse == true)
                {
                    (row as CollDataGridViewRow).IsCollapse = false;

                    if ((row as CollDataGridViewRow).Rows.Count != 0)
                    {
                        for (int i = 0; i < (row as CollDataGridViewRow).Rows.Count; i++)
                        {
                            this.Rows.Insert(nRowIndex + 1 + i, (row as CollDataGridViewRow).Rows[i]);
                            //展开子条目时重绘子条目背景色，防止与主条目背景色不一致
                            (row as CollDataGridViewRow).Rows[i].DefaultCellStyle.BackColor =
                                (row as CollDataGridViewRow).DefaultCellStyle.BackColor;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 折叠
        /// </summary>
        /// <param name="nRowIndex">行号</param>
        public void Collapse(int nRowIndex)
        {
            DataGridViewRow row = this.Rows[nRowIndex];
            if ((row as CollDataGridViewRow).IsCollapse == false)
            {
                if ((row as CollDataGridViewRow).Rows.Count != 0)
                {
                    this.RemoveAllSubRow((CollDataGridViewRow)row, false);
                }
                (row as CollDataGridViewRow).IsCollapse = true;
            }
        }

        /// <summary>
        /// 删除集合除首条外的所有子条目
        /// </summary>
        /// <param name="row">折叠行对象</param>
        /// <param name="flag"></param>
        private void RemoveAllSubRow(CollDataGridViewRow row, bool flag)
        {
            if (row.Rows.Count != 0)
            {
                if (!row.IsCollapse)
                {
                    for (int i = 0; i < row.Rows.Count; i++)
                    {
                        if (row.Rows[i] is CollDataGridViewRow)
                        {
                            RemoveAllSubRow((CollDataGridViewRow)row.Rows[i], true);
                        }
                        else
                        {
                            this.Rows.Remove(row.Rows[i]);
                        }
                    }
                }
                if (flag)
                {
                    row.IsCollapse = true;
                    this.Rows.Remove(row);
                }
            }
        }

        public void AddColumns(JObject obj)
        {
            int cloumIndex = 0;
            foreach (var item in obj)
            {
                var name = item.Key;
                AddTextBoxColumn(cloumIndex, name, name);
                cloumIndex++;
            }
        }

        public void AddData(JArray array, int ignoreIndex,int childNum)
        {
            foreach (var item in array)
            {
                var childList = new List<JObject>();
                for (int i = 0; i < childNum; i++)
                {
                    childList.Add((JObject)item);
                }

                CollDataGridViewRow collapseRow = new CollDataGridViewRow();
                collapseRow.IsCollapse = true;
                collapseRow.GroupTag = childList;

                DataGridViewCheckBoxCell cell = new DataGridViewCheckBoxCell();
                cell.Value = false;
                collapseRow.Cells.Add(cell);//不预先添加则无法折叠--相当于插入了空行

                for (int i = 1; i < childNum+1; i++)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    cell = new DataGridViewCheckBoxCell();
                    cell.Value = false;
                    row.Cells.Add(cell);//不预先添加则无法折叠
                    collapseRow.Rows.Add(row);
                }

                var rownum = this.Rows.Add(collapseRow);
                this.Expand(rownum);

                //Add Data
                for (int i = rownum; i < rownum + childNum+1; i++)
                {
                    if (i == rownum)
                    {
                        for (int k = 0; k < ignoreIndex; k++)
                        {
                            DataGridViewTextBoxCell cellTextBox = new DataGridViewTextBoxCell();
                            cellTextBox.Value = "";
                            this.Rows[i].Cells[k] = cellTextBox;
                        }
                    }
                    int j = ignoreIndex;
                    foreach (JProperty item2 in item)
                    {
                        this.Rows[i].Cells[j].Value = item2.Value;
                        j++;
                    }
                }
            }
        }

        private void AddTextBoxColumn(int index, string name, string headerText)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            column.Name = name;
            column.HeaderText = headerText;
            this.Columns.Insert(index, column);
        }
    }

    public class CollDataGridViewRow : DataGridViewRow
    {
        #region Private fields

        private CollDataGridViewRowCollection m_rowCollection = new CollDataGridViewRowCollection();

        #endregion

        #region Public Properties

        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsCollapse
        {
            get;
            set;
        }

        /// <summary>
        /// 折叠控件的集合
        /// </summary>
        public CollDataGridViewRowCollection Rows
        {
            get { return m_rowCollection; }
            set { m_rowCollection = value; }
        }

        /// <summary>
        /// 集合行的Tag，用于存储集合所有的信息
        /// </summary>
        public object GroupTag
        {
            get;
            set;
        }

        #endregion
    }

    public class CollDataGridViewRowCollection : IEnumerable<DataGridViewRow>, ICollection<DataGridViewRow>
    {
        private List<DataGridViewRow> m_list = new List<DataGridViewRow>();

        public DataGridViewRow this[int index]
        {
            get
            {
                if (index >= m_list.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return m_list[index];
            }
        }

        #region IEnumerable<DataGridViewRow> Member

        public IEnumerator<DataGridViewRow> GetEnumerator()
        {
            if (m_list.Count == 0)
            {
                throw new ArgumentOutOfRangeException("collection is null");
            }
            for (int i = 0; i < m_list.Count; i++)
            {
                yield return m_list[i];
            }
        }

        #endregion

        #region IEnumerable Member

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (m_list.Count == 0)
            {
                throw new ArgumentOutOfRangeException("collection is null");
            }
            for (int i = 0; i < m_list.Count; i++)
            {
                yield return m_list[i];
            }
        }

        #endregion

        #region ICollection<DataGridViewRow> Member

        public void Add(DataGridViewRow item)
        {
            m_list.Add(item);
        }

        public void Clear()
        {
            m_list.Clear();
        }

        public bool Contains(DataGridViewRow item)
        {
            return m_list.Contains(item);
        }

        public void CopyTo(DataGridViewRow[] array, int arrayIndex)
        {
            m_list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return m_list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(DataGridViewRow item)
        {
            return m_list.Remove(item);
        }

        #endregion
    }
}
