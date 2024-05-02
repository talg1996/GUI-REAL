using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System;
using System.Windows;
using System.Windows.Threading;


namespace GUI_REAL
{
    public partial class Window1 : Window
    {
        private DispatcherTimer timer;
        private int dotCount;

        public Window1()
        {
            InitializeComponent();

            // Initialize the timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            // Start the timer
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update the loading text with increasing dots
            string loadingText = "Loading";
            dotCount = (dotCount + 1) % 4; // Loop through 4 dots
            for (int i = 0; i < dotCount; i++)
            {
                loadingText += ".";
            }

            textBlock.Text = loadingText;
        }
    }
}