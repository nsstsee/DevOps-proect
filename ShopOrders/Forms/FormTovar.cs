using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShopOrders.Forms
{
    public partial class FormTovar : Form
    {
        public FormTovar()
        {
            InitializeComponent();
        }

        public FormTovar(Tovar tovar)
        {
            InitializeComponent();
            textBox1.Text = tovar.Title;
            textBox2.Text = tovar.Price.ToString();
        }

        public Tovar Tovar = new Tovar();

        private void button1_Click(object sender, EventArgs e)
        {
            if(double.TryParse(textBox2.Text, out double price))
            {
                Tovar.Title = textBox1.Text;
                Tovar.Price = price;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Цена указана неверно");
            }
        }
    }
}
