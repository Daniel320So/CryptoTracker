using CryptoTracker.Migrations;
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

        /// <summary>
        /// View a list of Wallet Summary
        /// </summary>
        /// <returns>
        /// View
        /// </returns>
        /// <example>
        /// GET: Wallet/List
        /// </example>
        public ActionResult List()
        {
            //objective: communicate with our wallet data api to retrieve a list of wallets
            //curl https://localhost:44331/api/walletdata/listwallets

            string url = "walletdata/listwallets";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<WalletDto> wallets = response.Content.ReadAsAsync<IEnumerable<WalletDto>>().Result;

            return View(wallets);
        }
        /// <summary>
        /// View details of a wallet
        /// </summary>
        /// <param name="id">Wallet ID</param>
        /// <returns>
        /// View
        /// </returns>
        /// <example>
        /// GET: Wallet/Details/1
        /// </example>
        public ActionResult Details(int id)
        {
            DetailsWallet ViewModel = new DetailsWallet();

            //objective: communicate with our wallet data api to retrieve one wallet
            //curl https://localhost:44324/api/walletdata/findwallet/{id}

            string url = "walletdata/findwallet/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            WalletDto SelectedWallet = response.Content.ReadAsAsync<WalletDto>().Result;

            ViewModel.SelectedWallet = SelectedWallet;

            url = "TokenData/ListTokensForWallet/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<TokenDto> Tokens = response.Content.ReadAsAsync<IEnumerable<TokenDto>>().Result;
            ViewModel.Tokens = Tokens;

            return View(ViewModel);
        }
        /// <summary>
        /// View Page to create a New Wallet
        /// </summary>
        /// <returns>
        /// View
        /// </returns>
        /// <example>
        /// GET: Wallet/New
        /// </example>

        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// Craete a Wallet
        /// </summary>
        /// <returns>
        /// View
        /// </returns>
        /// <example>
        /// POST: Token/Create
        /// </example>
        [HttpPost]
        public ActionResult Create(Wallet Wallet)
        {
            string url = "walletdata/addWallet";

            string jsonpayload = jss.Serialize(Wallet);


            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        /// View the edit page of a Wallet
        /// </summary>
        /// <param name="id">Wallet ID</param>
        /// <returns>
        /// View
        /// </returns>
        /// <example>
        /// GET: Wallet/Edit/1
        /// </example>
        public ActionResult Edit(int id)
        {
            DetailsWallet ViewModel = new DetailsWallet();

            string url = "walletdata/findwallet/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            WalletDto SelectedWallet = response.Content.ReadAsAsync<WalletDto>().Result;
            ViewModel.SelectedWallet = SelectedWallet;

            url = "TokenData/ListTokensForWallet/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<TokenDto> Tokens = response.Content.ReadAsAsync<IEnumerable<TokenDto>>().Result;
            ViewModel.Tokens = Tokens;

            return View(ViewModel);
        }

        /// <summary>
        /// Update a wallet
        /// </summary>
        /// <param name="id">Wallet ID</param>
        /// <returns>
        /// View
        /// </returns>
        /// <example>
        /// POST: Wallet/update/1
        /// </example>
        [HttpPost]
        public ActionResult Update(int id, Wallet wallet)
        {
            string url = "walletdata/updatewallet/" + id;
            string jsonpayload = jss.Serialize(wallet);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// View the delete page of a wallet
        /// </summary>
        /// <param name="id">Wallet ID</param>
        /// <returns>
        /// View
        /// </returns>
        /// <example>
        /// GET: Wallet/DeleteConfirm/1
        /// </example>

        public ActionResult DeleteConfirm(int id)
        {
            string url = "WalletData/findWallet/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            WalletDto selectedWallet = response.Content.ReadAsAsync<WalletDto>().Result;
            return View(selectedWallet);
        }

        /// <summary>
        /// Delete a wallet
        /// </summary>
        /// <param name="id">Wallet ID</param>
        /// <returns>
        /// View
        /// </returns>
        /// <example>
        /// POST: Wallet/Delete/5
        /// </example>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "WalletData/deleteWallet/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Update Token Balance of a wallet
        /// </summary>
        /// <param name="id">Wallet ID</param>
        /// <returns>
        /// View
        /// </returns>
        /// <example>
        /// POST : Wallet/UpdateTokenBalance/1
        /// </example>
        [HttpPost]
        public ActionResult UpdateTokenBalance(int id, TokenDto tokenDto)
        {
            string url = "WalletData/UpdateTokenBalance/" + id;
            string jsonpayload = jss.Serialize(tokenDto);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Edit", new {id = id});
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

    }
}