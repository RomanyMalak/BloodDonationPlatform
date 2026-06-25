using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.DTOs
{
    public class MedicalValidatorResponse
    {
        public List<string> CompatibleBloodTypes { get; set; }
    }
}
