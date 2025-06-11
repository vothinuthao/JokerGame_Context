using System;
using System.Globalization;
using UnityEngine;

namespace Core.Utils
{
    public class PlayerUtils
    {
        public static string CreatShortUID()
        {
            // Generate a new GUID
            Guid guid = Guid.NewGuid();

            // Convert the GUID to a base64-encoded string
            string base64Guid = Convert.ToBase64String(guid.ToByteArray());

            // Replace characters that are not URL-safe
            base64Guid = base64Guid.Replace("+", "-").Replace("/", "_");

            // Take the first 8 characters as the short UID
            string shortUID = base64Guid.Substring(0, 8);

            return shortUID;
        }

        public static string UtcDayNow()
        {
            DateTime utcNow = DateTime.UtcNow;

            return utcNow.ToString(CultureInfo.InvariantCulture);
        }
    
        
    }
}