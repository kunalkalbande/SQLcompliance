﻿using System.Net.Mail;

namespace SQLcomplianceCwfAddin.Helpers
{
    public class EmailHelper
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
