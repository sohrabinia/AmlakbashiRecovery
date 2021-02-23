using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.FilterHelpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.Infrastructure.FilterHelpers
{
    public class AdvertiseFilterHelper : IAdvertiseFilterHelper
    {
        public IQueryable<Advertise> FilterEmptyInRange(IQueryable<Advertise> input, List<DateTime> range)
        {
            var test = input.ToList();
            input = input.Where(w => w.OccupiedTables.Any(a =>
                range.Contains(a.Date)) == false);
            test = input.ToList();
            return input;
        }

        public IQueryable<Advertise> FilterParking(IQueryable<Advertise> input,
            string parking, bool hasParking)
        {
            if (parking != null)
            {
                int parking_int = 0;
                if (int.TryParse(parking, out parking_int) && parking_int > -1)
                {
                    input = input.Where(a => a.Parking == (ParkingItems)parking_int);
                }
            }
            if (hasParking)
            {
                input = input.Where(a => a.Parking > 0 && a.Parking != ParkingItems.NoParking);
            }
            return input;
        }

        public IQueryable<Advertise> FilterPhrase(IQueryable<Advertise> input, string phrase)
        {
            if (string.IsNullOrEmpty(phrase))
                return input;
            var containDirection = phrase.Contains("غرب") ||
                phrase.Contains("شرق") ||
                phrase.Contains("شمال") ||
                phrase.Contains("جنوب");
            var search_list = phrase.Split(' ').ToList();
            if (search_list.Contains("تهران"))
            {
                if (search_list.Contains("غرب"))
                {
                    search_list.Add("غرب تهران");
                }
                if (search_list.Contains("شرق"))
                {
                    search_list.Add("شرق تهران");
                }
                if (search_list.Contains("شمال"))
                {
                    search_list.Add("شمال تهران");
                }
                if (search_list.Contains("جنوب"))
                {
                    search_list.Add("جنوب تهران");
                }
            }
            foreach (var str in search_list)
            {
                var str_to_search = str;
                str_to_search = str_to_search.Replace("ي", "ی");
                var first_is_alef = str.FirstOrDefault() == 'ا';
                if (first_is_alef)
                {
                    var kolah = "آ" + str.Remove(0, 1);
                    if (containDirection)
                    {
                        input = input.Where(w =>
                            (w.Title != null && w.Title.Contains(str)) ||
                            (w.LocationString != null && w.LocationString.Contains(str)) ||
                            (w.Title != null && w.Title.Contains(kolah)) ||
                            (w.LocationString != null && w.LocationString.Contains(kolah)));
                    }
                    else
                    {
                        input = input.Where(w =>
                            (w.Title != null && w.Title.Contains(str)) ||
                            (w.Address != null && w.Address.Contains(str)) ||
                            (w.LocationString != null && w.LocationString.Contains(str)) ||
                            (w.Title != null && w.Title.Contains(kolah)) ||
                            (w.Address != null && w.Address.Contains(kolah)) ||
                            (w.LocationString != null && w.LocationString.Contains(kolah)));
                    }
                }
                else
                {
                    if (containDirection)
                    {
                        if (str == "سوئیت" || str == "سوییت")
                        {
                            input = input.Where(w =>
                                (w.Title != null && w.Title.Contains("سوئیت")) ||
                                (w.Title != null && w.Title.Contains("سوییت")) ||
                                (w.LocationString != null && w.LocationString.Contains(str)));
                        }
                        else
                        {
                            input = input.Where(w =>
                                (w.Title != null && w.Title.Contains(str)) ||
                                (w.LocationString != null && w.LocationString.Contains(str)));
                        }
                    }
                    else
                    {
                        if (str == "سوئیت" || str == "سوییت")
                        {
                            input = input.Where(w =>
                                (w.Title != null && w.Title.Contains("سوئیت")) ||
                                (w.Title != null && w.Title.Contains("سوییت")) ||
                                (w.Address != null && w.Address.Contains(str)) ||
                                (w.LocationString != null && w.LocationString.Contains(str)));
                        }
                        else
                        {
                            input = input.Where(w =>
                                (w.Title != null && w.Title != null && w.Title.Contains(str)) ||
                                (w.Address != null && w.Address.Contains(str)) ||
                                (w.LocationString != null && w.LocationString.Contains(str)));
                        }
                    }
                }
            }
            return input;
        }

        public IQueryable<Advertise> FilterPrice(IQueryable<Advertise> input,
            priceRangeTypes priceRangeType, int frompaypernight, int topaypernight)
        {
            if (frompaypernight > 0)
            {
                var filtered_ids = new List<long>();
                long relatedPrice;
                foreach (var item in input)
                {
                    if (item.Childs.Any())
                    {
                        switch (priceRangeType)
                        {
                            case priceRangeTypes.Holiday:
                                relatedPrice = item.Childs.Min(x => x.HolidayPrice);
                                break;
                            case priceRangeTypes.HolidayPeak:
                                relatedPrice = item.Childs.Min(x => x.HolidayPikePrice);
                                break;
                            case priceRangeTypes.Monthly:
                                relatedPrice = item.Childs.Min(x => x.RentPrice);
                                break;
                            default:
                                relatedPrice = item.Childs.Min(x => x.DailyPrice);
                                break;
                        }
                    }
                    else
                    {
                        switch (priceRangeType)
                        {
                            case priceRangeTypes.Holiday:
                                relatedPrice = item.HolidayPrice;
                                break;
                            case priceRangeTypes.HolidayPeak:
                                relatedPrice = item.HolidayPikePrice;
                                break;
                            case priceRangeTypes.Monthly:
                                relatedPrice = item.RentPrice;
                                break;
                            default:
                                relatedPrice = item.DailyPrice;
                                break;
                        }
                    }
                    if (relatedPrice >= frompaypernight)
                    {
                        filtered_ids.Add(item.Id);
                    }
                }
                input = input.Where(x => filtered_ids.Contains(x.Id));
            }
            if (topaypernight > 0)
            {
                var filtered_ids = new List<long>();
                long relatedPrice;
                foreach (var item in input)
                {
                    if (item.Childs.Any())
                    {
                        switch (priceRangeType)
                        {
                            case priceRangeTypes.Holiday:
                                relatedPrice = item.Childs.Min(x => x.HolidayPrice);
                                break;
                            case priceRangeTypes.HolidayPeak:
                                relatedPrice = item.Childs.Min(x => x.HolidayPikePrice);
                                break;
                            case priceRangeTypes.Monthly:
                                relatedPrice = item.Childs.Min(x => x.RentPrice);
                                break;
                            default:
                                relatedPrice = item.Childs.Min(x => x.DailyPrice);
                                break;
                        }
                    }
                    else
                    {
                        switch (priceRangeType)
                        {
                            case priceRangeTypes.Holiday:
                                relatedPrice = item.HolidayPrice;
                                break;
                            case priceRangeTypes.HolidayPeak:
                                relatedPrice = item.HolidayPikePrice;
                                break;
                            case priceRangeTypes.Monthly:
                                relatedPrice = item.RentPrice;
                                break;
                            default:
                                relatedPrice = item.DailyPrice;
                                break;
                        }
                    }
                    if (relatedPrice > 0 && relatedPrice <= topaypernight)
                    {
                        filtered_ids.Add(item.Id);
                    }
                }
                input = input.Where(x => filtered_ids.Contains(x.Id));
            }
            return input;
        }

        public IQueryable<Advertise> FilterRoom(IQueryable<Advertise> input, string room, List<int> roomList)
        {
            if (room != null && room != "-1")
            {
                int room_int = 0;
                if (int.TryParse(room, out room_int))
                {
                    if (room_int < 5)
                    {
                        input = input.Where(a => a.Room == room_int);
                    }
                    else
                    {
                        input = input.Where(a => a.Room >= room_int);
                    }
                }
            }
            if (roomList != null)
            {
                if (roomList.Contains(5))
                {
                    input = input.Where(a =>
                        a.Room > 4 ||
                        roomList.Contains(a.Room));
                }
                else
                {
                    input = input.Where(a => roomList.Contains(a.Room));
                }
            }
            return input;
        }
    }
}
