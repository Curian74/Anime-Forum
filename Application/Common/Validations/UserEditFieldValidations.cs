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

        public bool IsAllowed(UpdateUserDto targetUser,string role, Guid currentUserId)
        {
            if (role.Equals("Member") && allowedFieldsForUser.Contains(targetUser.field) && currentUserId == targetUser.userId)
            {
                return true;
            }
            else if (role.Equals("Admin") && allowedFieldsForAdmin.Contains(targetUser.field))
            {
                return true;
            }
            return false;

        }

    }
}
