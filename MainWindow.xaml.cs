using System.Collections.Generic;
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
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;




namespace GUI_REAL
{



    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window, INotifyPropertyChanged
    {
        Instrument temp_instrument = new Instrument();
        List<Instrument> Instruments_Names_List = new List<Instrument>();

        string[] Programing_hardware = new string[] { "ST_Link", "JLINK" };
        string[] UUT_amount = new string[] { "1", "2", "3", "4" };
        

        string User_mode;

        public event PropertyChangedEventHandler? PropertyChanged;










        public MainWindow()
        {





            // Lists to store names and command strings from Excel files

            string Instruments_path_file = "H:\\Project\\Instrunets.xlsx";
            // Update_Instrument_List(Instruments_path_file, Instruments_Names_List);

            InitializeComponent();
            Init();
        }

        private void Init()
        {
            Init_Programing();
        }

        private void Init_Programing()
        {


            Programing_choose_hardware.ItemsSource = Programing_hardware;
            how_many_uut_combobox.ItemsSource = UUT_amount;

        }

        private void Update_Instrument_List(string Instruments_path_file, List<Instrument> Instruments_Names_List)
        {

            Excel_Row_read_by_index(Instruments_path_file);
        }


        public List<string> Excel_Row_read_by_index(string path)
        {
            string[] temp = new string[10];
            string filePath = path;
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            Workbook wb;
            Worksheet ws;

            wb = excel.Workbooks.Open(filePath);
            ws = wb.Worksheets[1];

            // Find the last used row
            int lastRow = ws.Cells[ws.Rows.Count, 1].End[XlDirection.xlUp].Row;

            // Find the last used column
            int lastColumn = ws.Cells[1, ws.Columns.Count].End[XlDirection.xlToLeft].Column;

            for (int row = 2; row <= lastRow; row++)
            {
                for (int column = 1; column < lastColumn; column++)
                {
                    temp[column - 1] = ws.Cells[row, column].Value?.ToString();
                }
                try
                {
                    temp_instrument.Name = temp[1];
                    temp_instrument.Com = temp[2];
                    temp_instrument.Lan = temp[3];
                    temp_instrument.Visa_Usb = temp[4];
                    temp_instrument.Visa_Lan = temp[5];
                    Instruments_Names_List.Add(temp_instrument);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"error");
                    wb.Close(false); // Close the workbook without saving changes
                    excel.Quit();    // Quit the Excel application

                }


            }

            // Now you have the total rows and columns
            MessageBox.Show($"Total Rows: {lastRow}");
            MessageBox.Show($"Total Columns: {lastColumn}");
            wb.Close(false); // Close the workbook without saving changes
            excel.Quit();    // Quit the Excel application
            return new List<string>();
        }










        private void User_mode_button_click(object sender, RoutedEventArgs e)
        {
            User_mode = "User";
            aaa.Visibility = Visibility.Visible;
            Check_commands_lable.Visibility = Visibility.Hidden;
            MCU_Programing.Visibility = Visibility.Hidden;
            Realys_board_command_generate.Visibility = Visibility.Hidden;
            UART_Communication.Visibility = Visibility.Hidden;
            Add_test_equipment_and_commands.Visibility = Visibility.Hidden;
            Path.Visibility = Visibility.Hidden;
        }

        private void Technician_mode_button_Click(object sender, RoutedEventArgs e)
        {
            User_mode = "Technician";
            aaa.Visibility = Visibility.Visible;
            Path.Visibility = Visibility.Visible;
            Check_commands_lable.Visibility = Visibility.Visible;
            MCU_Programing.Visibility = Visibility.Visible;
            Realys_board_command_generate.Visibility = Visibility.Visible;
            UART_Communication.Visibility = Visibility.Visible;
            Add_test_equipment_and_commands.Visibility = Visibility.Visible;
        }

        private void button_generate_multi_relay_Click(object sender, RoutedEventArgs e)
        {

        }

