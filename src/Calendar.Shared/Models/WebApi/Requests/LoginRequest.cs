﻿using System.ComponentModel.DataAnnotations;

namespace Calendar.Shared.Models.WebApi.Requests
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
