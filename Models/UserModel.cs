using System;
using System.ComponentModel.DataAnnotations;

namespace RegAuthApiDemo.Models
{
	public class UserModel
	{
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string? Token { get; set; }


       
        public DateTime DateofBirth { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}

