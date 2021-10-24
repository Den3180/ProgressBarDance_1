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
using System.Runtime.InteropServices;

namespace ProgressBarDance_1
{
    public partial class Form1 : Form
    {
        //Делегат для функции безопасного обращения к ProgressBar из других потоков.
        private delegate void Safemethod(int numberBar);
        //Список баров.
        List<VerticalProgressBar> verticals;
        //Размер баров.
        Size sizeBar;
        //Колличество баров.
        int countThread;         
        Safemethod safemethod;       
        public Form1()
        {
            InitializeComponent();
            safemethod = new Safemethod(ProgressBarAction);
            verticals = new List<VerticalProgressBar>();           
            sizeBar = new Size(60, groupBox1.Height - 20);
            comboBox1.Text = comboBox1.Items[0].ToString();           
            this.StartPosition = FormStartPosition.CenterScreen;
            ClearButton.Enabled = false;
        }
        
        //Функция обратного вызова дл таймера.
        public void MyPerformStep(object obj)
        {
            Random r = new Random();
            int count = (int)obj;
            if(count<verticals.Count && verticals[count].InvokeRequired)
            verticals[count].Invoke(safemethod, r.Next(0, countThread));          
           
        }  
        //Приращение баров.
        public void ProgressBarAction(int numberBar)
        {            
            if(countThread!=0)
            {
                verticals[numberBar].PerformStep();
            }           
        }
        //Функция для передачи в очередь пула потоков.
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
            if (ClearButton.Enabled == false)
            {
                ClearButton.Enabled = true;
            }
            comboBox1.Enabled = false;
            StartButton.Enabled = false;
            //Число потоков по числу баров.
            countThread = comboBox1.SelectedIndex+1;           
            //Постановка потоков в очередь пула потоков.
            for (int i=0;i<countThread;i++)
            {               
                ThreadPool.QueueUserWorkItem(LetsDance, i);
            }            
        }
        //Запрет клавиатурного ввода в Combobox.
        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        //Перерисовка поля баров в зависимости от выбранного количества.
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (groupBox1.Controls.Count > 0) 
            { 
                groupBox1.Controls.Clear(); 
            }
            PlacementBars(comboBox1.Text);
        }
        //Функция перерисовки баров.
        public void PlacementBars(string number)
        {          
            int num = Int32.Parse(number);
            Random randomColor = new Random();
            Point positionBar = new Point(0, 20)
            {
                X = (groupBox1.Width / 2 - (sizeBar.Width / 2)) - sizeBar.Width * comboBox1.SelectedIndex
            };
            for (int i=0; i<num; i++)
            {
                verticals.Add(new VerticalProgressBar { Size=sizeBar, Step=1, Location=positionBar});
                verticals[i].Location = positionBar;
                ProgressBarColor.SetState(verticals[i], randomColor.Next(1,4));
                groupBox1.Controls.Add(verticals[i]);
                positionBar.X += 2 * sizeBar.Width;
            }
        }
        //Кнопка сброс.
        private void Clear_Click(object sender, EventArgs e)
        {           
            ClearButton.Enabled = false;
            comboBox1.Enabled = true;
            StartButton.Enabled = true;
            verticals.Clear();
            countThread = 0;
            comboBox1_SelectedIndexChanged(sender, e);
        }
      
    }
    //Вертикальный ProgressBar.
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
    //Экспорт функции для смены настроек цветов ProgressBar.
    public static class ProgressBarColor
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void SetState(this ProgressBar p, int state)
        {
            SendMessage(p.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }
}
