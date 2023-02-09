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
        public IQueryable<Token> ListTokens()
        {
            return db.Tokens;
        }

        // GET: api/TokenData/FindTokens/5
        [HttpGet]
        [ResponseType(typeof(Token))]
        public IHttpActionResult FindToken(int id)
        {
            Token token = db.Tokens.Find(id);
            if (token == null)
            {
                return NotFound();
            }

            return Ok(token);
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

            if (id != token.TokenID)
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

            return CreatedAtRoute("DefaultApi", new { id = token.TokenID }, token);
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
            return db.Tokens.Count(e => e.TokenID == id) > 0;
        }
    }
}