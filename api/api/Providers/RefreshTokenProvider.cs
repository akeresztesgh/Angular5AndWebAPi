﻿using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using api.Models;

namespace api.Providers
{
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var userId = context.Ticket.Properties.Dictionary["userId"];

            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            var refreshTokenId = Guid.NewGuid().ToString("n");

            using (var repo = new MyDbContext())
            {
                var refreshTokenLifeTime = api.Utils.Configuration.RefreshTokenExpireTimeMinutes;
                var token = new RefreshToken()
                {
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime)),
                    Token = refreshTokenId,
                    UserId = userId
                };

                context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
                context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

                token.ProtectedTicket = context.SerializeTicket();

                var result = await repo.AddRefreshToken(token);

                if (result)
                {
                    context.SetToken(refreshTokenId);
                }

            }
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {

            //var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            using (var repo = new MyDbContext())
            {
                var refreshToken = await repo.FindRefreshTokenAsync(context.Token);

                if (refreshToken != null)
                {
                    //Get protectedTicket from refreshToken class
                    context.DeserializeTicket(refreshToken.ProtectedTicket);
                    var result = await repo.RemoveRefreshToken(context.Token);
                }
            }
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }
}