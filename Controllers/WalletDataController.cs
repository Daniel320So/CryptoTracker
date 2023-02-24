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
using Antlr.Runtime;
using CryptoTracker.Migrations;
using CryptoTracker.Models;
using Microsoft.AspNet.Identity;

namespace CryptoTracker.Controllers
{
    public class WalletDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all Wallet data
        /// </summary>
        /// <returns>
        /// IEnumerable<WalletDto>
        /// </returns>
        /// <example>
        /// GET: api/WalletData/ListWallets
        /// </example>
        [HttpGet]
        public IEnumerable<WalletDto> ListWallets()
        {
            List<Wallet> Wallets = db.Wallets.ToList();

            List<WalletDto> WalletDtos = new List<WalletDto>();

            Wallets.ForEach(w => {
                WalletDtos.Add(new WalletDto()
                {
                    WalletId = w.WalletId,
                    WalletName = w.WalletName,
                    WalletDescription = w.WalletDescription,
                    WalletType = w.WalletType,
                    WalletTotalValue = this.GetWalletTotalValue(w.WalletId)
                });
            });

            return WalletDtos;
        }

        /// <summary>
        /// Returns data of a Wallet
        /// </summary>
        /// <param name="id"> Wallet Id</param>
        /// <returns>
        /// WalletDto
        /// </returns>
        /// <example>
        /// GET: api/WalletData/FindWallet/5
        /// </example>
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
                WalletType = wallet.WalletType,
                WalletTotalValue = this.GetWalletTotalValue(id)
            };

            return Ok(walletDto);
        }

        /// <summary>
        /// add a new Wallet
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Wallet in the database
        /// </returns>
        /// <example>
        /// POST: api/WalletData/AddWallet
        /// </example>

        [HttpPost]
        [ResponseType(typeof(Wallet))]
        public IHttpActionResult AddWallet(Wallet wallet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Wallets.Add(wallet);
            db.SaveChanges();
            //add one of each Token at 0 qty
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
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = wallet.WalletId }, wallet);

        }

        /// <summary>
        ///update data of a Wallet
        /// </summary>
        /// <param name="id"> Wallet Id</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: TokeWalletns in the database
        /// </returns>
        /// <example>
        /// POST: api/WalletData/UpdateWallet/5
        /// </example>
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

        /// <summary>
        /// Delete data of a Wallet
        /// </summary>
        /// <param name="id"> Wallet Id</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Wallet in the database
        /// </returns>
        /// <example>
        /// POST: api/WalletData/DeleteWallet/5
        /// </example>

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
        /// Gathers information about all Wallets related to a particular Token ID
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Wallets in the database
        /// </returns>
        /// <param name="id">Token ID.</param>
        /// <example>
        /// GET: api/TokenData/ListWalletsForToken/3
        /// </example>
        [HttpGet]
        [ResponseType(typeof(WalletxTokenDto))]
        public IHttpActionResult ListWalletsForToken(int id)
        {
            List<WalletxToken> WxTs = db.WalletxTokens.Where(wxt => wxt.TokenId == id).Include(wxt => wxt.Wallet).ToList();

            List<WalletDto> WalletDtos = new List<WalletDto>();

            WxTs.ForEach(wxt => WalletDtos.Add(new WalletDto()
            {
                WalletId = wxt.Wallet.WalletId,
                WalletName = wxt.Wallet.WalletName,
                WalletDescription = wxt.Wallet.WalletDescription,
                WalletType = wxt.Wallet.WalletType,
                WalletBalance = wxt.balance
            }));

            return Ok(WalletDtos);
        }


        /// <summary>
        /// Update balance in WalletxTokens with a specific WalletId
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// StatusCode(HttpStatusCode.NoContent)
        /// </returns>
        /// <param name="id">Wallet ID.</param>
        /// <example>
        /// POST: api/WalletData/UpdateTokenBalance/3
        /// </example>
        [HttpPost]
        public IHttpActionResult UpdateTokenBalance(int id, TokenDto tokenDto)
        {
            WalletxToken WxT= db.WalletxTokens.Where(wxt => wxt.WalletId == id && wxt.TokenId == tokenDto.TokenId).Include(wxt => wxt.Token).ToList()[0];
            WxT.balance = tokenDto.TokenBalance;
            db.Entry(WxT).State = EntityState.Modified;

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

        /// <summary>
        /// Get total value of a wallete
        /// </summary>
        /// <returns>
        /// Total value in USD
        /// </returns>
        /// <param name="id">Wallet ID.</param>
        /// <example>
        /// POST: api/WalletData/GetWalletTotalValue/3
        /// </example>
        public decimal GetWalletTotalValue(int id)
        {
            List<Token> Tokens = db.Tokens.ToList();
            List<WalletxToken> Wxts = db.WalletxTokens.ToList();

            decimal value = 0;
            List<WalletxToken> WxtsByWalletId = Wxts.FindAll(wxt => wxt.WalletId == id);
            Tokens.ForEach(t =>
            {
                decimal tokenAmount = 0;
                List<WalletxToken> WxtsByTokenId = WxtsByWalletId.FindAll(wxt => wxt.TokenId == t.TokenId);
                WxtsByTokenId.ForEach(wxt =>
                {
                    tokenAmount += wxt.balance;
                });
                value += tokenAmount * t.TokenPrice;
            });

            return value;
        }
    }
}