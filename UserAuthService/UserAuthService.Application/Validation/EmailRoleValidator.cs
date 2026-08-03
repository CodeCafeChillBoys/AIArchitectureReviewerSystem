using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAuthService.Application.Validation
{
    public class EmailRoleValidator
    {
        public static bool IsStudentEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string username = email.Split('@')[0];

            return Regex.IsMatch(username, @"^[A-Za-z]+\d+$");
        }
    }
}