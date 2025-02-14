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
    public partial class FormOrder : Form
    {
        //public Order Order = new Order();
        public Tovar Tovar = new Tovar();
        public List<Order> Orders = new List<Order>();
        public String Client;
        List<Tovar> _tovars = new List<Tovar>();

        List<String> _statuses = new List<string>()
        {
            "Собирается","Отправлен","Ожидает получения", "Получен"
        };


        public FormOrder(List<Tovar>tovars)
        {
            InitializeComponent();
            
            foreach(var status in _statuses)
            {
                comboBox2.Items.Add(status);
            }

            _tovars = tovars;
            textBox1.Text = DateTime.Now.ToShortDateString();
            comboBox1.SelectedIndex = -1;
            foreach (var tovar in tovars)
            {
                comboBox1.Items.Add(tovar.Title);
            }

            comboBox2.SelectedIndex = 0;
        }

        public FormOrder(List<Tovar>tovars, List<Order> orders)
        {
            InitializeComponent();

            foreach (var status in _statuses)
            {
                comboBox2.Items.Add(status);
            }

            _tovars = tovars;
            textBox1.Text = DateTime.Now.ToShortDateString();

            foreach (var tovar in tovars)
            {
                comboBox1.Items.Add(tovar.Title);
            }

            foreach (var order in orders)
            {
                var tovar = _tovars.First(x => x.ID == order.TovarIds);
                listBox1.Items.Add(tovar.Title);
            }

            textBox2.Text = orders.First().Client;
            comboBox2.SelectedItem = orders.First().Status;
        }



        private void button3_Click(object sender, EventArgs e)
        {
            var code = Guid.NewGuid().ToString().Split('-')[0];
            Client = textBox2.Text;
            foreach(var item in listBox1.Items)
            {
                Order order = new Order();
                order.Date = textBox1.Text;
                order.Code = code;
                order.Status = comboBox2.Text;
                order.Client = textBox2.Text;
                order.TovarIds = _tovars.First(x => x.Title == item.ToString()).ID;
                order.Price = _tovars.First(x => x.Title == item.ToString()).Price;
                Orders.Add(order);
            }
            this.DialogResult = DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex!=-1)
            {
                listBox1.Items.Add(comboBox1.Text);
                //Order.Tovars.Add(_tovars.First(x => x.Title == comboBox1.Text));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedItem!=null)
            {
                //Order.Tovars.Remove(_tovars.First(x => x.Title == listBox1.SelectedItem.ToString()));
                //Tovar.Orders.tov.Remove(_tovars.First(x => x.Title == listBox1.SelectedItem.ToString()));
                listBox1.Items.Remove(listBox1.SelectedItem);
            }
        }
    }
}
