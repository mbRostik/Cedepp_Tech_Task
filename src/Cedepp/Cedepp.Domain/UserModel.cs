﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cedepp.Domain
{
    public class UserModel
    {
        public string Id { get; set; }

        public byte[]? Photo { get; set; }
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string CodiceFiscale { get; set; }

        public string CAP { get; set; }

        public DateOnly DayOfBirth { get; set; }

        public string Workplace { get; set; }
    }
}
