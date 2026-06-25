
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BloodDonation.Application.DTOs
{
    public class CompatibleBloodTypesDto
    {
        [JsonPropertyName("compatible_blood_types")]
        public List<string> CompatibleBloodTypes { get; set; } = [];
    }
}
