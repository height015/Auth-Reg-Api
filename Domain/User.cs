using System;
using Microsoft.AspNetCore.Identity;

namespace RegAuthApiDemo.Domain
{
	public class User : IdentityUser
	{
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime Dateofbirth { get; set; }

        public string State { get; set; }

        public string Country { get; set; }


    }
}

