using CryptoTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoTracker.Models.ViewModels
{
    public class DetailsToken
    {

        public TokenDto SelectedToken { get; set; }

        public IEnumerable<WalletDto> Wallets { get; set; }

    }
}