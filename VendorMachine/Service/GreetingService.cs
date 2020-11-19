using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using VendorMachine.Domain;
using static VendorMachine.Domain.Constants;

namespace VendorMachine
{
    public class GreetingService : IGreetingService
    {
        #region properties
        private readonly IConfiguration _configuration;
        public static Dictionary<string, string> _productsSettings { get; private set; }       
        public static List<string> _coinSettings { get; private set; }
        public static Dictionary<string, string> _unitSettings { get; private set; }
        #endregion properties

        /// <summary>
        /// Initialize service with DI to enable logging and configuration
        /// </summary>
        /// <param name="_log">ILogger instance</param>
        /// <param name="_configuration">IConfiguration instance</param>
        public GreetingService(IConfiguration configuration)
        {
            _configuration = configuration;
            _productsSettings = _configuration.GetSection(Settings.ProductSettings).GetChildren().ToDictionary(x => x.Key, x => x.Value);            
            _coinSettings = _configuration.GetSection(Settings.CoinSettings).GetSection(Settings.Coins).Get<List<string>>();
            _unitSettings = _configuration.GetSection(Settings.UnitSettings).GetChildren().ToDictionary(x => x.Key, x => x.Value);
        }
        public void Run()
        {
            var vendorMachine = new Machine(_productsSettings, _coinSettings, _unitSettings);

            while (true) 
            {                
                Console.Write("Input: ");
                Console.WriteLine("Output: " + vendorMachine.Process(Console.ReadLine()));
            }
        }
    }
}
