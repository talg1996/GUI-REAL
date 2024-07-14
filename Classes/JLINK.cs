using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace GUI_REAL.Classes
{
    internal class JLINK
    {
        // Properties declaration
        public string Device { get; set; }              //The MCU type like "STM32G747RE"
        public string CliExecutablePath { get; set; }   //Path to Segger CL
        public string HexFile { get; set; }             // Path to .hex file to program to MCU
        public int DeviceNumber { get; set; }           //How many MCU are connecting with JLINK as daisy chain
        public string Speed { get; set; }               // I set the speed to 4000Hz

        // Constructor
        public JLINK(string device, string cliExecutablePath, int deviceNumber, string hexFile, string speed = "4000")
        {
            Device = device;
            CliExecutablePath = cliExecutablePath;
            DeviceNumber = deviceNumber;
            Speed = speed;
            HexFile = hexFile;
        }

        /// <summary>
        /// Description: This method erases the MCU with the option to daisy chain (device number)
        /// </summary>
        /// <returns>Output from the erase operation</returns>
        public string Erase()
        {
            // Commands for different UUTs
            string[] commands1UUTs = { "connect", Device, "J", "-1 -1", Speed, "erase", "Exit" };
            string[] commands2UUTs = { "connect", Device, "J", "-1 -1", Speed, "erase", "JTAGConf 9 2", "connect", "erase" };
            string[] commands3UUTs = { "connect", Device, "J", "-1 -1", Speed, "erase", "JTAGConf 9 2", "connect", "erase", "JTAGConf 18 4", "connect", "erase" };
            string[] commands4UUTs = { "connect", Device, "J", "-1 -1", Speed, "erase", "JTAGConf 9 2", "connect", "erase", "JTAGConf 18 4", "connect", "erase", "JTAGConf 27 6", "connect", "erase" };
            string[] eraseCommands; //The command array that will be sent to the Segger CL

            // Assign appropriate eraseCommands based on how many chained (device number)
            switch (DeviceNumber)
            {
                case 1:
                    eraseCommands = commands1UUTs;
                    break;
                case 2:
                    eraseCommands = commands2UUTs;
                    break;
                case 3:
                    eraseCommands = commands3UUTs;
                    break;
                case 4:
                    eraseCommands = commands4UUTs;
                    break;
                default:
                    eraseCommands = commands1UUTs;
                    break;
            }

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = CliExecutablePath;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardInput = true;
                    process.Start();

                    // Send commands to process
                    foreach (string command in eraseCommands)
                    {
                        process.StandardInput.WriteLine(command);
                    }

                    // Read and return output
                    string output = process.StandardOutput.ReadToEnd();
                    process.Kill();
                    process.WaitForExit();
                    return output;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return $"Error: {ex.Message}";
            }
        }

        /// <summary>
        /// Description: This method programs the MCU with the option to daisy chain programming (device number)
        /// </summary>
        public string Program()
        {
            // Commands for different UUTs
            string[] commands1UUTs = { "connect", Device, "J", "-1 -1", Speed, "LoadFile " + HexFile, "reset", "go" };
            string[] commands2UUTs = { "connect", Device, "J", "-1 -1", Speed, "LoadFile " + HexFile, "reset", "go", "JTAGConf 9 2", "connect", "LoadFile " + HexFile, "reset", "go" };
            string[] commands3UUTs = { "connect", Device, "J", "-1 -1", Speed, "LoadFile " + HexFile, "reset", "go", "JTAGConf 9 2", "connect", "LoadFile " + HexFile, "reset", "go", "JTAGConf 18 4", "connect", "LoadFile " + HexFile, "reset", "go" };
            string[] commands4UUTs = { "connect", Device, "J", "-1 -1", Speed, "LoadFile " + HexFile, "reset", "go", "JTAGConf 9 2", "connect", "LoadFile " + HexFile, "reset", "go", "JTAGConf 18 4", "connect", "LoadFile " + HexFile, "reset", "go", "JTAGConf 27 6", "connect", "LoadFile " + HexFile, "reset", "go", "exit" };

            string[] programCommands; //The command array that will be sent to the Segger CL

            // Assign appropriate commands based on device number
            switch (DeviceNumber)
            {
                case 1:
                    programCommands = commands1UUTs;
                    break;
                case 2:
                    programCommands = commands2UUTs;
                    break;
                case 3:
                    programCommands = commands3UUTs;
                    break;
                case 4:
                    programCommands = commands4UUTs;
                    break;
                default:
                    programCommands = commands1UUTs;
                    break;
            }

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = CliExecutablePath;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardInput = true;
                    process.Start();

                    // Send commands to process
                    foreach (string command in programCommands)
                    {
                        process.StandardInput.WriteLine(command);
                    }

                    // Wait for process to exit
                    process.WaitForExit();
                    return "Program executed successfully.";
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return $"Error: {ex.Message}";
            }
        }

        /// <summary>
        /// Description: This method sends commands to JLink.exe via cmd and returns the output as a string so it is possible to present it at GUI 
        /// </summary>
        public string cmd_program()
        {
            try
            {
                string jLinkExecutable = @"H:\JLink.exe";
                string jLinkCommandFilePath = @"H:\JLinkCommandFile.jlink";
                string newComments = "";
                if (DeviceNumber == 1)
                {
                    newComments = "device " + Device + "\n" + "si JTAG\n" + "speed 4000\n" + "jtagconf -1,-1\n" + "connect\n" + "LoadFile " + HexFile + "\n" + "reset\n" + "go\n "+"Exit\n";
                }
                if (DeviceNumber == 2)
                {
                    newComments = "device " + Device + "\n" + "si JTAG\n" + "speed 4000\n" + "jtagconf -1,-1\n" + "connect\n" + "LoadFile " + HexFile + "\n" + "reset\n" + "go\n " + "JTAGConf 9 2" + "\n" + "connect" + "\n" + "LoadFile " + HexFile + "\n" + "reset\n" + "go\n " + "Exit\n";

                }

                if (DeviceNumber == 3)
                {
                    newComments = "device " + Device + "\n" + "si JTAG\n" + "speed 4000\n" + "jtagconf -1,-1\n" + "connect\n" + "LoadFile " + HexFile + "\n" + "reset\n" + "go\n " + "JTAGConf 9 2" + "\n" + "connect" + "\n" + "LoadFile " + HexFile + "\n" + "reset\n" + "go\n " + "JTAGConf 18 4" + "\n" + "connect" + "\n" + "LoadFile " + HexFile + "\n" + "reset\n" + "go\n " + "Exit\n";

                }
                if (DeviceNumber == 4)
                {
                    newComments = "device " + Device + "\n" + "si JTAG\n" + "speed 4000\n" + "jtagconf -1,-1\n" + "connect\n" + "LoadFile " + HexFile + "\n" + "reset\n" + "go\n " + "JTAGConf 9 2"+ "\n"+ "connect"+ "\n"+ "LoadFile " + HexFile+ "\n" + "reset\n" + "go\n " + "JTAGConf 18 4"+ "\n"+ "connect"+ "\n"+ "LoadFile " + HexFile+ "\n" + "reset\n" + "go\n " + "JTAGConf 27 6"+ "\n"+ "connect"+ "\n"+ "LoadFile " + HexFile +"\n"+ "reset\n" + "go\n "+ "Exit\n";
    
                }

                            // Write only the new comments to the file
                            File.WriteAllText(jLinkCommandFilePath, newComments);
                // Create process start information
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "cmd.exe";
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = true;
                psi.UseShellExecute = false;

                // Start the process
                Process process = Process.Start(psi);

                // Write the command to cmd
                process.StandardInput.WriteLine($"{jLinkExecutable} -CommandFile {jLinkCommandFilePath}");

                // Close the input stream to indicate that we have finished writing the command
                process.StandardInput.Close();

                // Read the output
                string output = process.StandardOutput.ReadToEnd();

                // Wait for the process to exit
                process.WaitForExit();

                string pattern = @"JTAG chain detection found (\d+) devices:";

                // Match the pattern in the output string
                Match match = Regex.Match(output, pattern);
                Trace.WriteLine(match.Groups[1].Value);

                // return the output
                if (output.Contains("No valid device has been selected."))
                    return "No device found";
                else if (output.Contains("Erasing done."))
                    return "Erasing done.";
                return output;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return $"Error: {ex.Message}";
            }
        }

        /// <summary>
        /// Description: This method sends erase command to JLink.exe via cmd and returns the output as a string so it is possible to present it at GUI 
        /// </summary>
        public string cmd_erase()
        {
            try
            {
                string jLinkExecutable = @"H:\JLink.exe";
                string jLinkCommandFilePath = @"H:\JLinkCommandFile.jlink";

                string newComments="";
                if (DeviceNumber == 1)
                {
                    newComments = "device " + Device + "\n" + "si JTAG\n" + "speed 4000\n" + "jtagconf -1,-1\n" + "connect\n" + "erase\n" + "Exit\n";
                }
                if (DeviceNumber == 2)
                {
                    newComments = "device " + Device + "\n" + "si JTAG\n" + "speed 4000\n" + "jtagconf -1,-1\n" + "connect\n" + "erase\n" + "JTAGConf 9 2" + "\n" + "connect" + "\n" + "erase\n" + "Exit\n";

                }

                if (DeviceNumber == 3)
                {
                    newComments = "device " + Device + "\n" + "si JTAG\n" + "speed 4000\n" + "jtagconf -1,-1\n" + "connect\n" + "erase\n" + "JTAGConf 9 2" + "\n" + "connect" + "\n" + "erase\n"  + "JTAGConf 18 4" + "\n" + "connect" + "\n" + "erase\n"  + "Exit\n";

                }
                if (DeviceNumber == 4)
                {
                    newComments = "device " + Device + "\n" + "si JTAG\n" + "speed 4000\n" + "jtagconf -1,-1\n" + "connect\n" + "erase\n"  + "JTAGConf 9 2" + "\n" + "connect" + "\n" + "erase\n"  + "JTAGConf 18 4" + "\n" + "connect" + "\n" + "erase\n" + "JTAGConf 27 6" + "\n" + "connect" + "\n" + "erase\n" + "Exit\n";

                }
                // Write only the new comments to the file
                File.WriteAllText(jLinkCommandFilePath, newComments);
                // Create process start information
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "cmd.exe";
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = true;
                psi.UseShellExecute = false;

                // Start the process
                Process process = Process.Start(psi);

                // Write the command to cmd
                process.StandardInput.WriteLine($"{jLinkExecutable} -CommandFile {jLinkCommandFilePath}");

                // Close the input stream to indicate that we have finished writing the command
                process.StandardInput.Close();

                // Read the output
                string output = process.StandardOutput.ReadToEnd();

                // Wait for the process to exit
                process.WaitForExit();

                string pattern = @"JTAG chain detection found (\d+) devices:";

                // Match the pattern in the output string
                Match match = Regex.Match(output, pattern);
                int uutAmount = int.Parse(match.Groups[1].Value) / 2;
                Trace.WriteLine(uutAmount);

                // return the output
                if (output.Contains("No valid device has been selected."))
                    return "No device found";
                else if (output.Contains("Erasing done."))
                    return "Erasing done.";

                return output;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return $"Error: {ex.Message}";
            }
        }
    }
}
