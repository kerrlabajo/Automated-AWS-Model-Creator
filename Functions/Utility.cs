using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace LSC_Trainer.Functions
{
    /// <summary>
    /// Utility class for managing weight files and retrieving values from text boxes.
    /// </summary>
    public class Utility
    {
        private Dictionary<string, string> sizeToWeightMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="Utility"/> class.
        /// </summary>      
        public Utility()
        {
            sizeToWeightMap = new Dictionary<string, string>
            {
                { "640", "yolov5s.pt" },
                { "1280", "yolov5n6.pt" }
                // more image sizes
            };
        }

        /// <summary>
        /// Gets the weight file corresponding to the given image size.
        /// </summary>
        /// <param name="imageSize">The size of the image.</param>
        /// <returns>The weight file name.</returns>
        public string GetWeightFile(string imageSize)
        {
            if (sizeToWeightMap.TryGetValue(imageSize, out string weightFile))
            {
                return weightFile;
            }

            // Default value in the case where the size is not found
            return "640";
        }

        /// <summary>
        /// Retrieves the value from the specified text box.
        /// </summary>
        /// <param name="textBox">The text box from which to retrieve the value.</param>
        /// <returns>The value from the text box, or an empty string if the text box is null or empty.</returns>
        public string GetValueFromTextBox(TextBox textBox)
        {
            return !string.IsNullOrWhiteSpace(textBox.Text) ? textBox.Text : "";
        }
    }
}
