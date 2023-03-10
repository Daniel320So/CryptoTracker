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
    public class TokenController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static TokenController()
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
        /// View a list of Token Summary
        /// </summary>
        /// <returns>
        /// View
        /// </returns>
        /// <example>
        /// GET: Token/List
        /// </example>

        public ActionResult List()
        {
            //objective: communicate with our token data api to retrieve a list of token
            //curl https://localhost:44331/api/tokendata/listtokens

            string url = "tokendata/listtokens";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<TokenDto> tokens = response.Content.ReadAsAsync<IEnumerable<TokenDto>>().Result;

            return View(tokens);
        }

        /// <summary>
        /// View details of a token
        /// </summary>
        /// <param name="id">Token ID</param>
        /// <returns>
        /// View
        /// </returns>
        /// <example>
        /// GET: Token/Details/1
        /// </example>

        public ActionResult Details(int id)
        {
            DetailsToken ViewModel = new DetailsToken();

            //objective: communicate with our token data api to retrieve one token
            //curl https://localhost:44324/api/tokendata/findtoken/{id}

            string url = "tokendata/findtoken/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            TokenDto SelectedToken = response.Content.ReadAsAsync<TokenDto>().Result;

            ViewModel.SelectedToken = SelectedToken;

            url = "WalletData/ListWalletsForToken/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<WalletDto> Wallets = response.Content.ReadAsAsync<IEnumerable<WalletDto>>().Result;

            ViewModel.Wallets = Wallets;

            return View(ViewModel);
        }

        /// <summary>
        /// View Page to create a New Token
        /// </summary>
        /// <returns>
        /// View
        /// </returns>
        /// <example>
        /// GET: Token/New
        /// </example>

        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// Craete a New Token
        /// </summary>
        /// <returns>
        /// View
        /// </returns>
        /// <example>
        /// POST: Token/Create
        /// </example>

        [HttpPost]
        public ActionResult Create(Token token)
        {
            string url = "tokendata/addToken";

            string jsonpayload = jss.Serialize(token);

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
        /// View the edit page of a token
        /// </summary>
        /// <param name="id">Token ID</param>
        /// <returns>
        /// View
        /// </returns>
        /// <example>
        /// GET: Token/Edit/1
        /// </example>
        public ActionResult Edit(int id)
        {
            UpdateToken ViewModel = new UpdateToken();

            string url = "tokendata/findtoken/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            TokenDto SelectedToken = response.Content.ReadAsAsync<TokenDto>().Result;
            ViewModel.SelectedToken = SelectedToken;

            return View(ViewModel);
        }

        /// <summary>
        /// Update a token
        /// </summary>
        /// <param name="id">Token ID</param>
        /// <returns>
        /// View
        /// </returns>
        /// <example>
        /// POST: Token/update/1
        /// </example>

        [HttpPost]
        public ActionResult Update(int id, Token token)
        {
            string url = "tokendata/updatetoken/" + id;
            string jsonpayload = jss.Serialize(token);
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
        /// View the delete page of a token
        /// </summary>
        /// <param name="id">Token ID</param>
        /// <returns>
        /// View
        /// </returns>
        /// <example>
        /// GET: Token/DeleteConfirm/1
        /// </example>
        public ActionResult DeleteConfirm(int id)
        {
            string url = "TokenData/findToken/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            TokenDto selectedToken = response.Content.ReadAsAsync<TokenDto>().Result;
            return View(selectedToken);
        }

        /// <summary>
        /// Delete a token
        /// </summary>
        /// <param name="id">Token ID</param>
        /// <returns>
        /// View
        /// </returns>
        /// <example>
        /// POST: Token/Delete/5
        /// </example>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "TokenData/deleteToken/" + id;
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
        /// <param name="id">Token ID</param>
        /// <returns>
        /// View
        /// </returns>
        /// <example>
        /// POST : Token/UpdateTokenBalance/1
        /// </example>

        [HttpPost]
        public ActionResult UpdateTokenBalance(int id, WalletDto walletDto)
        {
            string url = "TokenData/UpdateTokenBalance/" + id;
            string jsonpayload = jss.Serialize(walletDto);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Edit", new { id = id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

    }
}