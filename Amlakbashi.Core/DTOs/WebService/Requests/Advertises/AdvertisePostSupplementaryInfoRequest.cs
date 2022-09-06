using Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Advertises
{
    public class AdvertisePostSupplementaryInfoRequest
    {
        [Range(1, int.MaxValue)]
        public long residenceId { get; set; }
        public Advertise.HeatingSystemItems heatingSystem { get; set; }
        public Advertise.CoolingSystemItems coolingSystem { get; set; }
        public Advertise.WCItems wc { get; set; }
        public bool elevator { get; set; }
        public bool pool { get; set; }
        public bool poolHotWater { get; set; }
        public bool poolFiltration { get; set; }
        public bool poolOpen { get; set; }
        public bool poolCovered { get; set; }
        public bool sauna { get; set; }
        public bool jacuzzi { get; set; }
        public bool bathroom { get; set; }
        public bool wifi { get; set; }
        public bool washingMachine { get; set; }
        public bool microwaveOven { get; set; }
        public bool soundSystem { get; set; }
        public bool golf { get; set; }
        public bool poolTable { get; set; }
        public bool foosball { get; set; }
        public bool hairdryer { get; set; }
        public bool tv { get; set; }
        public bool oven { get; set; }
        public bool refrigerator { get; set; }
        public bool kitchenHood { get; set; }
        public bool kitchenUtensils { get; set; }
        public bool teaMaker { get; set; }
        public bool party { get; set; }
        public bool pets { get; set; }
        public bool smoking { get; set; }
        public string requiredEvidence { get; set; }
        public string otherRules { get; set; }
        public Advertise.OwnershipTypeEnum ownershipType { get; set; }
        public string ownerPhoneNumber { get; set; }
        public string ownerFullName { get; set; }
        public bool license { get; set; }
        public string licenseNumber { get; set; }

        [BindNever]
        public int userId { get; set; }

        public bool IsValid(ModelStateDictionary modelState)
        {
            if (heatingSystem == Advertise.HeatingSystemItems.Unset ||
                Enum.IsDefined(typeof(Advertise.HeatingSystemItems), heatingSystem) == false)
            {
                modelState.AddModelError(nameof(heatingSystem), "value is incorrect");
            }
            if (coolingSystem == Advertise.CoolingSystemItems.Unset ||
                Enum.IsDefined(typeof(Advertise.CoolingSystemItems), coolingSystem) == false)
            {
                modelState.AddModelError(nameof(coolingSystem), "value is incorrect");
            }
            if (wc == Advertise.WCItems.Unset ||
                Enum.IsDefined(typeof(Advertise.WCItems), wc) == false)
            {
                modelState.AddModelError(nameof(wc), "value is incorrect");
            }
            if (ownershipType == Advertise.OwnershipTypeEnum.Intermediary)
            {
                if (string.IsNullOrEmpty(ownerPhoneNumber))
                {
                    modelState.AddModelError(nameof(ownerPhoneNumber), "value is incorrect");
                }
                if (string.IsNullOrEmpty(ownerFullName))
                {
                    modelState.AddModelError(nameof(ownerFullName), "value is incorrect");
                }
            }
            if (license)
            {
                if (string.IsNullOrEmpty(licenseNumber))
                {
                    modelState.AddModelError(nameof(licenseNumber), "value is incorrect");
                }
            }
            return modelState.IsValid;
        }
    }
}
