using BloodDonation.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.DTOs
{
    public class AvailableDonorDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string BloodType { get; set; } = string.Empty;

        public string? Phone { get; set; }
    }
}
