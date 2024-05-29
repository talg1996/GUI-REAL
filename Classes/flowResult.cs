using System;
using System.Diagnostics;
using System.Windows;

namespace GUI_REAL.Classes
{
    /// <summary>
    /// Test the result of a flow operation.
    /// </summary>
    internal struct flowResult
    {
        /// <summary>
        /// Gets or sets the type of the result.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the value of the result.
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Gets or sets the accepted value for comparison.
        /// </summary>
        public string? AcceptedValue { get; set; }

        /// <summary>
        /// Gets or sets the positive divisor value for range checking.
        /// </summary>
        public string? divP { get; set; }

        /// <summary>
        /// Gets or sets the negative divisor value for range checking.
        /// </summary>
        public string? divN { get; set; }


        // Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="flowResult"/> struct with type and value.
        /// </summary>
        /// <param name="type">The type of the result.</param>
        /// <param name="value">The value of the result.</param>
        public flowResult(string type, string value)
        {
            Type = type;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="flowResult"/> struct with all values.
        /// </summary>
        /// <param name="type">The type of the result.</param>
        /// <param name="value">The value of the result.</param>
        /// <param name="AValue">The accepted value for comparison.</param>
        /// <param name="divp">The positive divisor value for range checking.</param>
        /// <param name="divn">The negative divisor value for range checking.</param>
        public flowResult(string type, string value, string AValue, string divp, string divn)
        {
            Type = type;
            Value = value;
            divP = divp;
            divN = divn;
            AcceptedValue = AValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="flowResult"/> struct.
        /// </summary>
        public flowResult()
        {

        }


        // Methods

        /// <summary>
        /// Deletes the result by setting all properties to null.
        /// </summary>
        public void deleteResult()
        {
            Type = null;
            Value = null;
            divP = null;
            divN = null;
            AcceptedValue = null;
        }

        /// <summary>
        /// Checks if the sample value is within the range of divisorPositive and divisorNegative.
        /// </summary>
        /// <returns>A string indicating pass or fail.</returns>
        public string isItPass()
        {
            try
            {
                // Convert string inputs to float
                float divisorPositive;
                float divisorNegative;
                float acceptedValue;
                float sampleValue;

                if (!float.TryParse(divP, out divisorPositive) ||
                    !float.TryParse(divN, out divisorNegative) ||
                    !float.TryParse(AcceptedValue, out acceptedValue) ||
                    !float.TryParse(Value, out sampleValue))
                {
                    // If parsing fails, return error
                    MessageBox.Show("Please enter numbers");
                    return "Error";
                }

                // Check if sampleValue falls within the range defined by the divisors
                if (divisorPositive < sampleValue || divisorNegative > sampleValue)
                    return "Fail";
                else
                    return "Pass";
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during execution
                MessageBox.Show("An error occurred: " + ex.Message);
                return "Error"; // Or handle the error in another appropriate way
            }
        }
    }
}
