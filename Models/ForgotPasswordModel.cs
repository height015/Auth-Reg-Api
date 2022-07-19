using System;
using System.ComponentModel.DataAnnotations;

namespace RegAuthApiDemo.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

