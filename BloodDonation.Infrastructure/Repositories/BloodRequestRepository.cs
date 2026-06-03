using BloodDonation.Application.Interfaces.Repositories;
using BloodDonation.Domain.Entities;
using BloodDonation.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Infrastructure.Repositories
{
    public class BloodRequestRepository : Repository<BloodRequest>, IBloodRequestRepository
    {
        public BloodRequestRepository(AppDbContext context) : base(context) { }
       
    }
}
