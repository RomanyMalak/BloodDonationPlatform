using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.DTOs
{
    public class AvailableBloodTypesResponse
    {
        public string PatientBloodType { get; set; }

        public List<string> AvailableBloodTypes { get; set; }
    }
}
