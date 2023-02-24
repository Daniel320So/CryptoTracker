using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CryptoTracker.Models
{
    public class Token
    {
        [Key]
        public int TokenId { get; set; }
        public string TokenName { get; set; }
        public string TokenDescription { get; set; }
        public string TokenRiskLevel { get; set; }
        public string TokenSymbol { get; set; }
        public decimal TokenPrice { get; set; }

    }

    public class TokenDto
    {
        public int TokenId { get; set; }
        public string TokenName { get; set; }
        public string TokenDescription { get; set; }
        public string TokenRiskLevel { get; set; }
        public int TokenBalance { get; set; }
        public string TokenSymbol { get; set; }
        public decimal TokenPrice { get; set; }
        public decimal TokenValue { get; set; }
    }
}