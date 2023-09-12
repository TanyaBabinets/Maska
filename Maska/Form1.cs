using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
//Разработать WinForms-приложение «Поиск файлов и папок», выполняющее поиск по заданной маске. 
//    В маске можно использовать «*» (любые символы в любом количестве), а также «?» (один любой символ). 
//    На форме предусмотреть текстовое поле (TextBox) для ввода маски, комбинированный список (ComboBox) 
//    для выбора диска, на котором производить поиск, а также список просмотра (ListView) 
//    для отображения найденных файлов и папок. 
//    Поиск файлов и папок следует выполнить в отдельном вторичном потоке.
namespace Maska
{
    public partial class Form1 : Form
    {
        public static SynchronizationContext uiContext;
        Thread t = null;
        
        public Form1()
        {
            InitializeComponent();
            uiContext = SynchronizationContext.Current;

            comboBox1.Items.AddRange(Directory.GetLogicalDrives());
            
            listView1.View = View.Details;
        }

        public class Mask
        {
            public string filepath;
            public string mask;
            public static int count;


            public Mask(string f, string m)
            {
                filepath = f;
                mask = m;
            }
            public Mask() { }


            
            public static List<FileInfo> MyFiles(string path, string extention)
            {
                List<FileInfo> files = new List<FileInfo>();
                try
                {
                    foreach (var file in Directory.GetFiles(path, extention))
                    {
                        files.Add(new FileInfo(file));
                        count++;
                    }
                    string[] directories = Directory.GetDirectories(path);
                    if (directories.Length > 0)
                    {
                        foreach (string directory in directories)
                        {
                            files.AddRange(MyFiles(directory, extention));
                        }
                    }
                    
                }
                catch { }
                return files;
            }
        }
        public void ShowInfo(object obj)
        {
            uiContext.Send(x => listView1.Items.Clear(), null);
            Mask maska = (Mask)obj;
            string path = maska.filepath;
            string extention = maska.mask;
            int qty = 1;

            List<FileInfo> list = Mask.MyFiles(path, extention);
            foreach (var item in list)
            {
                ListViewItem spisok = new ListViewItem(qty.ToString());
                spisok.SubItems.Add(item.Name);
                spisok.SubItems.Add(item.FullName);
                spisok.SubItems.Add(item.Length.ToString());
                spisok.SubItems.Add(item.LastAccessTime.ToString());
                uiContext.Send(x => listView1.Items.Add(spisok), null);
                uiContext.Send(x => label2.Text = Mask.count.ToString(), null);//считает кол-во файлов
                qty++;
            }
        }
      
        private void textBox1_TextChanged(object sender, EventArgs e)
        { }

        private void button1_Click(object sender, EventArgs e)
        {
            string a = textBox1.Text;
            string b = comboBox1.SelectedItem as string;
            
            Mask maska = new Mask(b, a);
            t = new Thread(new ParameterizedThreadStart(ShowInfo));
            t.IsBackground = true;
            t.Start(maska);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)//// STOP
        {
            t.Abort();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
            
        }
    }
}

