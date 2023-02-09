using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoTracker.Models
{
    public class WalletxToken
    {
        [Key]
        public int WalletxTokenId { get; set; }
        [ForeignKey("Wallet")]
        public int WalletId { get; set; }
        public virtual Wallet Wallet { get; set; }
        [ForeignKey("Token")]
        public int TokenId { get; set; }
        public virtual Token Token { get; set; }
        public int balance { get; set; }
    }

    public class WalletxTokenDto
    {
        public int WalletxTokenId { get; set; }

        public int WalletId { get; set; }
        public int TokenId { get; set; }

        public decimal balance { get; set; }

    }
}