        private void file_to_program_textBox_button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                file_to_program_textBox.Text = openFileDialog.FileName;

        }

        private void Programing_choose_hardware_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            string hardware_selected = (sender as ComboBox).SelectedItem as string;
            if (hardware_selected == "ST_Link")
                {
                    ///--- Visible STLink CONTROL ---///
                    stlink_software_text.Visibility = Visibility.Visible;
                    stlink_hardware_reset_button.Visibility = Visibility.Visible;
                    stlink_erase_button.Visibility = Visibility.Visible;
                    stlink_software_reset_button.Visibility = Visibility.Visible;
                    stlink_full_erase_text.Visibility = Visibility.Visible;
                    stlink_software_text.Visibility = Visibility.Visible;
                    stlink_programming_button.Visibility = Visibility.Visible;

                    ///--- Hide JLINK CONTROL ---///
                    jlink_MCU_Name_textBlock.Visibility = Visibility.Hidden;
                    JLINK_erase_button.Visibility = Visibility.Hidden;
                    jlink_uut_name_textBox.Visibility = Visibility.Hidden;
                    JLINK_program_button.Visibility = Visibility.Hidden;
                    how_many_uut_combobox.Visibility = Visibility.Hidden;
                    how_many_uut_textBlock.Visibility = Visibility.Hidden;

                }
                else
                {
                    ///--- Hidden STLink CONTROL ---///
                    stlink_software_text.Visibility = Visibility.Hidden;
                    stlink_hardware_reset_button.Visibility = Visibility.Hidden;
                    stlink_erase_button.Visibility = Visibility.Hidden;
                    stlink_software_reset_button.Visibility = Visibility.Hidden;
                    stlink_full_erase_text.Visibility = Visibility.Hidden;
                    stlink_software_text.Visibility = Visibility.Hidden;
                    stlink_programming_button.Visibility = Visibility.Hidden;


                    ///--- Visible JLINK CONTROL ---///
                    jlink_MCU_Name_textBlock.Visibility = Visibility.Visible;
                    JLINK_erase_button.Visibility = Visibility.Visible;
                    jlink_uut_name_textBox.Visibility = Visibility.Visible;
                    JLINK_program_button.Visibility = Visibility.Visible;
                    how_many_uut_combobox.Visibility = Visibility.Visible;
                    how_many_uut_textBlock.Visibility = Visibility.Visible;


                }
            

        }

        private void stlink_programming_button_Click(object sender, RoutedEventArgs e)
        {

            string STM32_Programer_CLI_Path = "H:\\STM32CubeProgrammer\\bin\\STM32_Programmer_CLI.exe";
            string Elf_File_To_Flash = file_to_program_textBox.Text;
            STM32_Programer_CLI test = new STM32_Programer_CLI(STM32_Programer_CLI_Path, Elf_File_To_Flash);
            programere_output_textbox.Text = test.Flash_STM32();
        }

        private void stlink_erase_button_Click(object sender, RoutedEventArgs e)
        {
            string STM32_Programer_CLI_Path = "H:\\STM32CubeProgrammer\\bin\\STM32_Programmer_CLI.exe";
            STM32_Programer_CLI test = new STM32_Programer_CLI(STM32_Programer_CLI_Path);

            programere_output_textbox.Text = test.Delete_STM32();
        }

        private void programere_output_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            programere_output_textbox.ScrollToEnd();

        }

        private void stlink_software_reset_button_Click(object sender, RoutedEventArgs e)
        {
            string STM32_Programer_CLI_Path = "H:\\STM32CubeProgrammer\\bin\\STM32_Programmer_CLI.exe";
            STM32_Programer_CLI test = new STM32_Programer_CLI(STM32_Programer_CLI_Path);
            programere_output_textbox.Text = test.Reset_STM32();
        }

        private void stlink_hardware_reset_button_Click(object sender, RoutedEventArgs e)
        {
            string STM32_Programer_CLI_Path = "H:\\STM32CubeProgrammer\\bin\\STM32_Programmer_CLI.exe";
            STM32_Programer_CLI test = new STM32_Programer_CLI(STM32_Programer_CLI_Path);
            programere_output_textbox.Text = test.Hard_Reset_STM32();
        }

        private void button5_Copy4_Click(object sender, RoutedEventArgs e)
        {
            programere_output_textbox.Text = "";
        }

      

        private void JLINK_erase_button_Click(object sender, RoutedEventArgs e)
        {
            JLINK test = new JLINK("STM32G431RB", "C:\\Program Files\\SEGGER\\JLink_V794k\\JLink.exe", 1, file_to_program_textBox.Text, "4000");
            test.Erase();
        }

        private void JLINK_program_button_Click(object sender, RoutedEventArgs e)
        {
            JLINK test = new JLINK("STM32G431RB", "C:\\Program Files\\SEGGER\\JLink_V794k\\JLink.exe", 1, file_to_program_textBox.Text, "4000");
            test.Program();
        }

        private void how_many_uut_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}