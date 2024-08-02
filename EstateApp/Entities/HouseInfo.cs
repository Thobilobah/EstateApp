namespace EstateApp.Entities
{
    public class HouseInfo
    {
        public int id { get; set; }
        public string houseNo { get; set; }
        public string streetName { get; set; }
        public bool isRented { get; set; }
        public int noOfApartment { get; set; }
        public string houseType { get; set; }
        public decimal rentPrice { get; set; }
    }
}
