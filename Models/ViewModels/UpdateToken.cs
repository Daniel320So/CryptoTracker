using CryptoTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoTracker.Models.ViewModels
{
    public class UpdateToken
    {

        public TokenDto SelectedToken { get; set; }

        public IEnumerable<string> TokenRiskLevels { get; set; }

        public UpdateToken() {
            List<string> TokenRiskLevelsList = new List<string>();
            TokenRiskLevelsList.Add("Low");
            TokenRiskLevelsList.Add("Medium");
            TokenRiskLevelsList.Add("High");
            TokenRiskLevels = TokenRiskLevelsList;
        }

    }
}