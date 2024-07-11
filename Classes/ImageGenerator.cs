using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI_REAL.Classes
{
    struct ImageGenerator
    {
        /// <summary>
        /// Decodes the binary block data from a text file.
        /// </summary>
        /// <param name="filePath">The path to the text file containing the binary block data.</param>
        /// <returns>A byte array representing the decoded binary data.</returns>
        public static byte[] DecodeBinaryBlockData(byte [] data)
        {
            // Read the binary block data from the text file


            // Split the text into individual integer values


            // Convert the string values to bytes
            byte[] binaryData = data;
           

            // Check if the data starts with the PNG file signature
            if (binaryData.Length < 8 ||
                binaryData[0] != 137 || binaryData[1] != 80 || binaryData[2] != 78 || binaryData[3] != 71 ||
                binaryData[4] != 13 || binaryData[5] != 10 || binaryData[6] != 26 || binaryData[7] != 10)
            {
                throw new ArgumentException("Invalid PNG file signature");
            }

            return binaryData;
        }

        /// <summary>
        /// Saves the image data to a file in the specified directory.
        /// </summary>
        /// <param name="imageData">The image data to be saved.</param>
        /// <param name="directoryPath">The path to the directory where the image file will be saved.</param>
        /// <param name="fileName">The name of the image file, including the extension.</param>
        public static void SaveImageDataToFile(byte[] imageData, string directoryPath, string fileName)
        {
            // Save the image data to the specified directory
            string filePath = Path.Combine(directoryPath, fileName);
            File.WriteAllBytes(filePath, imageData);
            Console.WriteLine("Image saved to: " + filePath);
        }

        /// <summary>
        /// Gets the file extension from a file name.
        /// </summary>
        /// <param name="fileName">The file name, including the extension.</param>
        /// <returns>The file extension, without the leading dot.</returns>
        public static string GetFileExtension(string fileName)
        {
            return Path.GetExtension(fileName).TrimStart('.');
        }

        /// <summary>
        /// Checks if a file extension is a valid image extension.
        /// </summary>
        /// <param name="extension">The file extension to check.</param>
        /// <returns>True if the extension is a valid image extension, false otherwise.</returns>
        public static bool IsValidImageExtension(string extension)
        {
            string[] validExtensions = { "png", "jpg", "jpeg", "bmp", "gif" };
            return validExtensions.Contains(extension.ToLower());
        }

        /// <summary>
        /// Checks if a file path is valid and accessible.
        /// </summary>
        /// <param name="filePath">The file path to check.</param>
        /// <returns>True if the file path is valid and accessible, false otherwise.</returns>
        public static bool IsValidFilePath(string filePath)
        {
            try
            {
                using (FileStream fs = File.OpenRead(filePath))
                {
                    // The file is accessible, so the path is valid
                    return true;
                }
            }
            catch
            {
                // The file is not accessible, so the path is invalid
                return false;
            }
        }

    }
}
