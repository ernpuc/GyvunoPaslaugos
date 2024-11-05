﻿using Microsoft.AspNetCore.Identity;

namespace PetServiceWebApplication.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
    }
}
