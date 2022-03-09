using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Mediator.Commands.FileCommands
{
    public class UpdateAdvertiseLicenseFileCommand : IRequest<long>
    {
        public long AdvertiseId { get; set; }
        public int UserId { get; set; }
        public IFormFile NewLicenseFile { get; set; }
        public long? LicenseFileId { get; set; }
        public UpdateAdvertiseLicenseFileCommand(IFormFile newLicenseFile, long advertiseId, int userId, long? licenseFileId)
        {
            this.NewLicenseFile = newLicenseFile;
            this.AdvertiseId = advertiseId;
            this.UserId = userId;
            this.LicenseFileId = licenseFileId;
        }
    }
}
