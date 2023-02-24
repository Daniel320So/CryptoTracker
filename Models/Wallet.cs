using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CryptoTracker.Models
{
    public class Wallet
    {
        [Key]
        public int WalletId { get; set; }
        public string WalletName { get; set; }
        public string WalletDescription { get; set; }
        public string WalletType { get; set; }
    }

    public class WalletDto
    {
        public int WalletId { get; set; }
        public string WalletName { get; set; }
        public string WalletDescription { get; set; }
        public string WalletType { get; set; }
        public int WalletBalance { get; set; } //This is refers to a specific token only
        public decimal WalletTotalValue { get; set; } //This is the total usd value of all tokens in the wallet
    }
}