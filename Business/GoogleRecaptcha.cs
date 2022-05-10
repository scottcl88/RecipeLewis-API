using Microsoft.Extensions.Configuration;
using RestSharp;

namespace RecipeLewis.Business;

public static class GoogleRecaptcha
{
    public static async Task<RestResponse> Verify(IConfiguration configuration, string token, bool useSurfNEat = true)
    {
        RestClient client = new RestClient("https://www.google.com/recaptcha/api/siteverify");
        RestRequest request = new RestRequest();
        if (useSurfNEat)
        {
            request.AddParameter("secret", configuration["GoogleRecaptchaServerSecret"]);
        }
        else
        {
            request.AddParameter("secret", configuration["GoogleRecaptchaPersonalSiteSecretKey"]);
        }
        request.AddParameter("response", token);
        return await client.PostAsync(request);
    }
}