using System.Collections.Generic;
using System.Linq;
using VendorMachine.Domain;

namespace VendorMachine
{
    public class UserRequest
    {
        public UserRequest()
        {
            Products = new List<Product>();
            Coins = new List<Coin>();
        }
        public List<Product> Products { get; set; }

        public List<Coin> Coins { get; set; }

        public bool ChangeRequested { get; set; }

        public bool IsValid => Coins.Count > 0 && Products.Count > 0 ? true : false;

        public decimal Amount => Products.Where(c => c.Price > 0).Sum(x => x.Price);      

    }
}
