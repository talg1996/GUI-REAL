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
using System.Reflection.Emit;



namespace GUI_REAL
{



    /// <summary>
    /// Description: Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window, INotifyPropertyChanged
    {
        Instrument temp_instrument = new Instrument(); // Temporary instrument instance
        List<Instrument> Instruments_Names_List = new List<Instrument>(); // List to store instrument names

        Command temp_Command = new Command(); // Temporary Command_List instance
        List<Command> Command_List = new List<Command>(); // List to store Command_List names

        List<Command> Command_List_per_instrument = new List<Command>(); // List to store Command_List names per label


        // Those strings are the content of the combo boxes any combo box in the
        string[] Programing_hardware = new string[] { "ST_Link", "JLINK" }; 
        string[] Relay_Option =        new string[] { "48 relays", "32 relays" };
        string[] UUT_amount =         new string[]  { "1", "2", "3", "4" };



        
        string User_mode;// can be "user" or "technician"

        public event PropertyChangedEventHandler? PropertyChanged;


        public MainWindow()
        {
            // Lists to store names and command strings from Excel files
            string Instruments_path_file = "H:\\Project\\Instrunets.xlsx";
            string Commands_path_file = "H:\\Project\\Commands.xlsx";

            Update_Instrument_List(Instruments_path_file, Instruments_Names_List);
            Update_Commands_List(Commands_path_file, Command_List);
            InitializeComponent();
            Init();
            
        }

        private List<string> Update_Commands_List(string commands_path_file, List<Command> command_List)
        {

            string[] temp = new string[10];
            string filePath = commands_path_file;
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
                for (int column = 1; column <= lastColumn; column++)
                {
                    temp[column - 1] = ws.Cells[row, column].Value?.ToString();
                }
                try
                {
                    temp_Command.Model = temp[0];
                    temp_Command.Name = temp[1];
                    temp_Command.SCPI_Command = temp[2];
                    
                    command_List.Add(temp_Command);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"error");
                    wb.Close(false); // Close the workbook without saving changes
                    excel.Quit();    // Quit the Excel application

                }


            }


            wb.Close(false); // Close the workbook without saving changes
            excel.Quit();    // Quit the Excel application
            return new List<string>();
        }

        /// <summary>
        /// Description: Init the combo boxes
        /// </summary>
        private void Init()
        {
            Init_Programing();
            Init_Relay();
            Init_InstrumentsAndCommands();
        }

        /// <summary>
        /// Description: init the relays options
        /// </summary>
        /// 
        private void Init_InstrumentsAndCommands()
        {
            comboBox_Instrument_select.ItemsSource = Instruments_Names_List.Select(instrument => instrument.Name).ToList();
            comboBox_Instrument_select.SelectedIndex = 0;

            


        }

            private void Init_Relay()
        {  
            relays_options_comboBox.ItemsSource = Relay_Option;
            relays_options_comboBox.SelectedIndex = 0; // Set default value so it wont be empty
        }

        /// <summary>
        /// Description: Init the programing options
        /// </summary>
        private void Init_Programing()
        {


            Programing_choose_hardware.ItemsSource = Programing_hardware;
            Programing_choose_hardware.SelectedIndex = 0;

            how_many_uut_combobox.ItemsSource = UUT_amount;
            how_many_uut_combobox.SelectedIndex = 0;

        }

        /// <summary>
        /// Description: Update the List according to the file specific at the path 
        /// </summary>
        /// <param name="Instruments_path_file"></param>
        /// <param name="Instruments_Names_List"></param>
        private void Update_Instrument_List(string Instruments_path_file, List<Instrument> Instruments_Names_List)
        {
             Excel_Row_read_by_index(Instruments_path_file);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
                for (int column = 1; column <= lastColumn; column++)
                {
                    temp[column - 1] = ws.Cells[row, column].Value?.ToString();
                }
                try
                {
                    temp_instrument.Model = temp[0];
                    temp_instrument.Name = temp[1];
                    temp_instrument.Com = temp[2];
                    temp_instrument.Lan = temp[3];
                    temp_instrument.Visa_USB = temp[4];
                    temp_instrument.Visa_Lan = temp[5];
                    temp_instrument.IP = temp[6];
                    Instruments_Names_List.Add(temp_instrument);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"error");
                    wb.Close(false); // Close the workbook without saving changes
                    excel.Quit();    // Quit the Excel application

                }


            }

           
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
        /// <summary>
        /// Description: Check if the input is 0x(00-FF)
        /// </summary>
        /// <param name="input"></param>
        /// <returns>True  is the input is = 0x(00-FF)</returns>
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
        /// <summary>
        /// Description:this function return the code to relay base on the check box used for 42 relay code generator
        /// </summary>
        /// <returns>ABCDEFGHIJKL each letter can be 0x(0,1,2...,F)</returns>
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

        /// <summary>
        /// Description:this function return the code to relay base on the check box used for 32 relay code generator
        /// </summary>
        /// <returns>ABCDEFGHIJKL each letter can be 0x(0,1,2...,H)</returns>
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


        /// <summary>
        /// Description: This func take string of binary code and translate it to hex string
        /// </summary>
        /// <param name="binary"></param>
        /// <returns> string with the hex value of the binary code</returns>
        /// <exception cref="ArgumentException"></exception>
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

        

/// <summary>
/// s
/// </summary>
/// <param name="input"></param>
/// <returns></returns>
        private bool singleInputCommandOK(string input)
        {
            int how_many_relays=48-relays_options_comboBox.SelectedIndex*16;  // Set the amount of the relay 48 or 32

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
        /// <summary>
        /// Check if the address is between 0x(00-FF)
        /// </summary>
        /// <param name="text"></param>
        /// <returns> return true if the address is between 0x(00-FF) else false </returns>
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


        /// <summary>
        ///Convert number to hexadecimal
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static string GetHexValue(string input)
        {
            int number;
            int.TryParse(input, out number);


            // Convert number to hexadecimal
            return (number - 1).ToString("X2");
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

        /// <summary>
        /// Resposibal on UI depend of 32 or 48 relays. Show / hide check box
        /// </summary>
        /// <param name="op"></param>
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

        private void comboBox_Instrument_select_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { string lable= comboBox_Instrument_select.SelectedItem as string;
            Trace.WriteLine(lable);
           string model=  findModelPerLable(lable);
            updateComboboxCommand(model);
           
        }
        private string findModelPerLable(string label)
        {
            foreach (Instrument instrument in Instruments_Names_List)
            {
                if (instrument.Name == label)
                {
                    // Add matching command to Command_List_per_instrument
                    return instrument.Model;
                }
            }

            return "none";
        }
        private void updateComboboxCommand(string model)
        {

            // Clear the contents of Command_List_per_instrument
            Command_List_per_instrument.Clear();

            // Iterate through Command_List to find commands with matching Name
            foreach (Command command in Command_List)
            {
                if (command.Model == model)
                {
                    // Add matching command to Command_List_per_instrument
                    Command_List_per_instrument.Add(command);
                }
            }

            // Set the ItemsSource of comboBox_command to Command_List_per_instrument
            comboBox_command.ItemsSource = Command_List_per_instrument.Select(command => command.Name).ToList();

        }
    }

}
