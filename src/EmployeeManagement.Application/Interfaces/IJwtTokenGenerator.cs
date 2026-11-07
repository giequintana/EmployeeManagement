using EmployeeManagement.Application.DTOs.Auth;
using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Application.Interfaces;

public interface IJwtTokenGenerator
{
    AccessToken Generate(ApplicationUser user);
}