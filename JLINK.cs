using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace GUI_REAL
{
    internal class JLINK
    {

        public string Device { get; set; }
        public string cliExecutablePath { get; set; }
        public string hex_File { get; set; }
        public int device_number { get; set; }
        public string Speed { get; set; }



        public JLINK(string Device,string cliExecutablePath,int device_number, string hex_File,string speed = "4000")
        {
            this.Device = Device;
            this.cliExecutablePath = cliExecutablePath;
            this.device_number = device_number;
            this.Speed = speed;
            this.hex_File = hex_File;
        }
        public void Erase()
        {
           
            string[] commands_1_UUTs = { "connect", Device, "J", "-1 -1", Speed, "erase" };
            string[] commands_2_UUTs = { "connect", Device, "J", "-1 -1", Speed, "erase", "JTAGConf 9 2", "connect", "erase" };
            string[] commands_3_UUTs = { "connect", Device, "J", "-1 -1", Speed, "erase", "JTAGConf 9 2", "connect", "erase", "JTAGConf 18 4", "connect", "erase" };
            string[] commands_4_UUTs = { "connect", Device, "J", "-1 -1", Speed, "erase", "JTAGConf 9 2", "connect", "erase", "JTAGConf 18 4", "connect", "erase", "JTAGConf 27 6", "connect", "erase" };
            string[] erase_commands;
            switch (device_number)
            {
                case 1:
                    erase_commands = commands_1_UUTs;
                    break;
                case 2:
                    erase_commands = commands_2_UUTs;
                    break;
                case 3:
                    erase_commands = commands_3_UUTs;
                    break;
                case 4:
                    erase_commands = commands_4_UUTs;
                    break;
                default:
                    erase_commands = commands_1_UUTs;
                    break;

            }
            // Delay between commands in milliseconds
            

            // Start the CLI process
            Process process = new Process();
            process.StartInfo.FileName = cliExecutablePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.Start();

            // Send commands to the CLI with delays
            foreach (string command in erase_commands)
            {
                process.StandardInput.WriteLine(command);
                
            }

            // Wait for user input to keep the console open
            
            

            // Close the CLI process
           
            // Read the output of the command
           
            
            // Wait for the process to finish
            process.WaitForExit();

         

            // Output the result      
           
        }
        public void Program()
        {

            string[] commands_1_UUTs = { "connect", Device, "J", "-1 -1", Speed, "LoadFile "+hex_File };
            string[] commands_2_UUTs = { "connect", Device, "J", "-1 -1", Speed, "LoadFile " + hex_File, "JTAGConf 9 2", "connect", "LoadFile " + hex_File };
            string[] commands_3_UUTs = { "connect", Device, "J", "-1 -1", Speed, "LoadFile " + hex_File, "JTAGConf 9 2", "connect", "LoadFile " + hex_File, "JTAGConf 18 4", "connect", "LoadFile " + hex_File };
            string[] commands_4_UUTs = { "connect", Device, "J", "-1 -1", Speed, "LoadFile " + hex_File, "JTAGConf 9 2", "connect", "LoadFile " + hex_File, "JTAGConf 18 4", "connect", "LoadFile " + hex_File, "JTAGConf 27 6", "connect", "LoadFile " + hex_File };
            string[] Program_commands;
            switch (device_number)
            {
                case 1:
                    Program_commands = commands_1_UUTs;
                    break;
                case 2:
                    Program_commands = commands_2_UUTs;
                    break;
                case 3:
                    Program_commands = commands_3_UUTs;
                    break;
                case 4:
                    Program_commands = commands_4_UUTs;
                    break;
                default:
                    Program_commands = commands_1_UUTs;
                    break;

            }
            // Delay between commands in milliseconds


            // Start the CLI process
            Process process = new Process();
            process.StartInfo.FileName = cliExecutablePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.Start();

            // Send commands to the CLI with delays
            foreach (string command in Program_commands)
            {
                process.StandardInput.WriteLine(command);

            }

            // Wait for user input to keep the console open



            // Close the CLI process

            // Read the output of the command


            // Wait for the process to finish
            process.WaitForExit();



            // Output the result      

        }
    }
}
