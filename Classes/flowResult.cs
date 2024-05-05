using System;
using System.Diagnostics;
using System.Windows;

namespace GUI_REAL.Classes
{
    internal struct flowResult
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string AcceptedValue { get; set; }
        public string divP { get; set; }
        public string divN { get; set; }

        public flowResult(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public flowResult(string type, string value, string AValue, string divp, string divn)
        {
            Type = type;
            Value = value;
            divP = divp;
            divN = divn;
            AcceptedValue = AValue;
        }
        public flowResult()
        {

        }

        public void deleteResult()
        {
            Type = null;
            Value = null;
            divP = null;
            divN = null;
            AcceptedValue = null;
        }
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
                    // If parsing fails, return false
                    MessageBox.Show("Please enter number");
                    return "Error";
                }

                // Calculate the modified divisor values
                divisorPositive = divisorPositive;
                divisorNegative = divisorNegative;
                Trace.WriteLine("lowest current highest result");
                // Check if sampleValue falls within the range defined by the modified divisors
                if (divisorPositive < sampleValue || divisorNegative > sampleValue)
                    return divisorNegative.ToString() + ":" + sampleValue.ToString() + ":" + divisorPositive.ToString() + ":" + "Fail";

                else
                {
                    return divisorNegative.ToString() + ":" + sampleValue.ToString() + ":" + divisorPositive.ToString() + ":" + "Pass";
                }

            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the execution
                MessageBox.Show("An error occurred:AA " + ex.Message);
                return "Error"; // Or handle the error in another appropriate way
            }
        }
    }
}
