using CryptoTracker.Models;
using CryptoTracker.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CryptoTracker.Controllers
{
    public class WalletController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static WalletController()
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

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// For proper WebAPI authentication, you can send a post request with login credentials to the WebAPI and log the access token from the response. The controller already knows this token, so we're just passing it up the chain.
        /// 
        /// Here is a descriptive article which walks through the process of setting up authorization/authentication directly.
        /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }

        // GET: Wallet/List
        public ActionResult List()
        {
            //objective: communicate with our wallet data api to retrieve a list of wallets
            //curl https://localhost:44331/api/walletdata/listwallets

            string url = "walletdata/listwallets";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<WalletDto> wallets = response.Content.ReadAsAsync<IEnumerable<WalletDto>>().Result;

            return View(wallets);
        }

        public ActionResult Details(int id)
        {
            DetailsWallet ViewModel = new DetailsWallet();

            //objective: communicate with our wallet data api to retrieve one wallet
            //curl https://localhost:44324/api/walletdata/findwallet/{id}

            string url = "walletdata/findwallet/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            WalletDto SelectedWallet = response.Content.ReadAsAsync<WalletDto>().Result;

            ViewModel.SelectedWallet = SelectedWallet;

            url = "WalletData/ListTokensForWallet/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<TokenDto> Tokens = response.Content.ReadAsAsync<IEnumerable<TokenDto>>().Result;

            ViewModel.Tokens = Tokens;

            Console.WriteLine(Tokens);
            return View(ViewModel);
        }

    }
}