using Application.DTO;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Validations
{

    public class UserEditFieldValidations
    {

        private readonly HashSet<string> allowedFieldsForUser = new HashSet<string> { "Bio", "PhoneNumber", "Password" };
        private readonly HashSet<string> allowedFieldsForAdmin = new HashSet<string> { "Status", "Role" };

        public bool IsAllowed(string field,string role)
        {
            Console.WriteLine("==================================="); 
            Console.WriteLine(field + " " +  role); 
            if (role.Equals("Member") && allowedFieldsForUser.Contains(field))
            {
                return true;
            }
            else if (role.Equals("Admin") && allowedFieldsForAdmin.Contains(field))
            {
                return true;
            }
            return false;

        }

    }
}
