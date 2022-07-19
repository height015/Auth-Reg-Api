using System;
using RegAuthApiDemo.Domain;

namespace RegAuthApiDemo.Service
{
	public interface ITokenService
	{
        string CreateToken(User appUser);
    }
}

