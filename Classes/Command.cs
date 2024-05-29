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

        /// <summary>
         /// Initializes a new instance of the <see cref="Command"/> struct.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="name"></param>
        /// <param name="SCPI_command"></param>
        public Command(string model, string name, string SCPI_command)
        {
            Model = model;
            Name = name;
            SCPI_Command = SCPI_command;
        }

        /// <summary>
        /// Build object but only the SCPI command (for send command from flow)
        /// </summary>
        /// <param name="command"></param>
        public Command(string command)
        {

            SCPI_Command = command;
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public Command(Command other)
        {
            Model = other.Model;
            Name = other.Name;
            SCPI_Command = other.SCPI_Command;
        }


    }
}
