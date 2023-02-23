using CryptoTracker.Models;
using CryptoTracker.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CryptoTracker.Controllers
{
    public class HomeController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        static HomeController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44331/api/");
        }

        public ActionResult Index()
        {

            Home ViewModel = new Home();

            string url = "TokenData/GetTotalTokenUSDValue";
            HttpResponseMessage response = client.GetAsync(url).Result;

            decimal value = response.Content.ReadAsAsync<decimal>().Result;
            ViewModel.TotalValue = value;
            return View(ViewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}