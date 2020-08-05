using SekkaWahda.Models;
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
            var NameWithoutExtension = FileName.Substring(FileName.IndexOf(FileName.First()), FileName.LastIndexOf("."));


            
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
        public static decimal CalcTotalRate(this UserMaster user) 
        {
            using (SECURITY_DBEntities context = new SECURITY_DBEntities()) 
            {
                List<byte?> ratings = context.Ratings.Where(r => r.DriverId == user.UserID).Select(r => r.RateValue).ToList();
                var TotalRate =( (decimal)ratings.Sum<byte?>(l =>l.Value)/ratings.Count);
                TotalRate = Math.Round(TotalRate, 2);
                return TotalRate;

            }
        }

    }
}