using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StravaMultiMapper2.Models;
using StravaMultiMapper2.Util;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace StravaMultiMapper2.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _config;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<IActionResult> OnGetAsync([FromServices] ActivityStore activityStore)
        {
            var stravaCode = Request.Query["code"].ToString();

            if (!String.IsNullOrEmpty(stravaCode))
            {
                var postData = new Dictionary<string, string>
                {
                    { "client_id", _config["Strava:ClientId"] },
                    { "client_secret", _config["Strava:ClientSecret"] },
                    { "code", stravaCode },
                    { "grant_type", "authorization_code" }
                };

                var content = new FormUrlEncodedContent(postData);

                using var client = new HttpClient();
                var response = await client.PostAsync(Urls.TokenUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonObject = JsonNode.Parse(responseContent);

                var accessToken = jsonObject["access_token"].ToString();
                var username = jsonObject["athlete"]["username"].ToString();
                var profilePicture = jsonObject["athlete"]["profile_medium"].ToString();

                HttpContext.Session.SetString("Username", username);
                HttpContext.Session.SetString("ProfilePicture", profilePicture);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var page = 1;
                var donePaginating = false;
                var allActivities = new List<String>();

                while (!donePaginating)
                {
                    var activities = await client.GetAsync(Urls.ListActivitiesUrl + "&page=" + page.ToString());
                    var activitiesContent = await activities.Content.ReadAsStringAsync();

                    var stravaActivities = JsonSerializer.Deserialize<List<StravaActivity>>(activitiesContent);
                    var encodedActivities = stravaActivities.Select(i => i.map.summary_polyline).ToList();

                    donePaginating = encodedActivities.Count == 0;
                    allActivities.AddRange(encodedActivities);
                    page++;
                }

                activityStore.Activities = allActivities;

                return RedirectToPage("/Map");
            }

            return Page();
        }
    }
}
