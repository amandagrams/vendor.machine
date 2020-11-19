using System;
using System.Collections.Generic;
using System.Linq;
using static VendorMachine.Domain.Constants;

namespace VendorMachine.Domain
{
    public class Machine
    {
        #region properties
        
        private Dictionary<string, string> _productsSettings;
        List<string> _coinSettings;
        Dictionary<string, string> _unitSettings;
        public List<Product> MachineProducts = new List<Product>();        
        public List<Coin> MachineCoins = new List<Coin>();
        public List<Coin> CustomerCoins = new List<Coin>();
        public decimal CustomerAmount = 0;
        public List<Coin> AcceptedCoins = new List<Coin>();
        public List<Product> AcceptedProducts = new List<Product>();
        public string Output = string.Empty;
        
        #endregion properties

        public Machine(Dictionary<string, string> productsSettings, List<string> coinSettings, Dictionary<string, string> unitSettings)
        {
            _productsSettings = productsSettings;
            _coinSettings = coinSettings;
            _unitSettings = unitSettings;

            SetAvailabeProducts();
            SetAvailableCoins();
            SetAcceptedCoins();
            SetAcceptedProducts();
        }

        #region Process Machine
        /// <summary>
        /// Process user requests 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string Process(string input) 
        {
           Output = string.Empty;
           var userRequest = GetUserRequestFormatting(input);            

            if (!userRequest.IsValid)
                return "INVALID_INPUT =" + CustomerAmount;

            ProcessCustomerCoins(userRequest.Coins);

            if (!HaveProductEnough(userRequest.Products))
                return "NO_PRODUCT =" + CustomerAmount;

            if (!(CustomerAmount >= userRequest.Amount) || !HaveCoinsToChange(CustomerAmount - userRequest.Amount))
                return "NO_COINS =" + CustomerAmount;

            if (!userRequest.Products.Any())
                Output += CustomerAmount.ToString();

            ComputeCoins(ref CustomerCoins, ref MachineCoins, CustomerAmount);
            ComputeCoins(ref MachineCoins, ref CustomerCoins, (CustomerAmount - userRequest.Amount));

            DeliverProduct(userRequest.Products);

            if (userRequest.ChangeRequested)
                ProcessChange();             
           
            return Output.TrimEnd();
        }

        #endregion Process Machine

        #region Vendor Machine Methods
               
        /// <summary>
        /// Process the customer coins insert
        /// </summary>
        /// <param name="coins"></param>
        public void ProcessCustomerCoins(List<Coin> coins)
        {
            CustomerCoins.Clear();
            if (coins.Count <= 0)
                return;

            coins.ForEach(
                Coin =>
                {
                    CustomerCoins.Add(Coin);
                });

            CustomerAmount = CustomerCoins.Where(c => c.Value > 0).Sum(x => x.Value);
        }
        
        /// <summary>
        /// Check if have product enough
        /// </summary>
        /// <param name="productRequestByUser"></param>
        /// <returns></returns>
        public bool HaveProductEnough(List<Product> productRequestByUser) 
        {
            var machineProducts = new List<Product>(MachineProducts);
            
            foreach (var product in productRequestByUser)
            {
                //var count = machineProducts.Count(p => p.Name == product.Name);
                //if (count <= 0)
                //    return false;
               
                if (!(machineProducts.Remove(machineProducts.FirstOrDefault(p => p.Name == product.Name))))
                    return false;
            }
            return true;
        }
        
        /// <summary>
        /// Verify if have coin enough to change
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private bool HaveCoinsToChange(decimal value)
        {
            var haveCoinsToChange = false;

            var machineCoinsAux = MachineCoins;
            foreach (var coinAccepted in AcceptedCoins.OrderByDescending(a => a.Value))
            {                
                while (value >= coinAccepted.Value && (machineCoinsAux.FirstOrDefault(c => c.Value == coinAccepted.Value) != null))
                {
                    value -= coinAccepted.Value;
                    machineCoinsAux.Remove(coinAccepted);
                }
            }
            if (value == 0.00m)
                haveCoinsToChange = true;

            return haveCoinsToChange;
        }

