using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RegAuthApiDemo.Domain;

namespace RegAuthApiDemo.Extention
{
	public static class UserManagerExt
	{
		public static async Task<User> FindEmailFromClaimPrinciple(this UserManager<User> record, ClaimsPrincipal  user  )
        {
			var email = user.FindFirstValue(ClaimTypes.Email);

			return await record.Users.SingleOrDefaultAsync(x => x.Email == email);
        }
	}
}

