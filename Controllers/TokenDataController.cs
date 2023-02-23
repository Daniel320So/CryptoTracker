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
using CryptoTracker.Migrations;
using CryptoTracker.Models;

namespace CryptoTracker.Controllers
{
    public class TokenDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all token data
        /// </summary>
        /// <returns>
        /// IEnumerable<TokenDto>
        /// </returns>
        /// <example>
        /// GET: api/TokenData/ListTokens
        /// </example>

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
                TokenSymbol = t.TokenSymbol,

            }));

            return TokenDtos;

        }

        /// <summary>
        /// Returns data of a token
        /// </summary>
        /// <param name="id"> Token Id</param>
        /// <returns>
        /// TokenDto
        /// </returns>
        /// <example>
        /// GET: api/TokenData/FindTokens/5
        /// </example>

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
                TokenSymbol = token.TokenSymbol,
            };
            if (token == null)
            {
                return NotFound();
            }

            return Ok(tokenDto);
        }

        /// <summary>
        ///update data of a token
        /// </summary>
        /// <param name="id"> Token Id</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Tokens in the database
        /// </returns>
        /// <example>
        /// POST: api/TokenData/UpdateToken/5
        /// </example>

        [HttpPost]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateToken(int id, Token token)
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

        /// <summary>
        /// add a new token
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Tokens in the database
        /// </returns>
        /// <example>
        /// POST: api/TokenData/AddToken
        /// </example>

        [HttpPost]
        [ResponseType(typeof(Token))]
        public IHttpActionResult AddToken(Token token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Tokens.Add(token);
            db.SaveChanges();
            //add one of each Wallet at 0 amount
            List<Wallet> Wallets = db.Wallets.ToList();
            Wallets.ForEach(w =>
                db.WalletxTokens.Add(
                    new WalletxToken
                    {
                        WalletId = w.WalletId,
                        TokenId = token.TokenId,
                        balance = 0
                    }
                )
            ); ;
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = token.TokenId }, token);
        }

        /// <summary>
        /// Delete data of a token
        /// </summary>
        /// <param name="id"> Token Id</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Tokens in the database
        /// </returns>
        /// <example>
        /// POST: api/TokenData/DeleteToken/5
        /// </example>

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

        /// <summary>
        /// Update balance in WalletxTokens with a specific TokenId
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// StatusCode(HttpStatusCode.NoContent)
        /// </returns>
        /// <param name="id">Token ID.</param>
        /// <example>
        /// POST: api/TokenData/UpdateTokenBalance/3
        /// </example>
        [HttpPost]
        public IHttpActionResult UpdateTokenBalance(int id, WalletDto walletDto)
        {
            WalletxToken WxT = db.WalletxTokens.Where(wxt => wxt.WalletId == walletDto.WalletId && wxt.TokenId == id).Include(wxt => wxt.Wallet).ToList()[0];
            WxT.balance = walletDto.WalletBalance;
            db.Entry(WxT).State = EntityState.Modified;

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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}