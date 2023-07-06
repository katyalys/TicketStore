using Microsoft.AspNetCore.Mvc;
using Order.Application.Dtos;
using System.Security.Claims;

namespace Order.WebApi.Controllers
{
    public class BaseController : ControllerBase
    {
        private UserDto _userDto;

        protected UserDto User
        {
            get
            {
                if (_userDto == null)
                {
                    _userDto = new UserDto
                    {
                        Id = base.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    };
                }

                return _userDto;
            }
        }
    }
}
