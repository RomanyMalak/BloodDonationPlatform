using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.DTOs.BloodRequest
{
    public class CompleteBloodRequestDto
    {
        public Guid Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string BloodType { get; set; } = string.Empty;
        public int UnitsNeeded { get; set; }

        public string Urgency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string HospitalName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}


