using BloodDonation.Application.DTOs.Ocr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.Features.BloodRequests.Commands.VerifyBloodRequestDocument
{
    public sealed record VerifyBloodRequestDocumentCommand ( Guid BloodRequestId) : IRequest<OcrResultDto>;
}
