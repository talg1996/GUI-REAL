﻿using GUI_REAL.Classes;
using IAHAL;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Excel = Microsoft.Office.Interop.Excel;



namespace GUI_REAL
{



    /// <summary>
    /// Description: Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window, INotifyPropertyChanged
    {
        //*********** handle chosen instrument *************//
        Instrument temp_instrument = new Instrument(); // Temporary instrument instance
        Instrument chosen_instrument = new Instrument(); // chosen instrument instance
        List<Instrument> Instruments_Names_List = new List<Instrument>(); // List to store instrument names
        //*********** handle chosen instrument *************//

        //*********** handle chosen command*************//
        Command temp_Command = new Command(); // Temporary Command_List instance
        Command chosen_Command = new Command(); // chosen Command_List instance
        List<Command> Command_List = new List<Command>(); // List to store Command_List names
        List<Command> Command_List_per_instrument = new List<Command>(); // List to store Command_List names per label
        //*********** handle chosen command*************//

        //*********** handle flow *************//
        Command flow_Command = new Command();
        Instrument flow_Instrument = new Instrument();
        FlowInstruction tempFlowInstruction = new FlowInstruction();
        List<FlowInstruction> FlowInstructions_List = new List<FlowInstruction>();
        bool? stopFlow = false;
        //*********** handle flow *************//

        //*********** handle relay *************//
        List<string> IAIpAdressDevicesList = new List<string>();
        IADevices iadevs = new IADevices();
        IADevice chosenRelayBoard;
        //*********** handle relay *************//

        //*********** MCU choose*************//
        List<string> MCUProgramingList = new List<string>();
        //*********** MCU choose*************//

        // Those strings are the content of the combo boxes any combo box in the
        string[] Programing_hardware = new string[] { "ST_Link", "JLINK" };
        string[] Relay_Option = new string[] { "48 relays", "32 relays" };
        string[] UUT_amount = new string[] { "1", "2", "3", "4" };

        string result;//store the instriment result
        flowResult[] results = new flowResult[501]; // Will save all the result from the user flow


        string User_mode;// can be "user" or "technician"

        public event PropertyChangedEventHandler? PropertyChanged;


        public MainWindow()
        {
            // Lists to store names and command strings from Excel files
            string Instruments_path_file = "H:\\Project\\Instrunets.xlsx";
            string Commands_path_file = "H:\\Project\\Commands.xlsx";

            Update_Instrument_List(Instruments_path_file, Instruments_Names_List);
            Update_Commands_List(Commands_path_file, Command_List);
            updateIAIpAdressDevicesList();
            InitializeComponent();
            Init();

        }

        private List<string> Update_Commands_List(string commands_path_file, List<Command> command_List)
        {

            string[] temp = new string[10];
            string filePath = commands_path_file;

            Microsoft.Office.Interop.Excel.Application excel = null;
            Workbook wb = null;
            Worksheet ws = null;

            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application();
                wb = excel.Workbooks.Open(commands_path_file);
                ws = wb.Worksheets[1];

                int lastRow = ws.Cells[ws.Rows.Count, 1].End[XlDirection.xlUp].Row;
                int lastColumn = ws.Cells[1, ws.Columns.Count].End[XlDirection.xlToLeft].Column;

                for (int row = 2; row <= lastRow; row++)
                {
                    for (int column = 1; column <= lastColumn; column++)
                    {
                        temp[column - 1] = ws.Cells[row, column].Value?.ToString();
                    }

                    try
                    {
                        Command tempCommand = new Command();
                        tempCommand.Model = temp[0];
                        tempCommand.Name = temp[1];
                        tempCommand.SCPI_Command = temp[2];

                        command_List.Add(tempCommand);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                // Clean up resources
                if (ws != null) Marshal.ReleaseComObject(ws);
                if (wb != null)
                {
                    wb.Close(false); // Close the workbook without saving changes
                    Marshal.ReleaseComObject(wb);
                }
                if (excel != null)
                {
                    excel.Quit(); // Quit the Excel application
                    Marshal.ReleaseComObject(excel);
                }
            }

            return new List<string>();
        }

        /// <summary>
        /// Description: Init the combo boxes
        /// </summary>
        private void Init()
        {
            InitATRFlow();
            Init_Programing();
            Init_Relay();
            Init_InstrumentsAndCommands();
            InitMCUProgramming();
        }

        private void InitMCUProgramming()
        {



            string path = @"H:\Project\STM32MCUs.xlsx";

            Microsoft.Office.Interop.Excel.Application excel = null;
            Workbook wb = null;
            Worksheet ws = null;

            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application();
                wb = excel.Workbooks.Open(path);
                ws = wb.Worksheets[1];

                int lastRow = ws.Cells[ws.Rows.Count, 1].End[XlDirection.xlUp].Row;
                int lastColumn = ws.Cells[1, ws.Columns.Count].End[XlDirection.xlToLeft].Column;

                for (int row = 2; row <= lastRow; row++)
                {
                    for (int column = 1; column <= lastColumn; column++)
                        MCUProgramingList.Add(ws.Cells[row, column].Value?.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                // Clean up resources
                if (ws != null) Marshal.ReleaseComObject(ws);
                if (wb != null)
                {
                    wb.Close(false); // Close the workbook without saving changes
                    Marshal.ReleaseComObject(wb);
                }
                if (excel != null)
                {
                    excel.Quit(); // Quit the Excel application
                    Marshal.ReleaseComObject(excel);
                }
            }
            chooseMCU.ItemsSource = MCUProgramingList;
            chooseMCU.SelectedIndex = 0;
        }

        private void InitATRFlow()
        {
            string directoryPath = @"H:\Project\Flows\Projects";

            if (Directory.Exists(directoryPath))
            {
                string[] folderNames = Directory.GetDirectories(directoryPath);

                foreach (string folderName in folderNames)
                {
                    // Add folder name to ComboBox
                    productChooseCB.Items.Add(System.IO.Path.GetFileName(folderName));
                }
            }
            else
            {
                MessageBox.Show("Directory not found!");
            }
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
            string[] temp = new string[15];




            Microsoft.Office.Interop.Excel.Application excel = null;
            Workbook wb = null;
            Worksheet ws = null;

            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application();
                wb = excel.Workbooks.Open(path);
                ws = wb.Worksheets[1];

                int lastRow = ws.Cells[ws.Rows.Count, 1].End[XlDirection.xlUp].Row;
                int lastColumn = ws.Cells[1, ws.Columns.Count].End[XlDirection.xlToLeft].Column;

                for (int row = 2; row <= lastRow; row++)
                {
                    for (int column = 1; column <= lastColumn; column++)
                    {
                        temp[column - 1] = ws.Cells[row, column].Value?.ToString();
                    }

                    try
                    {
                        Instrument tempInstrument = new Instrument();
                        tempInstrument.Model = temp[0];
                        tempInstrument.Name = temp[1];
                        tempInstrument.Com = temp[2];
                        tempInstrument.Lan = temp[3];
                        tempInstrument.Visa_USB = temp[4];
                        tempInstrument.Visa_Lan = temp[5];
                        tempInstrument.IP = temp[6];
                        tempInstrument.ModbusIP = temp[7];
                        Instruments_Names_List.Add(tempInstrument);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                // Clean up resources
                if (ws != null) Marshal.ReleaseComObject(ws);
                if (wb != null)
                {
                    wb.Close(false); // Close the workbook without saving changes
                    Marshal.ReleaseComObject(wb);
                }
                if (excel != null)
                {
                    excel.Quit(); // Quit the Excel application
                    Marshal.ReleaseComObject(excel);
                }
            }

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

                string command = "!" + address + "2";
                if (relays_options_comboBox.SelectedIndex == 0)
                {
                    generated_command = command + get_realys_command48();
                }
                else
                {
                    generated_command = command + get_realys_command32();
                }
                relay_output_command.Text = generated_command;
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

            return A + B + C + D + E + F + G + H + I + J + K + L;
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

            return A + B + C + D + E + F + G + H;
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
                chooseMCU.Visibility = Visibility.Hidden;
                JLINK_erase_button.Visibility = Visibility.Hidden;
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
                JLINK_program_button.Visibility = Visibility.Visible;
                how_many_uut_combobox.Visibility = Visibility.Visible;
                how_many_uut_textBlock.Visibility = Visibility.Visible;
                chooseMCU.Visibility = Visibility.Visible;



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
            if ( !string.IsNullOrEmpty(how_many_uut_combobox.Text))
            {
                try
                {
                    JLINK test = new JLINK(chooseMCU.Text, "C:\\Program Files\\SEGGER\\JLink_V794k\\JLink.exe", int.Parse(how_many_uut_combobox.Text), file_to_program_textBox.Text, "4000");
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
                JLINK test = new JLINK(chooseMCU.Text, "C:\\Program Files\\SEGGER\\JLink_V794k\\JLink.exe", int.Parse(how_many_uut_combobox.Text), file_to_program_textBox.Text, "4000");
                programere_output_textbox.Text = test.cmd_program();
                // programere_output_textbox.Text = test.Program();
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
            int how_many_relays = 48 - relays_options_comboBox.SelectedIndex * 16;  // Set the amount of the relay 48 or 32

            int number;
            // Check if the input can be parsed as an integer and is within the range
            if (int.TryParse(input, out number) && number >= 1 && number <= how_many_relays)
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
        {
            comboBox_selected_command.ItemsSource = null;
            string lable = comboBox_Instrument_select.SelectedItem as string;

            string model = findModelPerLable(lable);
            updateComboboxCommand(model);

        }
        private string findModelPerLable(string label)
        {
            foreach (Instrument instrument in Instruments_Names_List)
            {
                if (instrument.Name == label)
                {

                    chosen_instrument = instrument;
                    // Add matching command to Command_List_per_instrument
                    return instrument.Model;
                }
            }

            return "none";
        }
        /// <summary>
        /// update global temp_Instrument per lable
        /// </summary>
        /// <param name="label"></param>
        public void findInstrumentPerLable(string label)
        {
            foreach (Instrument instrument in Instruments_Names_List)

                if (instrument.Name == label) temp_instrument = instrument;






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
            comboBox_selected_command.ItemsSource = Command_List_per_instrument.Select(command => command.Name).ToList();

        }

        private void comboBox_selected_command_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string command_name = comboBox_selected_command.SelectedItem as string;

            foreach (Command command in Command_List_per_instrument)
            {
                if (command.Name == command_name)
                {
                    chosen_Command = command;


                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Command command_with_args = new Command(chosen_Command);
            if (chosen_Command.SCPI_Command != null)
                command_with_args.SCPI_Command = chosen_Command.SCPI_Command.Replace("a1", textBox_arg1.Text).Replace("a2", textBox_arg2.Text).Replace("a3", textBox_arg3.Text);

            SendCommand send_Command = new SendCommand(command_with_args, chosen_instrument);
            result_output_textBox.Text = send_Command.SendCommandToInstrument();
            result = result_output_textBox.Text;
            SCPI_Command_output_textBox.Text = command_with_args.SCPI_Command;
            Lable_Command_output_textBox.Text = chosen_instrument.Name;


        }

        private void button_clear_output_Copy_Click(object sender, RoutedEventArgs e)
        {
            SCPI_Command_output_textBox.Text = "";
        }

        private void button_clear_output_Click(object sender, RoutedEventArgs e)
        {
            result_output_textBox.Text = "";
        }






        private CancellationTokenSource cancellationTokenSource;
        private ManualResetEventSlim pauseEvent = new ManualResetEventSlim(false);

        private async void btn_start_flow_test_Click(object sender, RoutedEventArgs e)
        {
            try
            {  // Create a new CancellationTokenSource
                cancellationTokenSource = new CancellationTokenSource();
                string project = productChooseCB.Text as string;
                string dash = chooseDashCB.Text as string;
                string flow = chosenFlowCB.Text as string;

                flow_output_textbox.Text = "Test started";
                string flowPath = $@"H:\Project\Flows\Projects\{project}\{dash}\{flow}.xlsx";
                upDateFlow(flowPath);

                // Start excuteFlow asynchronously with CancellationToken
                await excuteFlow(cancellationTokenSource.Token, project, dash, flow);
            }
            catch (Exception ex)
            {

                MessageBox.Show("cant find path");
            }

        }


        private async Task excuteFlow(CancellationToken cancellationToken, string project, string dash, string flow)
        {
            // Reset the pauseEvent when execution completes or is stopped
            pauseEvent.Reset();

            int index_to_save;
            string logfilePath = $@"H:\Project\Flows\Logs\{project}\{dash}\{flow}\{flowSerialUUT.Text}.xlsx";
            try
            {
                clearResultsArray();

                // Update UI from the UI thread
                Dispatcher.Invoke(() => flow_output_textbox.Text = "");
                int ExcuteRow = 0;// Mark the row that we cuu
                precentToFinishTB.Text = "0";
                foreach (FlowInstruction user_Instruction in FlowInstructions_List)

                {
                    ExcuteRow++;

                    // Check if cancellation is requested
                    if (cancellationToken.IsCancellationRequested)
                    {
                        // Clean up and exit the method
                        printResults(logfilePath);
                        return;
                    }

                    // Check for pause
                    while (pauseEvent.Wait(0))
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            printResults(logfilePath);
                            return;
                        }
                        // Wait for a short time before checking again
                        await Task.Delay(100);
                    }

                    // Asynchronously update UI with the current instruction and all previous instructions
                    Dispatcher.InvokeAsync(() =>
                   {
                       // Append the current instruction to the existing text in the textbox

                       flow_output_textbox.Text += ExcuteRow + ": " + user_Instruction.Lable + " --> " + user_Instruction.SCPI_Command + "\n";
                       precentToFinishTB.Text = (ExcuteRow * 100 / FlowInstructions_List.Count).ToString();
                   });

                    switch (user_Instruction.Lable)
                    {

                        case "math":
                            await math(user_Instruction);


                            break;
                        case "Delay":
                            await Task.Delay(int.Parse(user_Instruction.SCPI_Command), cancellationToken);
                            break;
                        case "Relay":
                            string ipAdress, command;

                            // Split the input string by comma
                            string[] parts = user_Instruction.SCPI_Command.Split(',');

                            // Extract IP address and command
                            ipAdress = parts[0];
                            command = parts[1];

                            //send the command
                            string response = "";
                            findRelayBoard(ipAdress);
                            try
                            {
                                chosenRelayBoard.SendReceive(command, ref response);
                            }
                            catch (Exception ex)
                            {
                                cancellationTokenSource?.Cancel();
                                MessageBox.Show("No relay found");

                            }

                            break;
                        case "heading":
                            index_to_save = int.Parse(user_Instruction.Index_To_Save);
                            results[index_to_save].Type = "heading";
                            results[index_to_save].Value = user_Instruction.SCPI_Command;
                            break;
                        case "label":
                            index_to_save = int.Parse(user_Instruction.Index_To_Save);
                            results[index_to_save].Type = "label";
                            results[index_to_save].Value = user_Instruction.SCPI_Command;
                            break;
                        case "test":

                            index_to_save = int.Parse(user_Instruction.Index_To_Save);

                            string[] testValues = user_Instruction.SCPI_Command.Split(',');
                            string Type, testName, divP, divN, AcceptedValue, index, measureValue;
                            Type = "Test";
                            index = testValues[0];
                            divP = testValues[2];
                            divN = testValues[3];
                            AcceptedValue = testValues[1];
                            testName = testValues[4];
                            measureValue = results[int.Parse(index)].Value.Replace("\n", ""); ;

                            flowResult current = new flowResult(Type, measureValue, AcceptedValue, divP, divN);
                            results[index_to_save].Value = testName + ":" + divN + ":" + measureValue + ":" + divP + ":" + current.isItPass();
                            results[index_to_save].Type = Type;

                            break;

                        default:
                            findInstrumentPerLable(user_Instruction.Lable); // Update the global temp instrument
                            SendCommand user_send_command = new SendCommand(user_Instruction.SCPI_Command, temp_instrument);
                            if (user_Instruction.Index_To_Save != "none")
                            {


                                try
                                {
                                    index_to_save = int.Parse(user_Instruction.Index_To_Save);
                                    results[index_to_save].Type = "measure";
                                    results[index_to_save].Value = user_send_command.SendCommandToInstrument();
                                }
                                catch (FormatException)
                                {
                                    MessageBox.Show("Please insert index 0-499 or none to Index to save at flow excel");
                                }

                            }
                            else
                            {
                                user_send_command.SendCommandToInstrument();
                            }
                            break;
                    }




                    // Use Task.Delay instead of Thread.Sleep
                    await Task.Delay(80, cancellationToken);
                    // Write the result to the file



                }
                printResults(logfilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
                printResults(logfilePath);


            }

        }

        private async Task math(FlowInstruction user_Instruction)
        {

            float number1, number2;
            float result;
            string SCPIcommand = user_Instruction.SCPI_Command;
            int indexToSave = int.Parse(user_Instruction.Index_To_Save);
            try
            {
                string[] arr = SCPIcommand.Split(',');
                if (arr.Length != 3)
                {
                    // Display MessageBox for invalid SCPI command format
                    MessageBox.Show("Invalid SCPI command format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }




                // Parsing indices and operation
                int index1 = int.Parse(arr[0]);
                string operation = arr[1];
                int index2 = int.Parse(arr[2]);

                number1 = float.Parse(results[index1].Value);
                number2 = float.Parse(results[index2].Value);

                result = calc(ref number1, ref number2, ref operation);
                results[indexToSave].Type = "Math";
                results[indexToSave].Value = result.ToString();

                flow_output_textbox.Text += $"{number1} " + operation + $" {number2}" + " = " + results[indexToSave].Value;
            }
            catch
            {
                MessageBox.Show("Invalid Math format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private float calc(ref float number1, ref float number2, ref string operation)
        {
            switch (operation)
            {
                case "+":
                    return number1 + number2;
                case "-":
                    return number1 - number2;
                case "*":
                    return number1 * number2;
                case "/":
                    if (number2 != 0)
                        return number1 / number2;
                    else
                    {
                        // Display MessageBox for divide by zero error
                        MessageBox.Show("Cannot divide by zero.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return float.NaN; // Return NaN to signify an error
                    }
                default:
                    // Display MessageBox for invalid operation symbol
                    MessageBox.Show("Invalid operation symbol.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return float.NaN; // Return NaN to signify an error
            }
        }


        private void findRelayBoard(string ipAdress)
        {
            chosenRelayBoard = null;
            foreach (IADevice board in iadevs)
            {
                if (ipAdress == board.Address)
                {
                    chosenRelayBoard = board;

                }
            }

        }



        private void upDateFlowOutput(FlowInstruction user_Instruction)
        {
            flow_output_textbox.Text += user_Instruction.Lable + "-->" + user_Instruction.SCPI_Command + "\n";
        }

        private void printResults(string logFilePath)
        {
            bool isTestPass = true;
            if (string.IsNullOrEmpty(logFilePath))
            {
                throw new ArgumentException("Log file path cannot be null or empty.");
            }

            Microsoft.Office.Interop.Excel.Application excel = null;
            Workbook wb = null;
            Worksheet ws = null;


            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Visible = false;       // Hide Excel application
                excel.DisplayAlerts = false; // Disable alerts


                if (File.Exists(logFilePath))
                {
                    wb = excel.Workbooks.Open(logFilePath);
                }
                else
                {
                    wb = excel.Workbooks.Add();
                }

                ws = wb.Worksheets[1];



                int excelRow = 1;
                int excelCol = 1;

                ws.Cells[excelRow, excelCol] = "Tested by " + flowTesterName.Text;
                ws.Cells[excelRow, excelCol].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Plum);
                ws.Cells[excelRow, excelCol].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; // Set border style
                ws.Cells[excelRow, excelCol].Borders.Weight = Excel.XlBorderWeight.xlThin; // Set border weight
                excelRow++;

                ws.Cells[excelRow, excelCol] = "Room temp is  " + flowTempeture.Text;
                ws.Cells[excelRow, excelCol].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Plum);
                ws.Cells[excelRow, excelCol].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; // Set border style
                ws.Cells[excelRow, excelCol].Borders.Weight = Excel.XlBorderWeight.xlThin; // Set border weight
                excelRow++;

                ws.Cells[excelRow, excelCol] = "UUt serial is  " + flowSerialUUT.Text;
                ws.Cells[excelRow, excelCol].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Plum);
                ws.Cells[excelRow, excelCol].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; // Set border style
                ws.Cells[excelRow, excelCol].Borders.Weight = Excel.XlBorderWeight.xlThin; // Set border weight
                excelRow++;


                foreach (flowResult result in results)
                {
                    excelCol = 1;
                    if (result.Type == "Test" || result.Type == "heading" || result.Type == "label")
                    {
                        if (result.Type == "Test")
                        {
                            string[] labels = ["Value tested", "Low", "Measure", "High", "Result"];
                            foreach (string str in labels)
                            {

                                ws.Cells[excelRow, excelCol] = str; // Always write to first column
                                ws.Cells[excelRow, excelCol].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightCyan);
                                ws.Cells[excelRow, excelCol].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                ws.Cells[excelRow, excelCol].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; // Set border style
                                ws.Cells[excelRow, excelCol].Borders.Weight = Excel.XlBorderWeight.xlMedium; // Set border weight
                                ws.Cells[excelRow, excelCol].Font.Bold = true;
                                excelCol++;

                            }
                            excelRow++;
                            excelCol = 1;
                            string[] arr = result.Value.Split(':');
                            foreach (string str in arr)
                            {
                                ws.Cells[excelRow, excelCol].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; // Set border style
                                ws.Cells[excelRow, excelCol].Borders.Weight = Excel.XlBorderWeight.xlThin; // Set border weight

                                ws.Cells[excelRow, excelCol] = str; // Always write to first column
                                if (str == "Pass")
                                {
                                    ws.Cells[excelRow, excelCol].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGreen);
                                    ws.Cells[excelRow, excelCol].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    ws.Cells[excelRow, excelCol].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; // Set border style
                                    ws.Cells[excelRow, excelCol].Borders.Weight = Excel.XlBorderWeight.xlMedium; // Set border weight
                                    ws.Cells[excelRow, excelCol].Font.Bold = true;
                                }
                                else if (str == "Fail")
                                {
                                    isTestPass = false;
                                    ws.Cells[excelRow, excelCol].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightSalmon);
                                    ws.Cells[excelRow, excelCol].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    ws.Cells[excelRow, excelCol].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; // Set border style
                                    ws.Cells[excelRow, excelCol].Borders.Weight = Excel.XlBorderWeight.xlMedium; // Set border weight
                                    ws.Cells[excelRow, excelCol].Font.Bold = true;

                                }

                                excelCol++;

                            }
                            excelRow++;
                        }
                        else if (result.Type == "heading")
                        {

                            ws.Cells[excelRow, excelCol] = result.Value; // Always write to first column
                            ws.Cells[excelRow, excelCol].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow);
                            ws.Cells[excelRow, excelCol].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            ws.Cells[excelRow, excelCol].Font.Bold = true;
                            ws.Cells[excelRow, excelCol].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; // Set border style
                            ws.Cells[excelRow, excelCol].Borders.Weight = Excel.XlBorderWeight.xlMedium; // Set border weight

                            excelRow++;
                        }
                        else if (result.Type == "label")
                        {

                            ws.Cells[excelRow, excelCol] = result.Value; // Always write to first column
                            ws.Cells[excelRow, excelCol].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Orange);
                            ws.Cells[excelRow, excelCol].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            ws.Cells[excelRow, excelCol].Font.Bold = true;
                            ws.Cells[excelRow, excelCol].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; // Set border style
                            ws.Cells[excelRow, excelCol].Borders.Weight = Excel.XlBorderWeight.xlMedium; // Set border weight

                            excelRow++;
                        }

                    }

                }




                // Select all cells in the worksheet
                Excel.Range allCells = ws.Cells;

                // Set horizontal alignment to center for all cells
                allCells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;


                // Auto-fit columns
                Excel.Range columns = ws.UsedRange.Columns;
                columns.AutoFit();

                // Auto-fit rows
                Excel.Range rows = ws.UsedRange.Rows;
                rows.AutoFit();


                // Save and close workbook
                // Check if the directory exists
                string directoryPath = System.IO.Path.GetDirectoryName(logFilePath);
                if (!Directory.Exists(directoryPath))
                {
                    // Create the directory if it doesn't exist
                    Directory.CreateDirectory(directoryPath);
                }

                // Now you can save your Excel file
                wb.SaveAs(logFilePath);

                wb.Close();

                playFinishSound(isTestPass);
            }
            catch (Exception ex)
            {
                // Handle exceptions gracefully
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            finally
            {
                // Clean up resources
                if (ws != null) Marshal.ReleaseComObject(ws);
                if (wb != null) Marshal.ReleaseComObject(wb);
                if (excel != null)
                {
                    excel.Quit();
                    Marshal.ReleaseComObject(excel);
                }
            }
        }

        private void playFinishSound(bool isTestPass)
        {
            try
            {
                string soundFilePath;
                // Path to the sound file
                if (isTestPass == true)
                {
                    soundFilePath = "H:\\Project\\Sounds\\passSound.wav";
                }
                else
                {
                    soundFilePath = "H:\\Project\\Sounds\\failSound.wav";

                }

                // Create a SoundPlayer instance with the sound file path
                SoundPlayer player = new SoundPlayer(soundFilePath);

                // Play the sound
                player.Play();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("An error occurred. Please try again later.", "Error");
            }
        }

        private void clearResultsArray()
        {
            foreach (flowResult result in results)
            {
                result.deleteResult();
            }
        }

        private void upDateFlow(string flowPath)
        {
            FlowInstructions_List.Clear();
            string[] temp = new string[10];
            string filePath = flowPath;
            Microsoft.Office.Interop.Excel.Application excel = null;
            Workbook wb = null;
            Worksheet ws = null;

            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application();
                wb = excel.Workbooks.Open(flowPath);
                ws = wb.Worksheets[1];

                int lastRow = ws.Cells[ws.Rows.Count, 1].End[XlDirection.xlUp].Row;
                int lastColumn = ws.Cells[1, ws.Columns.Count].End[XlDirection.xlToLeft].Column;

                for (int row = 2; row <= lastRow; row++)
                {
                    for (int column = 1; column <= lastColumn - 1; column++)
                    {
                        temp[column - 1] = ws.Cells[row, column].Value?.ToString();
                    }

                    try
                    {
                        FlowInstruction tempFlowInstruction = new FlowInstruction();
                        tempFlowInstruction.Lable = temp[0];
                        tempFlowInstruction.SCPI_Command = temp[1];
                        tempFlowInstruction.Index_To_Save = temp[2];

                        FlowInstructions_List.Add(tempFlowInstruction);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                // Clean up resources
                if (ws != null) Marshal.ReleaseComObject(ws);
                if (wb != null)
                {
                    wb.Close(false); // Close the workbook without saving changes
                    Marshal.ReleaseComObject(wb);
                }
                if (excel != null)
                {
                    excel.Quit(); // Quit the Excel application
                    Marshal.ReleaseComObject(excel);

                }
            }
        }



        private void stopFlowBTNClick(object sender, RoutedEventArgs e)
        {
            // Cancel the CancellationTokenSource when the stop button is clicked
            cancellationTokenSource?.Cancel();
        }
        private void pauseFlowBTNClick(object sender, RoutedEventArgs e)
        {
            pauseEvent.Set(); // Set the ManualResetEventSlim to signal pause
        }
        private void resumeFlowBTNClick(object sender, RoutedEventArgs e)
        {
            pauseEvent.Reset(); // Reset the ManualResetEventSlim to signal resume
        }

        private void flow_output_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            flow_output_textbox.ScrollToEnd();
        }
        private void updateIAIpAdressDevicesList()
        {
            IAError ec = iadevs.DetectTCPDevices();
            if (ec != IAError.IA_OK)
            {
                MessageBox.Show(iadevs.GetErrorMessage(ec));
                return;
            }
            foreach (IADevice b in iadevs)
            {
                IAIpAdressDevicesList.Add(b.Address);
            }

            //RelayCB.ItemsSource = IAIpAdressDevicesList;
        }

        private void productChooseCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            chooseDashCB.Items.Clear();
            string? chosenProject = productChooseCB.SelectedItem as string;
            if (chosenProject != null)
            {
                string directoryPath = @"H:\Project\Flows\Projects\" + chosenProject;

                if (Directory.Exists(directoryPath))
                {
                    string[] folderNames = Directory.GetDirectories(directoryPath);

                    foreach (string folderName in folderNames)
                    {
                        // Add folder name to ComboBox
                        chooseDashCB.Items.Add(System.IO.Path.GetFileName(folderName));
                    }
                }
                else
                {
                    MessageBox.Show("Directory not found!");
                }
            }

        }

        private void chooseDashCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            chosenFlowCB.Items.Clear();
            string fileName;

            string? chosenDash = chooseDashCB.SelectedItem as string;
            if (chosenDash != null)
            {
                string directoryPath = $@"H:\Project\Flows\Projects\{productChooseCB.SelectedItem}\{chosenDash}";

                if (Directory.Exists(directoryPath))
                {
                    string[] excelFiles = Directory.GetFiles(directoryPath, "*.xlsx");

                    foreach (string excelFile in excelFiles)
                    {
                        fileName = System.IO.Path.GetFileNameWithoutExtension(excelFile);
                        // Add Excel file name to ComboBox
                        chosenFlowCB.Items.Add(fileName);
                    }
                }
                else
                {
                    MessageBox.Show("Directory not found!");
                }
            }
        }

        private void chooseMCU_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string chosenMCU = (sender as ComboBox).SelectedItem as string;
            Trace.WriteLine(chosenMCU);

        }
    }

}
