using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.Features.Auth.Commands.Register
{
    public record RegisterHospitalCommand(
     string Email,
     string Password,
     string HospitalName,
     string Government,
     string City,
     string AddressDetail,
     double Latitude,
     double Longitude,
     string Hotline,
    IFormFile LicenseDocumentPath
 ) : IRequest<Guid>;
}