        /// <summary>
        /// Compute the coins
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <param name="value"></param>
        private void ComputeCoins(ref List<Coin> origin, ref List<Coin> destination, decimal value)
        {
            foreach (var acceptedCoin in AcceptedCoins.OrderByDescending(a => a.Value))
            {
                while (value >= acceptedCoin.Value && origin.FirstOrDefault(x => x.Value == acceptedCoin.Value) != null)
                {
                    value -= acceptedCoin.Value;
                    origin.Remove(origin.FirstOrDefault(c => c.Value == acceptedCoin.Value));
                    destination.Add(acceptedCoin);
                }
            }
        }

        /// <summary>
        /// Deliver the product
        /// </summary>
        /// <param name="products"></param>
        private void DeliverProduct(List<Product> products)
        {
            foreach (var product in products)
            {
                MachineProducts.Remove(MachineProducts.FirstOrDefault(p => p.Name == product.Name));
                CustomerAmount -= product.Price;
                Output += product.Name + " =" + CustomerAmount + " ";
            }
        }

        /// <summary>
        /// Process the change
        /// </summary>
        private void ProcessChange()
        {
            if (CustomerAmount == 0.00m)
            {
                Output += "NO_CHANGE";
                return;
            }

            foreach (var coin in CustomerCoins)
            {
                Output += coin.Value.ToString() + " ";
            }
            CustomerCoins.Clear();
        }

        /// <summary>
        /// Format the user request input 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private UserRequest GetUserRequestFormatting(string input)
        {
            var userRequest = new UserRequest();

            foreach (var item in input.Split(" "))
            {
                switch (item)
                {
                    case "1.00":
                        userRequest.Coins.Add(new Coin(1.00m));
                        break;

                    case "0.50":
                        userRequest.Coins.Add(new Coin(0.50m));
                        break;

                    case "0.25":
                        userRequest.Coins.Add(new Coin(0.25m));
                        break;

                    case "0.10":
                        userRequest.Coins.Add(new Coin(0.10m));
                        break;

                    case "0.05":
                        userRequest.Coins.Add(new Coin(0.05m));
                        break;

                    case "0.01":
                        userRequest.Coins.Add(new Coin(0.01m));
                        break;

                    case "Coke":
                        userRequest.Products.Add(AcceptedProducts.FirstOrDefault(x => x.Name == "Coke"));
                        break;

                    case "Water":
                        userRequest.Products.Add(AcceptedProducts.FirstOrDefault(x => x.Name == "Water"));
                        break;

                    case "Pastelina":
                        userRequest.Products.Add(AcceptedProducts.FirstOrDefault(x => x.Name == "Pastelina"));
                        break;

                    case "CHANGE":
                        userRequest.ChangeRequested = true;
                        break;
                }
            }

            return userRequest;
        }

        #endregion Vendor Machine Methods

        #region Product Settings
        private List<Product> SetAvailabeProducts()
        {
            var getProducts = _productsSettings;
            if (getProducts.Count <= 0)
                return MachineProducts;

            var unit = int.Parse(_unitSettings.FirstOrDefault(x => x.Key == Settings.ProductUnit).Value);
            foreach (var product in getProducts)
            {
                for (int i = 0; i < unit; i++)
                {
                    MachineProducts.Add(new Product(product.Key, Decimal.Parse(product.Value)));
                }
            }

            return MachineProducts;
        }

        /// <summary>
        /// Set accepted products
        /// </summary>
        private void SetAcceptedProducts()
        {
            if (_productsSettings.Count <= 0)
                return;

            foreach (var product in _productsSettings)
            {
                AcceptedProducts.Add(new Product(product.Key, Decimal.Parse(product.Value)));
            }
        }

        #endregion Product Settings

        #region Coin Settings
        private List<Coin> SetAvailableCoins()
        {
            var getCoins = _coinSettings;
            if (getCoins.Count <= 0)
                return MachineCoins = new List<Coin>();

            var unit = int.Parse(_unitSettings.FirstOrDefault(x => x.Key == Settings.CoinUnit).Value);

            foreach (var coin in getCoins)
            {
                for (int i = 0; i < unit; i++)
                {
                    MachineCoins.Add(new Coin(Decimal.Parse(coin)));
                }
            }

            return MachineCoins;
        }        

        /// <summary>
        /// Set the accepted coins based on appsettings file
        /// </summary>
        private void SetAcceptedCoins()
        {
            foreach (var coin in _coinSettings)
            {
                AcceptedCoins.Add(new Coin(Decimal.Parse(coin)));
            }
        }
        
        #endregion Coin Settings
    }
}
