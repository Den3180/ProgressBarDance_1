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
        Safemethod safemethod;       
        public Form1()
        {
            InitializeComponent();
            safemethod = new Safemethod(Foo);
            verticals = new List<VerticalProgressBar>();           
            sizeBar = new Size(60, groupBox1.Height - 20);
            comboBox1.Text = comboBox1.Items[0].ToString();           
            this.StartPosition = FormStartPosition.CenterScreen;
            Clear.Enabled = false;
        }
        public void MyPerformStep(object obj)
        {
            Random r = new Random();
            if((int)obj<verticals.Count && verticals[(int)obj].InvokeRequired)
            verticals[(int)obj].Invoke(safemethod, r.Next(0, countThread));          
           
        }      
        public void Foo(int numberBar)
        {            
            if(countThread!=0)
            {
                verticals[numberBar].PerformStep();
            }           
        }
        public void LetsDance(object state)
        {
            TimerCallback callback = new TimerCallback(MyPerformStep);
            System.Threading.Timer timer=new System.Threading.Timer(callback,state,0,100);
            if(verticals[(int)state].Value==verticals[(int)state].Maximum)
            {
                timer.Dispose();
            }
        }
        //Кнопка старт.
        private void button1_Click(object sender, EventArgs e)
        {           
            if (Clear.Enabled == false)
            {
                Clear.Enabled = true;
            }
            comboBox1.Enabled = false;
            button1.Enabled = false;
            //Число потоков по числу баров.
            countThread = comboBox1.SelectedIndex+1;           
            //Постановка потоков в очередь пула потоков.
            for (int i=0;i<countThread;i++)
            {               
                ThreadPool.QueueUserWorkItem(LetsDance, i);
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

        private void Clear_Click(object sender, EventArgs e)
        {           
            Clear.Enabled = false;
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
