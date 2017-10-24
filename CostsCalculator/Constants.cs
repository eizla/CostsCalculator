using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostsCalculator
{
    public static class Constants
    {
        public static string ApplicationURL = @"https://costscalculatorbackend.azurewebsites.net";
        public static string ApplicationID = @"3db41980-2b33-4f8b-b178-5906708484cd";
        public static string commonAuthority = "https://login.windows.net/common";


        public static string[] Scopes = { ApplicationID };
        public static string SignUpSignInPolicy = "B2C_1_Email";
        public static string Authority = "https://login.microsoftonline.com/AGHIO2017.onmicrosoft.com/";
        public static string ResetPasswordPolicy = "B2C_1_Przypomnienie";
        public static string EditingPolicy = "B2C_1_Editing";
    }
}
