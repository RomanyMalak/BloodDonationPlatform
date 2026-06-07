using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BloodDonation.Domain.Entities;

namespace BloodDonation.Application.Features.Auth.Commands.Register
{
    
    public record RegisterUserCommand(
      string FullName,
    string Email,
    string Password,
    string? Phone,
    int? Age,
    BloodType? BloodType,
    UserRole Role,
    double Latitude,
    double Longitude) : IRequest<RegisterResponseDto>;
    public record RegisterResponseDto(Guid UserId, string FullName, string Token);

   
}
