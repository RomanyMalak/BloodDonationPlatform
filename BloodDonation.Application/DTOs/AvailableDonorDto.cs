using BloodDonation.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.DTOs
{
    public record AvailableDonorDto(
    Guid Id,
    string FullName,
    BloodType BloodType
);
}
