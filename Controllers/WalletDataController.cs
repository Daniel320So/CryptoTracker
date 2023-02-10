using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Web.Http;
using System.Web.Http.Description;
using CryptoTracker.Migrations;
using CryptoTracker.Models;
using Microsoft.AspNet.Identity;

namespace CryptoTracker.Controllers
{
    public class WalletDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/WalletData/ListWallets
        [HttpGet]
        public IEnumerable<WalletDto> ListWallets()
        {
            List<Wallet> Wallets = db.Wallets.ToList();
            List<WalletDto> WalletDtos = new List<WalletDto>();

            Wallets.ForEach(w => WalletDtos.Add(new WalletDto()
            {
                WalletId = w.WalletId,
                WalletName = w.WalletName,
                WalletDescription = w.WalletDescription,
                WalletType = w.WalletType
            }));

            return WalletDtos;
        }

        // GET: api/WalletData/FindWallet/5
        [HttpGet]
        [ResponseType(typeof(Wallet))]
        public IHttpActionResult FindWallet(int id)
        {
            Wallet wallet = db.Wallets.Find(id);
            if (wallet == null)
            {
                return NotFound();
            }
            WalletDto walletDto = new WalletDto()
            {
                WalletId = wallet.WalletId,
                WalletName = wallet.WalletName,
                WalletDescription = wallet.WalletDescription,
                WalletType = wallet.WalletType
            };


            return Ok(walletDto);
        }

        // PUT: api/WalletData/AddWallet
        [HttpPost]
        [ResponseType(typeof(Wallet))]
        public IHttpActionResult AddWallet(Wallet wallet)
        {
            Debug.WriteLine("Add Wallet");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Debug.WriteLine("Testing");

            db.Wallets.Add(wallet);
            db.SaveChanges();
            Debug.WriteLine("Testing");
            //add one of each ticket at 0 qty
            List<Token> Tokens = db.Tokens.ToList();
            Tokens.ForEach(t =>
                db.WalletxTokens.Add(
                    new WalletxToken
                    {
                        WalletId = wallet.WalletId,
                        TokenId = t.TokenId,
                        balance = 0
                    }
                )
            ); ;
            Debug.WriteLine("Testing");
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = wallet.WalletId }, wallet);

        }

        // POST: api/WalletData/UpdateWallet
        [HttpPost]
        [ResponseType(typeof(Wallet))]
        public IHttpActionResult UpdateWallet(int id, Wallet wallet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != wallet.WalletId)
            {

                return BadRequest();
            }

            db.Entry(wallet).State = EntityState.Modified;


            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WalletExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/WalletData/DeleteWallet/5
        [HttpPost]
        [ResponseType(typeof(Wallet))]
        public IHttpActionResult DeleteWallet(int id)
        {
            Wallet wallet = db.Wallets.Find(id);
            if (wallet == null)
            {
                return NotFound();
            }

            db.Wallets.Remove(wallet);
            db.SaveChanges();

            return Ok(wallet);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WalletExists(int id)
        {
            return db.Wallets.Count(e => e.WalletId == id) > 0;
        }

        /// <summary>
        /// Gathers information about all Tokens related to a particular Wallet ID
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Tokens in the database
        /// </returns>
        /// <param name="id">Wallet ID.</param>
        /// <example>
        /// GET: api/WalletData/ListTokensForWallet/3
        /// </example>
        [HttpGet]
        [ResponseType(typeof(TokenDto))]
        public IHttpActionResult ListTokensForWallet(int id)
        {
            List<WalletxToken> WxTs = db.WalletxTokens.Where(wxt => wxt.WalletId == id).Include(wxt => wxt.Token).ToList();

            List<TokenDto> TokenDtos = new List<TokenDto>();
            WxTs.ForEach(wxt => TokenDtos.Add(new TokenDto()
            {
                TokenId = wxt.Token.TokenId,
                TokenName = wxt.Token.TokenName,
                TokenDescription = wxt.Token.TokenDescription,
                TokenRiskLevel = wxt.Token.TokenRiskLevel,
                TokenBalance = wxt.balance
            }));

            return Ok(TokenDtos);
        }
    }
}