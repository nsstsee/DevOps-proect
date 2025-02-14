using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShopOrders
{
    public partial class Auth : Form
    {
        public Auth()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var context = new TovarList())
            {
                var user = context.Users.FirstOrDefault(x => x.Login == textBox1.Text && x.Password == textBox2.Text);
                if(user != null)
                {
                    Form1 form1 = new Form1(user);
                    form1.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Пользователь не найден");
                }
            }
        }
    }
}
