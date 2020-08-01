using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SekkaWahda.ExtensionMethods
{
    public static class Extensions
    {
        public static string NoRepeateInFileName(this string FileName,string UserName) 
        {


            FileName = FileName.Trim('"');

            var Extension = FileName.Substring(FileName.LastIndexOf("."));
            var NameWithoutExtension = FileName.Substring(0, FileName.LastIndexOf("."));


            
            NameWithoutExtension = NameWithoutExtension + UserName + DateTime.Now;
            List<string> charToRemove = new List<string>()
                    {"/",":"

                    };
            foreach (var c in charToRemove)
            {
                NameWithoutExtension = NameWithoutExtension.Replace(c, string.Empty);

            }

            FileName = NameWithoutExtension + Extension;
            FileName = String.Concat(FileName.Where(c => !char.IsWhiteSpace(c)));
            return FileName;

        }

    }
}