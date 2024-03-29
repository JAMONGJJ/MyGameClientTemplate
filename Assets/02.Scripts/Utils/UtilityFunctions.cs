using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientTemplate
{
    namespace UtilityFunctions
    {
        public enum SizeType
        {
            bytes,
            Kbs,
            Mbs,
            Gbs,
        
        }
        
        public static class Utility
        {
            public static UtilityFunctions Functions { get; } = new UtilityFunctions();
        }

        public class UtilityFunctions
        {
            public string ConvertByteLongToString(long size)
            {
                float tmpSize = size;
                SizeType type = SizeType.bytes;
                while (tmpSize > 1024.0f)
                {
                    tmpSize /= 1024.0f;
                    type++;
                }

                return $"{tmpSize:F2}{type}";
            }
        }
    }
}
