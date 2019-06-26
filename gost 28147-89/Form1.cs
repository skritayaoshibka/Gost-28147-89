using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gost_28147_89
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void encrypt_button_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                string decrypted_text = textBox1.Text;
                string key = textBox2.Text;

                Gost gost = new Gost(decrypted_text, key);
                textBox3.Text = gost.Encrypt();
            }
            else
                MessageBox.Show("Введите текст и ключ", "Не введены данные");
          
        }

        private void decrypt_button_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "" && textBox3.Text != "")
            {
                string encrypted_text = textBox3.Text;
                string key = textBox2.Text;

                Gost gost = new Gost(encrypted_text, key);
                textBox1.Text = gost.Decrypt();
            }
            else
                MessageBox.Show("Введите текст и ключ", "Не введены данные");
        }
    }
}
