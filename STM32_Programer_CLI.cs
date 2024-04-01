using System;
using System.Diagnostics;
class STM32_Programer_CLI
{
    //Property
    string STM32_Programer_CLI_Path;
    string Elf_File_To_Flash;


    //Builders
   public STM32_Programer_CLI(string STM32_Programer_CLI_Path)
    {
        this.STM32_Programer_CLI_Path = STM32_Programer_CLI_Path;
       
    }
    public STM32_Programer_CLI(string STM32_Programer_CLI_Path, string Elf_File_To_Flash)
    {
        this.STM32_Programer_CLI_Path = STM32_Programer_CLI_Path;
        this.Elf_File_To_Flash = Elf_File_To_Flash;
    }



    //Methods

    /// <summary>
    /// Description: Program .elf file to the MCU
    /// </summary>
    /// <param name="STM32_Programer_CLI_Path"></param>
    /// <param name="Elf_File_To_Flash"></param>
   public string  Flash_STM32(string STM32_Programer_CLI_Path , string Elf_File_To_Flash)
    {
        // Create a new process start info
        ProcessStartInfo startInfo = new ProcessStartInfo();

        // Set the filename of the executable to run (cmd.exe)
        startInfo.FileName = "cmd.exe";
        string command = $"/C \"{'"' + STM32_Programer_CLI_Path}\" -c port=SWD mode=UR -d \"{Elf_File_To_Flash}"+'"'+" -v -g"+'"';

        // Set any arguments you want to pass to the command line
        startInfo.Arguments = command;

        // Optionally, set working directory
        // startInfo.WorkingDirectory = "C:\\path\\to\\working\\directory";

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
        return (output);
    }

    /// <summary>
    /// Description: Program .elf file to the MCU
    /// </summary>
    public string Flash_STM32()
    {
        // Create a new process start info
        ProcessStartInfo startInfo = new ProcessStartInfo();

        // Set the filename of the executable to run (cmd.exe)
        startInfo.FileName = "cmd.exe";
                                                                                                                                               // -c connect [argument]
        string command = $"/C \"{'"' + this.STM32_Programer_CLI_Path}\" -c port=JTAG mode=UR -d \"{ this.Elf_File_To_Flash}"+'"'+" -v -g"+'"'; // -v is verification
                                                                                                                                               // -g run the code at the spesific adress
                                                                                                                                               // -d download the content of the file into deviuce memory
        // Set any arguments you want to pass to the command line
        startInfo.Arguments = command;

        // Optionally, set working directory
        // startInfo.WorkingDirectory = "C:\\path\\to\\working\\directory";

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
        return(output);
    }

    /// <summary>
    /// Description: Resets the system.
    /// </summary>
    public string Reset_STM32()
    {
 // Create a new process start info
        ProcessStartInfo startInfo = new ProcessStartInfo();

        // Set the filename of the executable to run (cmd.exe)
        startInfo.FileName = "cmd.exe";

        string command = $"/C \"{'"' + this.STM32_Programer_CLI_Path}\" -c port=JTAG mode=UR -Rst" + '"';


        // Set any arguments you want to pass to the command line
        startInfo.Arguments = command;

        // Optionally, set working directory
        // startInfo.WorkingDirectory = "C:\\path\\to\\working\\directory";

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
        return (output);
    }

    /// <summary>
    /// Description: Hardware reset.
    /// </summary>
    public string Hard_Reset_STM32()
    {
 // Create a new process start info
        ProcessStartInfo startInfo = new ProcessStartInfo();

        // Set the filename of the executable to run (cmd.exe)
        startInfo.FileName = "cmd.exe";

        string command = $"/C \"{'"' + this.STM32_Programer_CLI_Path}\" -c port=JTAG mode=UR -HardRst"+'"';


        // Set any arguments you want to pass to the command line
        startInfo.Arguments = command;

        // Optionally, set working directory
        // startInfo.WorkingDirectory = "C:\\path\\to\\working\\directory";

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
        return (output);
    }

    /// <summary>
    /// Description: Executes a Full chip erase operation.
    /// </summary>
    public string Delete_STM32()
    {

        // Create a new process start info
        ProcessStartInfo startInfo = new ProcessStartInfo();

        // Set the filename of the executable to run (cmd.exe)
        startInfo.FileName = "cmd.exe";

        string command = $"/C \"{'"' + this.STM32_Programer_CLI_Path}\" -c port=JTAG mode=UR -E ALL" + '"';


        // Set any arguments you want to pass to the command line
        startInfo.Arguments = command;

        // Optionally, set working directory
        // startInfo.WorkingDirectory = "C:\\path\\to\\working\\directory";

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
        return (output);
    }
    




    //static void Main()
    //{
        
    //    string STM32_Programer_CLI_Path = "H:\\STM32CubeProgrammer\\bin\\STM32_Programmer_CLI.exe";
    //    string Elf_File_To_Flash = "C:\\Users\\tal.gadasi\\OneDrive - F.F3 E.N.C Ltd\\Desktop\\STM32\\led_blink_prj\\led_blink\\Debug\\led_blink.elf";
    //    STM32_Programer_CLI test= new STM32_Programer_CLI (STM32_Programer_CLI_Path, Elf_File_To_Flash);
    //    test.Delete_STM32();
    //    //test.Flash_STM32();
    //}
}