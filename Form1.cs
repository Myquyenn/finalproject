using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestConsoleCode;

namespace Final_Project
{
    public partial class Form1 : Form
    {
        public string input = "";
        public double result = 0;
        private BinTree binTree = new BinTree();
        frmHistory frmHistory;
        HistoryRepository repository;

        public Form1()
        {
            InitializeComponent();
            repository = new HistoryRepository();
            frmHistory = new frmHistory(this);
            KeyPreview = true;
        }

        private void btn0_Click(object sender, EventArgs e) => txtNhap.Text += "0";
        private void btn1_Click(object sender, EventArgs e) => txtNhap.Text += "1";
        private void btn2_Click(object sender, EventArgs e) => txtNhap.Text += "2";
        private void btn3_Click(object sender, EventArgs e) => txtNhap.Text += "3";
        private void btn4_Click(object sender, EventArgs e) => txtNhap.Text += "4";
        private void btn5_Click(object sender, EventArgs e) => txtNhap.Text += "5";
        private void btn6_Click(object sender, EventArgs e) => txtNhap.Text += "6";
        private void btn7_Click(object sender, EventArgs e) => txtNhap.Text += "7";
        private void btn8_Click(object sender, EventArgs e) => txtNhap.Text += "8";
        private void btn9_Click(object sender, EventArgs e) => txtNhap.Text += "9";

        private void btnAdd_Click(object sender, EventArgs e) => txtNhap.Text += " + ";
        private void btnMinus_Click(object sender, EventArgs e) => txtNhap.Text += " - ";
        private void btnMul_Click(object sender, EventArgs e) => txtNhap.Text += " * ";
        private void btnDiv_Click(object sender, EventArgs e) => txtNhap.Text += " / ";
        private void btnOpen_Click(object sender, EventArgs e) => txtNhap.Text += " ( ";
        private void txtClose_Click(object sender, EventArgs e) => txtNhap.Text += " ) ";
        private void btndot_Click(object sender, EventArgs e) => txtNhap.Text += ".";

        private void button3_Click(object sender, EventArgs e)
        {
            txtNhap.Text = "";
            txtXuat.Text = "";
        }

        private void btnPercent_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtXuat.Text, out double value))
            {
                txtXuat.Text = (value / 100).ToString();
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (txtNhap.Text.Length > 0)
            {
                txtNhap.Text = txtNhap.Text.Remove(txtNhap.Text.Length - 1);
                txtXuat.Text = "";
            }
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            if (frmHistory == null || frmHistory.IsDisposed)
                frmHistory = new frmHistory(this);
            frmHistory.Show();
        }

        public void UpdateInput(string input) => txtNhap.Text = input;

        private void txtNhap_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || "+-*/().".Contains(e.KeyChar))
                e.Handled = false;
            else if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Escape)
                e.Handled = true;
        }

        private void btnAns_Click(object sender, EventArgs e)
        {
            var histories = frmHistory.GetAllHistory();
            if (histories.Any())
                txtNhap.Text = histories.Last().Output;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnEqual.PerformClick();
            else if (e.KeyCode == Keys.Back)
                btnDel.PerformClick();
            else if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        // ✅ Nút "=" tính toán trên luồng phụ (tối ưu tránh lag)
        private async void btnEqual_Click(object sender, EventArgs e)
        {
            input = txtNhap.Text.Trim();
            txtXuat.Text = "Đang tính...";

            try
            {
                // ✅ Tính toán trên thread phụ
                result = await Task.Run(() => binTree.Calculate(input));
                txtXuat.Text = result.ToString();

                // ✅ Lưu lịch sử trên thread phụ
                History history = new History(input, result.ToString());
                await Task.Run(() => repository.SaveToExcel(history));
            }
            catch (Exception ex)
            {
                txtXuat.Text = "Lỗi: " + ex.Message;
            }
        }
    }
}
