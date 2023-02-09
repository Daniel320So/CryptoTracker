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
    public class TokenDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/TokenData/ListTokens
        [HttpGet]
        public IEnumerable<TokenDto> ListTokens()
        {
            List<Token> Tokens = db.Tokens.ToList();
            List<TokenDto> TokenDtos = new List<TokenDto>();

            Tokens.ForEach(t => TokenDtos.Add(new TokenDto()
            {
                TokenId = t.TokenId,
                TokenDescription = t.TokenDescription,
                TokenName = t.TokenName,
                TokenRiskLevel = t.TokenRiskLevel,

            }));

            return TokenDtos;

        }

        // GET: api/TokenData/FindTokens/5
        [HttpGet]
        [ResponseType(typeof(Token))]
        public IHttpActionResult FindToken(int id)
        {
            Token token = db.Tokens.Find(id);
            TokenDto tokenDto = new TokenDto()
            {
                TokenId = token.TokenId,
                TokenDescription = token.TokenDescription,
                TokenName = token.TokenName,
                TokenRiskLevel = token.TokenRiskLevel,
            };
            if (token == null)
            {
                return NotFound();
            }

            return Ok(tokenDto);
        }

        // PUT: api/TokenData/AddToken/5
        [HttpPost]
        [ResponseType(typeof(void))]
        public IHttpActionResult AddToken(int id, Token token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != token.TokenId)
            {
                return BadRequest();
            }

            db.Entry(token).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TokenExists(id))
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

        // POST: api/TokenData/UpdateToken
        [HttpPost]
        [ResponseType(typeof(Token))]
        public IHttpActionResult UpdateToken(Token token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Tokens.Add(token);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = token.TokenId }, token);
        }

        // DELETE: api/TokenData/DeleteToken/5
        [HttpPost]
        [ResponseType(typeof(Token))]
        public IHttpActionResult DeleteToken(int id)
        {
            Token token = db.Tokens.Find(id);
            if (token == null)
            {
                return NotFound();
            }

            db.Tokens.Remove(token);
            db.SaveChanges();

            return Ok(token);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TokenExists(int id)
        {
            return db.Tokens.Count(e => e.TokenId == id) > 0;
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
    }
}