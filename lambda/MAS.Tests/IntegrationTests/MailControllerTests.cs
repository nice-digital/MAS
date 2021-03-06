﻿using System.Threading.Tasks;
using Xunit;
using Shouldly;
using MAS.Tests.Infrastructure;
using System.Text;
using System;
using MAS.Configuration;
using System.Net.Http.Headers;
using System.Net.Http;
using MailChimp.Net.Models;
using Newtonsoft.Json;

namespace MAS.Tests.IntegrationTests.Mail
{
    //public class MailControllerTests : TestBase
    //{
    //    [Fact]
    //    public async Task PutRequestCreatesAndSendsCampaign()
    //    {
    //        //Arrange
    //        const string mailChimpCampaignsURI = "https://us5.api.mailchimp.com/3.0/campaigns/";

    //        var authValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"anystring:{AppSettings.MailConfig.ApiKey}")));
    //        var client = new HttpClient()
    //        {
    //            DefaultRequestHeaders = { Authorization = authValue }
    //        };

    //        //Act
    //        var response = await _client.PutAsync("/api/mail/daily", null);

    //        //Get campaign to check if it saved
    //        var campaignId = await response.Content.ReadAsStringAsync();
    //        var campaign = await client.GetAsync(mailChimpCampaignsURI + campaignId);
    //        var campaignJson = await campaign.Content.ReadAsStringAsync();
    //        var campaignResult = JsonConvert.DeserializeObject<Campaign>(campaignJson);

    //        // Assert
    //        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
    //        campaignResult.Status.ShouldNotBeNull();
    //        campaignResult.Status.ShouldNotBe("Draft");
    //    }
    //}
}
