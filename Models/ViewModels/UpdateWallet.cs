using CryptoTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoTracker.Models.ViewModels
{
    public class UpdateWallet
    {

        public WalletDto SelectedWallet { get; set; }

        public IEnumerable<string> WalletTypes { get; set; }

        public UpdateWallet()
        {

            List<string> WalletTypesList = new List<string>();
            WalletTypesList.Add("Centralised Exchange Platform");
            WalletTypesList.Add("Decentralised Hot Wallet");
            WalletTypesList.Add("Decentralised Cold Wallet");
            WalletTypes = WalletTypesList;
        }

    }
}