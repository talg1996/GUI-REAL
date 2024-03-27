using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI_REAL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void User_mode_button_click(object sender, RoutedEventArgs e)
        {
            aaa.Visibility= Visibility.Visible;
            Check_commands_lable.Visibility = Visibility.Hidden;
            MCU_Programing.Visibility = Visibility.Hidden;
            Realys_board_command_generate.Visibility = Visibility.Hidden;
            UART_Communication.Visibility = Visibility.Hidden;
            Add_test_equipment_and_commands.Visibility = Visibility.Hidden; 
            Path.Visibility = Visibility.Hidden;
        }

        private void Technician_mode_button_Click(object sender, RoutedEventArgs e)
        {
            aaa.Visibility = Visibility.Visible;
            Path.Visibility = Visibility.Visible;
            Check_commands_lable.Visibility= Visibility.Visible;
            MCU_Programing.Visibility= Visibility.Visible ;
            Realys_board_command_generate.Visibility= Visibility.Visible ;
            UART_Communication.Visibility= Visibility.Visible ;
            Add_test_equipment_and_commands.Visibility = Visibility.Visible;
        }

        
    }
}