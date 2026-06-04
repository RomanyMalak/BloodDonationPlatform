using BloodDonation.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.Features.Auth.Commands.Login
{
    public record LoginCommand( string Email,string Password):IRequest<LoginResponseDto>;
    public record LoginResponseDto(Guid UserId, string FullName, string Email, string Role, string Token);
    
}
