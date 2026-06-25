using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.Interfaces
{
    public interface IMedicalValidatorAgent
    {
        Task<List<string>> GetCompatibleBloodTypesAsync(
    string patientBloodType,
    List<string> availableBloodTypes,
    CancellationToken cancellationToken);
    }
}
