using System;
using System.ComponentModel.DataAnnotations;

namespace RegAuthApiDemo.Models
{
	public class ConfirmEmail
	{
        [Required]
        public string Token { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}

