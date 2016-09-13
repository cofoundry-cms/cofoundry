using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class UserContextMapper
    {
        private readonly IMapper _mapper;
        private readonly IUserAreaRepository _userAreaRepository;

        public UserContextMapper(
            IMapper mapper,
            IUserAreaRepository userAreaRepository
            )
        {
            _mapper = mapper;
            _userAreaRepository = userAreaRepository;
        }

        public UserContext Map(User dbUser)
        {
            if (dbUser == null) return null;

            var cx = _mapper.Map<UserContext>(dbUser);
            cx.UserArea = _userAreaRepository.GetByCode(dbUser.UserAreaCode);

            return cx;
        }
    }
}
