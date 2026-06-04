using PlataformaAutogestion.Application.Interfaces;
using PlataformaAutogestion.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserService _userService;

        public UserService(IUserService userService)
        {
            _userService = userService;
        }

        public List<UserDto> GetAll()
        {
            return UserDto.CreateList(userList);
        }

        public UserDto GetById(int id)
        {
            return UserDto.CreateList(userList);
        }
    }
}
