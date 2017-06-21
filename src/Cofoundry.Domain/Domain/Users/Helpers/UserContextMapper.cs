using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class UserContextMapper
    {
        private readonly IUserAreaRepository _userAreaRepository;

        public UserContextMapper(
            IUserAreaRepository userAreaRepository
            )
        {
            _userAreaRepository = userAreaRepository;
        }

        public UserContext Map(User dbUser)
        {
            if (dbUser == null) return null;

            var cx = new UserContext();

            cx.IsPasswordChangeRequired = dbUser.RequirePasswordChange;
            cx.RoleId = dbUser.RoleId;
            cx.UserId = dbUser.UserId;
            cx.UserArea = _userAreaRepository.GetByCode(dbUser.UserAreaCode);

            return cx;
        }
    }
}
