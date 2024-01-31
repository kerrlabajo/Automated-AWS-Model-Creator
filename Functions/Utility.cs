using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC_Trainer.Functions
{
    public class ImageSizeWeightMapping
    {
        private Dictionary<string, string> sizeToWeightMap;

        public ImageSizeWeightMapping()
        {
            sizeToWeightMap = new Dictionary<string, string>
        {
            { "640", "yolov5s.pt" },
            { "1280", "yolov5n6.pt" }
            // more image sizes
        };
        }

        public string GetWeightFile(string imageSize)
        {
            if (sizeToWeightMap.TryGetValue(imageSize, out string weightFile))
            {
                return weightFile;
            }

            // Default value in the case where the size is not found
            return "640";
        }
    }
}
