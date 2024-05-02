using System;
using System.Diagnostics;

class STLink
{
    // Properties
    string STM32_Programer_CLI_Path; // Path to "STM32_Programmer_CLI.exe"
    string Elf_File_To_Flash;        // Path to "program_file.hex"

    // Constructors
    public STLink(string STM32_Programer_CLI_Path)
    {
        this.STM32_Programer_CLI_Path = STM32_Programer_CLI_Path;
    }

    public STLink(string STM32_Programer_CLI_Path, string Elf_File_To_Flash)
    {
        this.STM32_Programer_CLI_Path = STM32_Programer_CLI_Path;
        this.Elf_File_To_Flash = Elf_File_To_Flash;
    }

    // Methods

    /// <summary>
    /// Description: Program .elf/.hex file to the MCU.
    /// </summary>
    /// <param name="STM32_Programer_CLI_Path"></param>
    /// <param name="Elf_File_To_Flash"></param>
    /// <returns></returns>
    public string STLink_Program_STM32(string STM32_Programer_CLI_Path, string Elf_File_To_Flash)
    {
        try
        {
            // Create a new process start info
            ProcessStartInfo startInfo = new ProcessStartInfo();

            // Set the filename of the executable to run (cmd.exe)
            startInfo.FileName = "cmd.exe";

            // Construct the command to flash STM32
            string command = $"/C \"{'"' + STM32_Programer_CLI_Path}\" -c port=SWD mode=UR -d \"{Elf_File_To_Flash}\" -v -g\"";

            // Set the command as arguments
            startInfo.Arguments = command;

            // Set to redirect standard output
            startInfo.RedirectStandardOutput = true;

            // Set to use shell execute
            startInfo.UseShellExecute = false;

            // Create and start the process
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            // Read the output of the command
            string output = process.StandardOutput.ReadToEnd();

            // Wait for the process to finish
            process.WaitForExit();

            // Output the result
            return output;
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Description: Program .elf file to the MCU using stored path and file.
    /// </summary>
    public string STLink_Program_STM32()
    {
        return STLink_Program_STM32(this.STM32_Programer_CLI_Path, this.Elf_File_To_Flash);
    }

    /// <summary>
    /// Description: Resets the MCU via ST-Link.
    /// </summary>
    public string STLink_Reset_STM32()
    {
        try
        {
            // Create a new process start info
            ProcessStartInfo startInfo = new ProcessStartInfo();

            // Set the filename of the executable to run (cmd.exe)
            startInfo.FileName = "cmd.exe";

            // Construct the command to reset STM32
            string command = $"/C \"{'"' + this.STM32_Programer_CLI_Path}\" -c port=JTAG mode=UR -Rst\"";

            // Set the command as arguments
            startInfo.Arguments = command;

            // Set to redirect standard output
            startInfo.RedirectStandardOutput = true;

            // Set to use shell execute
            startInfo.UseShellExecute = false;

            // Create and start the process
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            // Read the output of the command
            string output = process.StandardOutput.ReadToEnd();

            // Wait for the process to finish
            process.WaitForExit();
           
            // Output the result
            return output;
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Description: Hardware reset via ST-Link.
    /// </summary>
    public string STLink_Hard_Reset_STM32()
    {
        try
        {
            // Create a new process start info
            ProcessStartInfo startInfo = new ProcessStartInfo();

            // Set the filename of the executable to run (cmd.exe)
            startInfo.FileName = "cmd.exe";

            // Construct the command to perform a hard reset on STM32
            string command = $"/C \"{'"' + this.STM32_Programer_CLI_Path}\" -c port=JTAG mode=UR -HardRst\"";

            // Set the command as arguments
            startInfo.Arguments = command;

            // Set to redirect standard output
            startInfo.RedirectStandardOutput = true;

            // Set to use shell execute
            startInfo.UseShellExecute = false;

            // Create and start the process
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            // Read the output of the command
            string output = process.StandardOutput.ReadToEnd();

            // Wait for the process to finish
            process.WaitForExit();

            // Output the result
            return output;
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Description: Executes a Full chip erase operation via ST-Link.
    /// </summary>
    public string STLink_Erase_STM32()
    {
        try
        {
            // Create a new process start info
            ProcessStartInfo startInfo = new ProcessStartInfo();

            // Set the filename of the executable to run (cmd.exe)
            startInfo.FileName = "cmd.exe";

            // Construct the command to delete STM32 content
            string command = $"/C \"{'"' + this.STM32_Programer_CLI_Path}\" -c port=JTAG mode=UR -E ALL\"";

            // Set the command as arguments
            startInfo.Arguments = command;

            // Set to redirect standard output
            startInfo.RedirectStandardOutput = true;

            // Set to use shell execute
            startInfo.UseShellExecute = false;

            // Create and start the process
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            // Read the output of the command
            string output = process.StandardOutput.ReadToEnd();

            // Wait for the process to finish
            process.WaitForExit();

            // Output the result
            return output;
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}
