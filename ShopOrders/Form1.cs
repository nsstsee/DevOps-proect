using OfficeOpenXml;
using ShopOrders.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShopOrders
{
    public partial class Form1 : Form
    {
        public TovarList TovarList;

        User _user = new User();

        //Разделение ролей
        public Form1(User user)
        {
            InitializeComponent();
            TovarList = new TovarList();

            _user = user;

            foreach(var tovar in TovarList.Tovars)
            {
                dataGridView1.Rows.Add(tovar.ID, tovar.Title, tovar.Price);
            }
            foreach (var order in TovarList.Orders.ToList().GroupBy(x=>x.RecordID).ToList())
            {
                dataGridView2.Rows.Add(order.First().RecordID, order.First().Code, order.First().Status, order.First().Date, order.Sum(x=>x.Price), order.First().Client);
            }

            if (!user.IsAdmin)
            {
                foreach (TabPage page in this.Controls[0].Controls)
                {
                    if(page.Text!="Товары" && page.Text!="Заказы")
                    {
                        page.Text = $"*{page.Text}";
                    }
                }
                foreach (var btn in this.Controls[0].Controls[0].Controls)
                {
                    if(btn is Button)
                    {
                        (btn as Button).Enabled = false;   
                    }
                }

                foreach (var btn in this.Controls[0].Controls[1].Controls)
                {
                    if(btn is Button)
                    {
                        if ((btn as Button).Text == "Применить" || (btn as Button).Text == "Сбросить" || (btn as Button).Text == "Сформировать чек") continue;
                        (btn as Button).Enabled = false;
                    }
                }   
            }
            foreach (var usr in TovarList.Users)
            {
                comboBox1.Items.Add(usr.Login);
            }
            comboBox2.Items.AddRange(new String[] { "Собирается", "Отправлен", "Ожидает получения", "Получен" });
        }

        //Добавление товара
        private void button1_Click(object sender, EventArgs e)
        {
            FormTovar formTovar = new FormTovar();
            if(formTovar.ShowDialog()==DialogResult.OK)
            {
                var tovar = formTovar.Tovar;
                TovarList.Tovars.Add(tovar);
                TovarList.SaveChanges();
                dataGridView1.Rows.Add(tovar.ID, tovar.Title, tovar.Price);
            }
        }
        //Редактирование товара
        private void button2_Click(object sender, EventArgs e)
        {
            if(dataGridView1.SelectedRows.Count>0)
            {
                var index = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value);
                var tovar = TovarList.Tovars.First(x => x.ID == index);
                FormTovar formTovar = new FormTovar(tovar);
                if (formTovar.ShowDialog() == DialogResult.OK)
                {
                    tovar.Title = formTovar.Tovar.Title;
                    tovar.Price = formTovar.Tovar.Price;

                    dataGridView1.SelectedRows[0].Cells[1].Value = tovar.Title;
                    dataGridView1.SelectedRows[0].Cells[2].Value = tovar.Price;

                    TovarList.Entry(tovar).State = System.Data.Entity.EntityState.Modified;
                    TovarList.SaveChanges();
                }
            }
            
        }
        //Удаление товара
        private void button3_Click(object sender, EventArgs e)
        {
            if(dataGridView1.SelectedRows.Count>0)
            {
                var index = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value);
                var tovar = TovarList.Tovars.First(x => x.ID == index);
                TovarList.Entry(tovar).State = System.Data.Entity.EntityState.Deleted;
                TovarList.Tovars.Remove(tovar);
                TovarList.SaveChanges();
                dataGridView1.Rows.Remove(dataGridView1.SelectedRows[0]);
            }
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if(!_user.IsAdmin)
            {
                e.Cancel = e.TabPageIndex == 2;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        //Добавление заказа
        private void button4_Click(object sender, EventArgs e)
        {
            FormOrder formOrder = new FormOrder(TovarList.Tovars.ToList());
            if(formOrder.ShowDialog() == DialogResult.OK)
            {
                var orders = formOrder.Orders;
                var recordid = 1;
                double price = 0;
                if(TovarList.Orders.Count()>0)
                {
                    var last = TovarList.Orders.ToList().Max(x => x.RecordID);
                    recordid = recordid + (last++);
                }

                
                foreach(var order in orders)
                {
                    order.Client = formOrder.Client;
                    order.RecordID = recordid;
                    TovarList.Orders.Add(order);
                    TovarList.SaveChanges();
                    price+=order.Price;
                }
                var recordToDG = TovarList.Orders.Where(x => x.RecordID == recordid).GroupBy(x => x.RecordID).ToList();
                dataGridView2.Rows.Add(recordid, recordToDG[0].First().Code, recordToDG[0].First().Status, recordToDG[0].First().Date, price, recordToDG[0].First().Client);

                MessageBox.Show("Заказ добавлен. Код заказа - " + recordToDG[0].First().Code, "Добавлено");
            }
        }
        //Удаление заказа
        private void button6_Click(object sender, EventArgs e)
        {
            if(dataGridView2.SelectedRows.Count>0)
            {
                var index = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells[0].Value);
                var orders = TovarList.Orders.Where(x => x.RecordID == index).ToList();

                foreach (var order in orders)
                {
                    TovarList.Entry(order).State = System.Data.Entity.EntityState.Deleted;
                    TovarList.Orders.Remove(order);
                    TovarList.SaveChanges();
                }
                dataGridView3.Visible = false;
                dataGridView2.Rows.Remove(dataGridView2.SelectedRows[0]);
            }
        }

        private void dataGridView2_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(e.RowIndex != -1)
            {
                var index = Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells[0].Value);
                if (index == 0) return;
                dataGridView3.Visible = true;
                dataGridView3.Rows.Clear();
                var orders = TovarList.Orders.Where(x => x.RecordID == index).ToList();

                var groups = orders.GroupBy(x => x.TovarIds).ToList();

                double sumGroup = 0;
                foreach (var group in groups)
                {
                    var tovar = TovarList.Tovars.First(x => x.ID == group.Key);
                    sumGroup = tovar.Price * group.Count();
                    dataGridView3.Rows.Add(group.First().ID,
                                            tovar.Title, 
                                            group.Count(), 
                                            sumGroup);
                }
            }
        }
        //Редактирование заказа
        private void button5_Click(object sender, EventArgs e)
        {
            if(dataGridView2.SelectedRows.Count>0)
            {
                var index = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells[0].Value);
                var orders = TovarList.Orders.Where(x => x.RecordID == index).ToList();
                FormOrder formOrder = new FormOrder(TovarList.Tovars.ToList(),orders);
                if (formOrder.ShowDialog() == DialogResult.OK)
                {
                    var ordersEdit = formOrder.Orders;
                    int recordid = orders.First().RecordID;

                    foreach(var order in orders)
                    {
                        TovarList.Entry(order).State = System.Data.Entity.EntityState.Deleted;
                        TovarList.Orders.Remove(order);
                    }

                    double price = 0;
                    foreach (var order in ordersEdit)
                    {
                        order.RecordID = recordid;
                        order.Client = formOrder.Client;
                        price+=order.Price;
                        TovarList.Orders.Add(order);
                    }

                    TovarList.SaveChanges();

                    var recordToDG = TovarList.Orders.Where(x => x.RecordID == recordid).GroupBy(x => x.RecordID).ToList();
                    dataGridView2.Rows.Remove(dataGridView2.SelectedRows[0]);
                    dataGridView2.Rows.Add(recordid, recordToDG[0].First().Code, recordToDG[0].First().Status, recordToDG[0].First().Date, price, recordToDG[0].First().Client);
                    dataGridView3.Visible = false;
                }
            }
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            bool success = false;
            if(!string.IsNullOrEmpty(textBox1.Text))
            {
                if(!string.IsNullOrEmpty(textBox2.Text))
                {
                    User user = new User();
                    user.Login = textBox1.Text;
                    user.Password = textBox2.Text;
                    user.IsAdmin = radioButton1.Checked ? true : false;

                    TovarList.Users.Add(user);
                    TovarList.SaveChanges();
                    comboBox1.Items.Add(user.Login);
                    success = true;
                }
            }
            if(!success)
            {
                MessageBox.Show("Проверьте данные","Ошибка");
            }
            else
            {
                MessageBox.Show("Пользователь добавлен", "Успешно");
            }
        }
        //Применение фильтра
        private void button8_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(textBox3.Text))
            {
                dataGridView2.Rows.Clear();
                var ordersFilter = TovarList.Orders.Where(x => x.Code == textBox3.Text || x.Client == textBox3.Text).ToList().GroupBy(x=>x.RecordID).ToList();
                foreach(var order in ordersFilter)
                {
                    dataGridView2.Rows.Add(order.First().RecordID, order.First().Code, order.First().Status, order.First().Date, order.Sum(x => x.Price), order.First().Client);
                }
            }
            else
            {
                MessageBox.Show("Фильтр пуст");
            }
        }
        //Сброс фильтра
        private void button9_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();
            foreach (var order in TovarList.Orders.ToList().GroupBy(x => x.RecordID).ToList())
            {
                dataGridView2.Rows.Add(order.First().RecordID, order.First().Code, order.First().Status, order.First().Date, order.Sum(x => x.Price), order.First().Client);
            }
            textBox3.Text = "";
        }
        //Формирование чека
        private void button10_Click(object sender, EventArgs e)
        {
            if(dataGridView2.SelectedRows.Count>0)
            {
                var index = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells[0].Value);
                var orders = TovarList.Orders.Where(x => x.RecordID == index).ToList();

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using(var excel = new ExcelPackage($"Чек_{orders.First().Client}.xlsx"))
                {
                    var sheet = excel.Workbook.Worksheets.Add("Лист1");
                    sheet.Cells[1, 1].Value = "ФИО клиента";
                    sheet.Cells[1, 2].Value = orders.First().Client;

                    sheet.Cells[2, 1].Value = "Дата заказа";
                    sheet.Cells[2, 2].Value = orders.First().Date;

                    sheet.Cells[3, 1].Value = "Товары";
                    int row = 3;
                    double sum = 0;
                    foreach(var order in orders)
                    {
                        var tovar = TovarList.Tovars.First(x => x.ID == order.TovarIds);
                        sheet.Cells[row, 2].Value = tovar.Title;
                        sheet.Cells[row, 3].Value = $"{tovar.Price} руб.";
                        sum += tovar.Price;
                        row++;
                    }
                    sheet.Cells[row, 1].Value = "Общая сумма";
                    sheet.Cells[row, 3].Value = $"{sum} руб.";


                    sheet.Cells[1, 1, row, 3].AutoFitColumns();
                    sheet.Cells[1, 1, row, 1].Style.Font.Bold = true;
                    excel.Save();
                    MessageBox.Show("Чек сформирован","Успешно");
                }
            }
            else
            {
                MessageBox.Show("Не выбран заказ", "Ошибка");
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            var usr = TovarList.Users.FirstOrDefault(x => x.Login == comboBox1.Text);
            if(usr!=null)
            {
                comboBox1.Items.Remove(usr.Login);
                TovarList.Users.Remove(usr);
                TovarList.SaveChanges();
            }
        }
        //Экспорт заказов
        private void button12_Click(object sender, EventArgs e)
        {
            if(comboBox2.Text!="")
            {
                var tovarFilter = TovarList.Orders.Where(x => x.Status == comboBox2.Text).GroupBy(x=>x.RecordID).ToList();
                if(tovarFilter!=null)
                {
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    using(var excel = new ExcelPackage($"ExportByStatus_{comboBox2.Text}.xlsx"))
                    {
                        var sheet = excel.Workbook.Worksheets.Add("Лист1");
                        sheet.Cells[1, 1].Value = "Код заказа";
                        sheet.Cells[1, 2].Value = "Статус";
                        sheet.Cells[1, 3].Value = "Дата";
                        sheet.Cells[1, 4].Value = "Стоимость";
                        sheet.Cells[1, 5].Value = "ФИО клиента";

                        int row = 2;
                        foreach(var order in tovarFilter)
                        {
                            sheet.Cells[row, 1].Value = order.First().Code;
                            sheet.Cells[row, 2].Value = order.First().Status;
                            sheet.Cells[row, 3].Value = order.First().Date;
                            sheet.Cells[row, 4].Value = order.Sum(x=>x.Price);
                            sheet.Cells[row, 5].Value = order.First().Client;
                            row++;
                        }
                        sheet.Cells[1, 1, row, 5].AutoFitColumns();
                        sheet.Cells[1, 1, 1, 5].Style.Font.Bold = true;

                        excel.Save();
                        MessageBox.Show("Экспорт выполнен","Успешно");
                    }
                }
            }
        }
    }
}
