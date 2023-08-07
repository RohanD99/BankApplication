﻿using System;
using static BankApplication.Common.Enums;

namespace BankApplication.Models
{
    public class User
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public UserType Type { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

    }
}
