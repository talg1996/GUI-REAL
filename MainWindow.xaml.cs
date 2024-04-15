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
using System.Xml.Linq;
using System.Printing;
using System.Windows.Media;



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
        string[] Relay_Option = new string[] { "48 relays", "32 relays" };
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
            Init_Relay();
        }


        private void Init_Relay()
        {
            relays_options_comboBox.ItemsSource = Relay_Option;
            relays_options_comboBox.SelectedIndex = 0;
        }
        private void Init_Programing()
        {


            Programing_choose_hardware.ItemsSource = Programing_hardware;
            Programing_choose_hardware.SelectedIndex = 0;

            how_many_uut_combobox.ItemsSource = UUT_amount;
            how_many_uut_combobox.SelectedIndex = 0;

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

        private void button_generate_multi_relay_48_Click(object sender, RoutedEventArgs e)
        {
            string generated_command;
            if (address_input_ok(adress_input_command.Text))
            {
                string address = adress_input_command.Text;

                string command = address + "!002";
                if (relays_options_comboBox.SelectedIndex == 0)
                {
                    generated_command = command + get_realys_command48();
                }
                else
                {
                    generated_command = command + get_realys_command32();
                }
                relay_output_command.Text = generated_command + "<CR>";
                adress_input_command.Background = Brushes.White;
            }
            else
            {
                relay_output_command.Text = "Please enter 00-FF address input";
                adress_input_command.Background = Brushes.Pink;
            }


        }
        private bool address_input_ok(string input)
        {

            // Convert the string to uppercase for uniformity
            input = input.ToUpper();

            // Check if the string has exactly two characters
            if (input.Length != 2)
            {
                return false;
            }

            // Try parsing the string as a hexadecimal number
            if (int.TryParse(input, System.Globalization.NumberStyles.HexNumber, null, out int value))
            {
                // Check if the parsed value is between 0 and 255 (inclusive)
                if (value >= 0 && value <= 255)
                {
                    return true;
                }
            }

            return false;
        }

        private string get_realys_command48()
        {
            string A = BinaryToHex((checkBox_relay48.IsChecked == true ? "1" : "0") +
                       (checkBox_relay47.IsChecked == true ? "1" : "0") +
                       (checkBox_relay46.IsChecked == true ? "1" : "0") +
                       (checkBox_relay45.IsChecked == true ? "1" : "0"));

            string B = BinaryToHex((checkBox_relay44.IsChecked == true ? "1" : "0") +
                       (checkBox_relay43.IsChecked == true ? "1" : "0") +
                       (checkBox_relay42.IsChecked == true ? "1" : "0") +
                       (checkBox_relay41.IsChecked == true ? "1" : "0"));

            string C = BinaryToHex((checkBox_relay40.IsChecked == true ? "1" : "0") +
                       (checkBox_relay39.IsChecked == true ? "1" : "0") +
                       (checkBox_relay38.IsChecked == true ? "1" : "0") +
                       (checkBox_relay37.IsChecked == true ? "1" : "0"));

            string D = BinaryToHex((checkBox_relay36.IsChecked == true ? "1" : "0") +
                       (checkBox_relay35.IsChecked == true ? "1" : "0") +
                       (checkBox_relay34.IsChecked == true ? "1" : "0") +
                       (checkBox_relay33.IsChecked == true ? "1" : "0"));

            string E = BinaryToHex((checkBox_relay32.IsChecked == true ? "1" : "0") +
                       (checkBox_relay31.IsChecked == true ? "1" : "0") +
                       (checkBox_relay30.IsChecked == true ? "1" : "0") +
                       (checkBox_relay29.IsChecked == true ? "1" : "0"));

            string F = BinaryToHex((checkBox_relay28.IsChecked == true ? "1" : "0") +
                       (checkBox_relay27.IsChecked == true ? "1" : "0") +
                       (checkBox_relay26.IsChecked == true ? "1" : "0") +
                       (checkBox_relay25.IsChecked == true ? "1" : "0"));

            string G = BinaryToHex((checkBox_relay24.IsChecked == true ? "1" : "0") +
                       (checkBox_relay23.IsChecked == true ? "1" : "0") +
                       (checkBox_relay22.IsChecked == true ? "1" : "0") +
                       (checkBox_relay21.IsChecked == true ? "1" : "0"));

            string H = BinaryToHex((checkBox_relay20.IsChecked == true ? "1" : "0") +
                       (checkBox_relay19.IsChecked == true ? "1" : "0") +
                       (checkBox_relay18.IsChecked == true ? "1" : "0") +
                       (checkBox_relay17.IsChecked == true ? "1" : "0"));

            string I = BinaryToHex((checkBox_relay16.IsChecked == true ? "1" : "0") +
                       (checkBox_relay15.IsChecked == true ? "1" : "0") +
                       (checkBox_relay14.IsChecked == true ? "1" : "0") +
                       (checkBox_relay13.IsChecked == true ? "1" : "0"));

            string J = BinaryToHex((checkBox_relay12.IsChecked == true ? "1" : "0") +
                       (checkBox_relay11.IsChecked == true ? "1" : "0") +
                       (checkBox_relay10.IsChecked == true ? "1" : "0") +
                       (checkBox_relay9.IsChecked == true ? "1" : "0"));

            string K = BinaryToHex((checkBox_relay8.IsChecked == true ? "1" : "0") +
                       (checkBox_relay7.IsChecked == true ? "1" : "0") +
                       (checkBox_relay6.IsChecked == true ? "1" : "0") +
                       (checkBox_relay5.IsChecked == true ? "1" : "0"));

            string L = BinaryToHex((checkBox_relay4.IsChecked == true ? "1" : "0") +
                       (checkBox_relay3.IsChecked == true ? "1" : "0") +
                       (checkBox_relay2.IsChecked == true ? "1" : "0") +
                       (checkBox_relay1.IsChecked == true ? "1" : "0"));

            return A + B + C + D + E + F + H + I + J + K + L;
        }


        private string get_realys_command32()
        {
            

            string A = BinaryToHex((checkBox_relay32.IsChecked == true ? "1" : "0") +
                       (checkBox_relay31.IsChecked == true ? "1" : "0") +
                       (checkBox_relay30.IsChecked == true ? "1" : "0") +
                       (checkBox_relay29.IsChecked == true ? "1" : "0"));

            string B = BinaryToHex((checkBox_relay28.IsChecked == true ? "1" : "0") +
                       (checkBox_relay27.IsChecked == true ? "1" : "0") +
                       (checkBox_relay26.IsChecked == true ? "1" : "0") +
                       (checkBox_relay25.IsChecked == true ? "1" : "0"));

            string C = BinaryToHex((checkBox_relay24.IsChecked == true ? "1" : "0") +
                       (checkBox_relay23.IsChecked == true ? "1" : "0") +
                       (checkBox_relay22.IsChecked == true ? "1" : "0") +
                       (checkBox_relay21.IsChecked == true ? "1" : "0"));

            string D = BinaryToHex((checkBox_relay20.IsChecked == true ? "1" : "0") +
                       (checkBox_relay19.IsChecked == true ? "1" : "0") +
                       (checkBox_relay18.IsChecked == true ? "1" : "0") +
                       (checkBox_relay17.IsChecked == true ? "1" : "0"));

            string E = BinaryToHex((checkBox_relay16.IsChecked == true ? "1" : "0") +
                       (checkBox_relay15.IsChecked == true ? "1" : "0") +
                       (checkBox_relay14.IsChecked == true ? "1" : "0") +
                       (checkBox_relay13.IsChecked == true ? "1" : "0"));

            string F = BinaryToHex((checkBox_relay12.IsChecked == true ? "1" : "0") +
                       (checkBox_relay11.IsChecked == true ? "1" : "0") +
                       (checkBox_relay10.IsChecked == true ? "1" : "0") +
                       (checkBox_relay9.IsChecked == true ? "1" : "0"));

            string G = BinaryToHex((checkBox_relay8.IsChecked == true ? "1" : "0") +
                       (checkBox_relay7.IsChecked == true ? "1" : "0") +
                       (checkBox_relay6.IsChecked == true ? "1" : "0") +
                       (checkBox_relay5.IsChecked == true ? "1" : "0"));

            string H = BinaryToHex((checkBox_relay4.IsChecked == true ? "1" : "0") +
                       (checkBox_relay3.IsChecked == true ? "1" : "0") +
                       (checkBox_relay2.IsChecked == true ? "1" : "0") +
                       (checkBox_relay1.IsChecked == true ? "1" : "0"));

            return A + B + C + D + E + F + H ;
        }

        public string BinaryToHex(string binary)
        {
            // Check if the input string is not null or empty
            if (string.IsNullOrEmpty(binary))
                throw new ArgumentException("Input string cannot be null or empty.");

            // Pad the binary string with zeros to ensure it's a multiple of 4
            while (binary.Length % 4 != 0)
                binary = "0" + binary;

            // Convert binary string to hexadecimal
            string hex = "";
            for (int i = 0; i < binary.Length; i += 4)
            {
                string nibble = binary.Substring(i, 4);
                int decimalValue = Convert.ToInt32(nibble, 2);
                hex += decimalValue.ToString("X");
            }

            return hex;
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
            STLink test = new STLink(STM32_Programer_CLI_Path, Elf_File_To_Flash);
            programere_output_textbox.Text = test.STLink_Program_STM32();
        }

        private void stlink_erase_button_Click(object sender, RoutedEventArgs e)
        {
            string STM32_Programer_CLI_Path = "H:\\STM32CubeProgrammer\\bin\\STM32_Programmer_CLI.exe";
            STLink test = new STLink(STM32_Programer_CLI_Path);

            programere_output_textbox.Text = test.STLink_Erase_STM32();
        }

        private void programere_output_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            programere_output_textbox.ScrollToEnd();

        }

        private void stlink_software_reset_button_Click(object sender, RoutedEventArgs e)
        {
            string STM32_Programer_CLI_Path = "H:\\STM32CubeProgrammer\\bin\\STM32_Programmer_CLI.exe";
            STLink test = new STLink(STM32_Programer_CLI_Path);
            programere_output_textbox.Text = test.STLink_Reset_STM32();
        }

        private void stlink_hardware_reset_button_Click(object sender, RoutedEventArgs e)
        {
            string STM32_Programer_CLI_Path = "H:\\STM32CubeProgrammer\\bin\\STM32_Programmer_CLI.exe";
            STLink test = new STLink(STM32_Programer_CLI_Path);
            programere_output_textbox.Text = test.STLink_Hard_Reset_STM32();
        }

        private void button5_Copy4_Click(object sender, RoutedEventArgs e)
        {
            programere_output_textbox.Text = "";
        }



        private void JLINK_erase_button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(jlink_uut_name_textBox.Text) && !string.IsNullOrEmpty(how_many_uut_combobox.Text))
            {
                try
                {
                    JLINK test = new JLINK(jlink_uut_name_textBox.Text, "C:\\Program Files\\SEGGER\\JLink_V794k\\JLink.exe", int.Parse(how_many_uut_combobox.Text), file_to_program_textBox.Text, "4000");
                    Trace.WriteLine(how_many_uut_combobox.Text);
                    programere_output_textbox.Text = test.cmd_erase();

                }
                catch (Exception ex)
                {
                    programere_output_textbox.Text = ex.Message;
                }
            }
            else
                programere_output_textbox.Text = "Please enter all vlaues";





        }

        private void JLINK_program_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                JLINK test = new JLINK(jlink_uut_name_textBox.Text, "C:\\Program Files\\SEGGER\\JLink_V794k\\JLink.exe", int.Parse(how_many_uut_combobox.Text), file_to_program_textBox.Text, "4000");
                programere_output_textbox.Text = test.cmd_program();
            }
            catch (Exception ex)
            {
                programere_output_textbox.Text = ex.Message;
            }
        }

        private void how_many_uut_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private bool singleInputCommandOK(string input)
        {
            int how_many_relays=48-relays_options_comboBox.SelectedIndex*16;  // 48 or 32

            int number;
            // Check if the input can be parsed as an integer and is within the range
            if (int.TryParse(input, out number) && number >= 1 && number <=  how_many_relays)
            {
                return true; // Return true if input is valid
            }
            else
            {
                return false; // Return false if input is invalid
            }
        }
        private void button_generate_one_relay_Click(object sender, RoutedEventArgs e)
        {
            string input = single_input_command.Text;
            if (address_input_ok(adress_input_command.Text))
            {
                if (singleInputCommandOK(single_input_command.Text))
                {
                    string address = adress_input_command.Text;
                    string unfinish_command = "!" + address + "3";
                    string relay_hex_Value = GetHexValue(input);
                    string final_command = unfinish_command + relay_hex_Value + "<CR>";
                    single_output_command.Text = final_command;
                    single_input_command.Background = Brushes.White;
                    adress_input_command.Background = Brushes.White;
                }
                else
                {
                    single_output_command.Text = "Please enter relay 1-"+relays_options_comboBox.Text;
                    single_input_command.Background = Brushes.Pink;
                    adress_input_command.Background = Brushes.White;

                }
            }
            else
            {
                single_output_command.Text = "Please enter address 00-FF";
                adress_input_command.Background = Brushes.Pink;
                single_input_command.Background = Brushes.White;
            }


        }

        private string checkAdress(string text)
        {
            // Convert the string to uppercase for uniformity
            text = text.ToUpper();

            // Check if the string has exactly two characters
            if (text.Length != 2)
            {
                return "Please enter a hex number with exactly two characters.";
            }

            // Try parsing the string as a hexadecimal number
            if (int.TryParse(text, System.Globalization.NumberStyles.HexNumber, null, out int value))
            {
                // Check if the parsed value is between 0 and 255 (inclusive)
                if (value >= 0 && value <= 255)
                {
                    // Return the hexadecimal number as string
                    return text;
                }
            }

            return "Please enter a hex number between 00 and FF.";
        }



        static string GetHexValue(string input)
        {
            int number;
            int.TryParse(input, out number);


            // Convert number to hexadecimal
            return (number - 1).ToString("X2");
        }

        private void mark_unmark_checkboxes(string v)
        {

        }
        private void button_DeActive_generate_one_relay_Click(object sender, RoutedEventArgs e)
        {
            string input = single_input_command.Text;
            if (address_input_ok(adress_input_command.Text))
            {
                if (singleInputCommandOK(single_input_command.Text))
                {
                    string address = adress_input_command.Text;
                    string unfinish_command = "!" + address + "4";
                    string relay_hex_Value = GetHexValue(input);
                    string final_command = unfinish_command + relay_hex_Value + "<CR>";
                    single_output_command.Text = final_command;
                    single_input_command.Background = Brushes.White;
                    adress_input_command.Background = Brushes.White;
                }
                else
                {
                    single_output_command.Text = "Please enter relay 1-" + relays_options_comboBox.Text;
                    single_input_command.Background = Brushes.Pink;
                    adress_input_command.Background = Brushes.White;

                }
            }
            else
            {
                single_output_command.Text = "Please enter address 00-FF";
                adress_input_command.Background = Brushes.Pink;
                single_input_command.Background = Brushes.White;
            }


        }

        private void relays_options_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (relays_options_comboBox.SelectedIndex == 0)
            {
                textBlock1_Copy56.Text = "Relay 1-48";
               single_input_command.Text = "1-48";
            }
            else
            {
               textBlock1_Copy56.Text = "Relay 1-32";
                single_input_command.Text = "1-32";
            }

            string Chosen_Relay = (sender as ComboBox).SelectedItem as string;
            if (Chosen_Relay == "48 relays")
                ShowHideRelayControls("Visible");
            else if (Chosen_Relay == "32 relays")
                ShowHideRelayControls("Hidden");
        }
        private void ShowHideRelayControls(string op)
        {
            Visibility visibility = Visibility.Hidden; // Default visibility is Hidden

            // Determine the desired visibility based on the provided operation
            if (op == "Visible")
            {
                visibility = Visibility.Visible;
            }

            // Loop through the checkboxes from 33 to 48 and set their visibility
            for (int i = 33; i <= 48; i++)
            {
                string checkBoxName = "checkBox_relay" + i;
                string textBlockName = "textBlockRelay" + i;
                var checkBox = FindName(checkBoxName) as System.Windows.Controls.CheckBox;
                var textBlock = FindName(textBlockName) as System.Windows.Controls.TextBlock;
                if (checkBox != null)
                {
                    checkBox.Visibility = visibility;
                }
                if (textBlock != null)
                {
                    textBlock.Visibility = visibility;
                }
            }
        }

        
    }
}