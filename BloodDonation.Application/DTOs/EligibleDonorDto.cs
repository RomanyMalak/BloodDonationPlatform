using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.DTOs
{
    public class EligibleDonorDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string BloodType { get; set; } = string.Empty;
        public DateTime? LastDonationDate { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Phone { get; set; }
    }
}
