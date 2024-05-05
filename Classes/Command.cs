using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI_REAL.Classes
{
    struct Command
    {

        public string Model { get; set; }
        public string Name { get; set; }
        public string SCPI_Command { get; set; }

        //Methods
        public Command(string model, string name, string SCPI_command)
        {
            Model = model;
            Name = name;
            SCPI_Command = SCPI_command;
        }
        public Command(string command)
        {

            SCPI_Command = command;
        }

        public Command(Command other)
        {
            Model = other.Model;
            Name = other.Name;
            SCPI_Command = other.SCPI_Command;
        }


    }
}
