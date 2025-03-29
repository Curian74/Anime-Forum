
using AutoMapper;
using Domain.Common.Roles;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace Application.Services
{
    public class RoleService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public RoleService(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<IdentityRole<Guid>>> GetAllRolesAsync()
        {
            return _roleManager.Roles.ToList();
        }

        public async Task<List<string>> GetUserRolesAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return new List<string>();

            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }
    }
}
