using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CryptoTracker.Models;

namespace CryptoTracker.Controllers
{
    public class WalletDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/WalletData/ListWallets
        [HttpGet]
        public IQueryable<Wallet> ListWallets()
        {
            return db.Wallets;
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

            return Ok(wallet);
        }

        // PUT: api/WalletData/AddWallet/5
        [HttpPost]
        [ResponseType(typeof(void))]
        public IHttpActionResult AddWallet(int id, Wallet wallet)
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

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/WalletData/UpdateWallet
        [HttpPost]
        [ResponseType(typeof(Wallet))]
        public IHttpActionResult UpdateWallet(Wallet wallet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Wallets.Add(wallet);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = wallet.WalletId }, wallet);
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
    }
}