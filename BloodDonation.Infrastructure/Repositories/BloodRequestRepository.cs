using BloodDonation.Application.Interfaces.Repositories;
using BloodDonation.Domain.Entities;
using BloodDonation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Infrastructure.Repositories
{
    public class BloodRequestRepository :  IBloodRequestRepository
    {
        private readonly AppDbContext _context;
        public BloodRequestRepository(AppDbContext context)  {
            _context = context;
        }
        public async Task<BloodRequest?> GetWithAcceptancesAsync(Guid id)
        {
            return await _context.BloodRequests
                .Include(br => br.Patient)
                .Include(br => br.Hospital)
                .Include(br => br.Acceptances)
                .FirstOrDefaultAsync(br => br.Id == id);
        }
    }
}
