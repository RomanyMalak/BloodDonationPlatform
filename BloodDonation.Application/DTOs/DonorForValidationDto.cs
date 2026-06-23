using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.DTOs
{
    public class DonorForValidationDto
    {
        public Guid Id { get; set; }
        public string BloodType { get; set; } = string.Empty;
    }
}
