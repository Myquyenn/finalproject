using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Final_Project
{
    public partial class frmHistory : Form
    {
        History history;
        HistoryRepository repository;
        private Form1 _from1;
        public frmHistory(Form1 from1 )
        {
            _from1 = from1;
            InitializeComponent();
            history = new History();
            repository = new HistoryRepository();


        }

        private void frmHistory_Load(object sender, EventArgs e)
        {
            DataTable dataTable = new DataTable("History");
            dataTable.Columns.Add("Input");
            dataTable.Columns.Add("Output");
            dataTable.Columns["Output"].ReadOnly = true; // không cho chỉnh sửa cột Output
            dataTable.Columns.Add("Time Event");
            dataTable.Columns["Time Event"].ReadOnly = true; // không cho chỉnh sửa cột Time Event
            List<History> history = HistorySorted();
            
            history.Reverse(); // hiện date mới nhất lên trên
            foreach (History h in history)
            {
                dataTable.Rows.Add(h.Intput, h.Output, h.TimeEvent);
            }
            dataGridViewHistory.DataSource = dataTable;
            DataGridViewButtonColumn btnColumnReuse = new DataGridViewButtonColumn();
            btnColumnReuse.HeaderText = "Reuse";
            btnColumnReuse.Text = "Reuse";
            btnColumnReuse.UseColumnTextForButtonValue = true; // Hiển thị văn bản trên button

           

            dataGridViewHistory.Columns.Add(btnColumnReuse);
           
            // đăng kí sự kiện
            dataGridViewHistory.CellClick += dataGridViewHistory_CellClick;
        }

        public List<History> GetAllHistory()
        {
            return repository.GetAllHistory();
        }

        //Ứng dụng hàm Exchange sort
        public List<History> HistorySorted()
        {
            List<History> history = repository.GetAllHistory();
            for (int i = 0; i < history.Count-1; i++)
            {
                for (int j = i + 1; j < history.Count; j++)
                {
                    if (history[i].TimeEvent > history[j].TimeEvent)
                    {
                        History temp = history[i];
                        history[i] = history[j];
                        history[j] = temp;
                    }
                }
            }
            return history;
        }
        private void dataGridViewHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            else if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 3)
                {
                    _from1.UpdateInput(dataGridViewHistory.Rows[e.RowIndex].Cells[0].Value.ToString());
                    this.Close();
                }
            }
        }

    }
}
