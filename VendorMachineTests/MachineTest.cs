using System.Collections.Generic;
using VendorMachine.Domain;
using Xunit;
using FluentAssertions;

namespace VendorMachineTests
{
    public class MachineTest
    {

        private Dictionary<string, string> productsSettings = new Dictionary<string, string>() { { "Coke","1.50" },{ "Water","1.00"},{ "Pastelina","0.30"} };
        private List<string> coinSettings = new List<string> { "0.01","0.05","0.10","0.25","0.50","1.00"};
        private Dictionary<string, string> unitSettings = new Dictionary<string, string>() { { "ProductUnit","10" },{ "CoinUnit", "10" } };


        [Fact]
        public void Should_MachineProcess_SellWithoutChange()
        {
            var input = new Machine(productsSettings, coinSettings, unitSettings);
            var result = input.Process("0.50 1.00 Coke");

            result.Should().Be("Coke =0.00");           
        }

        [Fact]
        public void Should_MachineProcess_dReturnNoChange()
        {
            var input = new Machine(productsSettings, coinSettings, unitSettings);
            var result = input.Process("0.25 0.05 Pastelina CHANGE");

            result.Should().Be("Pastelina =0.00 NO_CHANGE");
        }

        [Fact]
        public void Should_MachineProcess_SellMoreThanOneProductOnInput()
        {
            var input = new Machine(productsSettings, coinSettings, unitSettings);
            var result = input.Process("1.00 Pastelina Pastelina Pastelina");

            result.Should().Be("Pastelina =0.70 Pastelina =0.40 Pastelina =0.10");
        }

        [Fact]
        public void Should_MachineProcess_NotAcceptJustCoinInput()
        {
            var input = new Machine(productsSettings, coinSettings, unitSettings);
            var result = input.Process("0.50");

            result.Should().Be("INVALID_INPUT =0");
        }

        [Fact]
        public void Should_MachineProcess_NotAcceptJustChangeInput()
        {
            var input = new Machine(productsSettings, coinSettings, unitSettings);
            var result = input.Process("CHANGE");

            result.Should().Be("INVALID_INPUT =0");
        }

        [Fact]
        public void Should_MachineProcess_NotAcceptProductInputNotFromMachine()
        {
            var input = new Machine(productsSettings, coinSettings, unitSettings);
            var result = input.Process("0.25 Mentos");

            result.Should().Be("INVALID_INPUT =0");
        }

        [Fact]
        public void Should_MachineProcess_NotSellProductsThatAreOutOfStock()
        {
            var input = new Machine(productsSettings, coinSettings, unitSettings);
            for (int i = 0; i < 10; i++)
            {
                input.Process("1.00 1.00 Coke");
            }            
            var result = input.Process("1.00 1.00 Coke");

            result.Should().Be("NO_PRODUCT =2.00");
        }

        [Fact]
        public void Should_MachineProcess_NotSellProductWhenTheDepositedBalanceIsLessThanTheValueOfTheProducts()
        {
            var input = new Machine(productsSettings, coinSettings, unitSettings);
            var result = input.Process("1.00 0.50 Coke Coke Coke Coke");

            result.Should().Be("NO_COINS =1.50");
        }
        
        [Fact]
        public void Should_MachineProcess_NotSellMoreProductsThatHave()
        {
            var input = new Machine(productsSettings, coinSettings, unitSettings);
            var result = input.Process("1.00 1.00 1.00 1.00 1.00 1.00 1.00 1.00 1.00 1.00 1.00 1.00 Water Water Water Water Water Water Water Water Water Water Water");

            result.Should().Be("NO_PRODUCT =12.00");
        }
    }
}
