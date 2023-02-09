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
        public int TokenID { get; set; }
        public string TokenName { get; set; }
        public string TokenDescription { get; set; }
        public string TokenRiskLevel { get; set; }

    }
}