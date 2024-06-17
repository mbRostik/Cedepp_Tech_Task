using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cedepp.Application.Contracts.DTOs
{
    public class ChangeUserProfileDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string CodiceFiscale { get; set; }

        public string CAP { get; set; }

        public DateOnly DayOfBirth { get; set; }

        public string Workplace { get; set; }
    }
}
