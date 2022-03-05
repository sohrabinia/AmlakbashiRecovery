using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Mediator.Commands.FileCommands
{
    public class UpdateAdvertiseLicenseFileCommand : IRequest
    {
        public IFormFile LicenseFile { get; set; }
        public long AdvertiseId { get; set; }
        public int UserId { get; set; }
        public long? OldLicenseFileId { get; set; }
        public UpdateAdvertiseLicenseFileCommand(IFormFile licenseFile, long advertiseId, int userId, long? oldLicenseFileId)
        {
            this.LicenseFile = licenseFile;
            this.AdvertiseId = advertiseId;
            this.UserId = userId;
            this.OldLicenseFileId = oldLicenseFileId;
        }
    }
}
