namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs
{
    public class ReservePopupDTO
    {
        public int Capacity { get; set; }
        public int MoreThanCapacity { get; set; }
        public DiscountDTO DiscountData { get; set; }

        public ReservePopupDTO(int capacity, int moreThanCapacity, DiscountDTO discountData)
        {
            Capacity = capacity;
            MoreThanCapacity = moreThanCapacity;
            DiscountData = discountData;
        }
    }
}
