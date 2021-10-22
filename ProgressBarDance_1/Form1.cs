using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace ProgressBarDance_1
{
    public partial class Form1 : Form
    {
        private delegate void Safemethod(int numberBar);
        private delegate void Safeme(object numb);
        List<VerticalProgressBar> verticals;
        Size sizeBar;
        int countThread;
        ThreadStart [] threadStarts;
        Thread[] threads;
        Safemethod safemethod;
        Safeme temp;
        //TimerCallback callback;
        //System.Threading.Timer timer;
        
        public Form1()
        {
            InitializeComponent();
            safemethod = new Safemethod(Foo);
           temp = new Safeme(foo2);
            verticals = new List<VerticalProgressBar>();
            //callback = new TimerCallback(MyPerformStep);
            //timer= new System.Threading.Timer(callback);
            sizeBar = new Size(60, groupBox1.Height - 20);
            comboBox1.Text = comboBox1.Items[0].ToString();           
            this.StartPosition = FormStartPosition.CenterScreen;
            button2.Enabled = false;
        }
        public void MyPerformStep(object obj)
        {
            System.Threading.Timer timer1=obj as System.Threading.Timer ;
            Random r = new Random();
               verticals[r.Next(0,countThread)].Invoke(safemethod, r.Next(0, countThread));
            // listBox1.Invoke(temp, (object)Thread.CurrentThread.ManagedThreadId);
            
            //if (verticals[r.Next(0, countThread)].Value == verticals[r.Next(0, countThread)].Maximum)
            //     timer1.Dispose();
        }
        public void foo2(object t)
        {
            listBox1.Items.Add($"Поток: {t.ToString()}");
        }
        public void Foo(int numberBar)
        {            
            if(countThread!=0)
            {
                int t = Thread.CurrentThread.ManagedThreadId;
                verticals[numberBar].PerformStep();
                listBox1.Items.Add($"Поток: {t.ToString()}");
            }           
        }
        public void LetsDance()
        {
            TimerCallback callback = new TimerCallback(MyPerformStep);
            System.Threading.Timer timer=new System.Threading.Timer(callback);
            timer.Change(0, 100);
            
        }

        public void LetsDance1(object a)
        {
            TimerCallback callback = new TimerCallback(MyPerformStep);
            System.Threading.Timer timer = new System.Threading.Timer(callback);
            timer.Change(0, 100);

        }

        //Кнопка старт.
        private void button1_Click(object sender, EventArgs e)
        {           
            if (button2.Enabled == false)
            {
                button2.Enabled = true;
            }
            comboBox1.Enabled = false;
            button1.Enabled = false;
            //Число потоков по числу баров.
            countThread = comboBox1.SelectedIndex+1;
            //Массив потоков.
            threads = new Thread[countThread];
            //Массив делегатов.
            threadStarts = new ThreadStart[countThread];
            //Инициализация потоков и их запуск.
            for (int i=0;i<countThread;i++)
            {
                //threadStarts[i] = new ThreadStart(LetsDance);
                //threads[i] = new Thread(threadStarts[i]);
                //threads[i].Start();
                //listBox1.Invoke(temp, threads[i].ManagedThreadId);
                ThreadPool.QueueUserWorkItem(LetsDance1,null);
            }            
        }
        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (groupBox1.Controls.Count > 0) 
            { 
                groupBox1.Controls.Clear(); 
            }
            PlacementBars(comboBox1.Text);
        }
        public void PlacementBars(string number)
        {
            if(verticals.Count>0)
            {
                verticals.Clear();
            }
            int num = Int32.Parse(number);
            Point positionBar = new Point(0, 20)
            {
                X = (groupBox1.Width / 2 - (sizeBar.Width / 2)) - sizeBar.Width * comboBox1.SelectedIndex
            };
            for (int i=0; i<num; i++)
            {
                verticals.Add(new VerticalProgressBar());
                verticals[i].Size = sizeBar;
                verticals[i].Location = positionBar;
                verticals[i].Step = 1;
                groupBox1.Controls.Add(verticals[i]);
                positionBar.X += 2 * sizeBar.Width;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < countThread; i++)
            {
                threads[i].Abort();
                
            }
            //timer.Dispose();
            button2.Enabled = false;
            comboBox1.Enabled = true;
            button1.Enabled = true;
            verticals.Clear();
            countThread = 0;
            comboBox1_SelectedIndexChanged(sender, e);
        }
    }
    public class VerticalProgressBar : ProgressBar
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x04;
                return cp;
            }
        }       
    }
}
