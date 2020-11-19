namespace VendorMachine.Domain
{
    public class Coin
    {
        public decimal Value { get; set; }       
        public Coin(decimal value)
        {
            Value = value;
        }
    }
}
