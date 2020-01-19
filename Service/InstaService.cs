﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.SessionHandlers;
using InstagramApiSharp.Logger;
using InstaWebApi.Models;
using InstaWebApi.Repository;

namespace InstaWebApi.Service
{
    public class InstaService
    {

        private IInstaApi _instaApi;
        private readonly IInstaAccountRepository<InstaAccount> _repository;

        public InstaService(IInstaAccountRepository<InstaAccount> repository)
        {
            _repository = repository;
        }

        private string GetChallengeCode(string username)
        {
            return "sad";
        }

        private async Task<bool> Login(InstaAccount account)
        {
            var delay = RequestDelay.FromSeconds(2, 2);

            // create new InstaApi instance using Builder
            _instaApi.LoadStateDataFromString(account.SessionData);

            if (_instaApi.IsUserAuthenticated) return true;

            _instaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(new UserSessionData
                {
                    UserName = account.Username,
                    Password = account.Password
                })
                .UseLogger(new DebugLogger(LogLevel.Exceptions)) // use logger for requests and debug messages
                .SetRequestDelay(delay)
                .Build();


            if (_instaApi.IsUserAuthenticated) return true;

            delay.Disable();
            var logInResult = await _instaApi.LoginAsync();


            delay.Enable();
            if (logInResult.Succeeded) return true;

            if (logInResult.Value != InstaLoginResult.ChallengeRequired) return false;

            var challenge = await _instaApi.GetChallengeRequireVerifyMethodAsync();

            if (!challenge.Succeeded) return false;

            if (challenge.Value.SubmitPhoneRequired)
            {
                //todo not implemented
            }
            else
            {
                if (challenge.Value.StepData == null) return false;

                if (!string.IsNullOrEmpty(challenge.Value.StepData.PhoneNumber))
                {
                    //todo not implemented
                }

                if (string.IsNullOrEmpty(challenge.Value.StepData.Email)) return false;

                var email =
                    await _instaApi.RequestVerifyCodeToEmailForChallengeRequireAsync();

                if (!email.Succeeded) return false;

                Console.WriteLine(
                    $"We sent verify code to this email:\n{email.Value.StepData.ContactPoint}");

                var code = GetChallengeCode(account.Username);

                var verifyLogin = await _instaApi.VerifyCodeForChallengeRequireAsync(code);

                return verifyLogin.Succeeded;
            }


            return false;
        }

        public async Task<bool> Login(string username)
        {
            var account = await _repository.Get(username);

            var result = await Login(account);

            if (result)
            {
                account.SessionData = _instaApi.GetStateDataAsString();
                var a = await _repository.Update(account);
            }

            return result;
        }

        public async Task<bool> CheckUsername(string usernameToCheck)
        {
            var result = await _instaApi.CheckUsernameAsync(usernameToCheck);

            return result.Value.Available;
        }

        public async Task<bool> Register(InstaAccount account)
        {
            var result = await _instaApi.CreateNewAccountAsync(account.Username, account.Password, account.Email, account.Fisrtname);

            return result.Value.AccountCreated;
        }

        public async Task<bool> EditAccount(InstaAccount account)
        {
            var result = await _instaApi.AccountProcessor.SetAccountPublicAsync();

            return result.Value.IsPrivate;
        }
    }
}