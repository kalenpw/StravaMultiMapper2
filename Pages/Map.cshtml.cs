using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StravaMultiMapper2.Models;
using System.Text.Json;

namespace StravaMultiMapper2.Pages
{
    public class MapModel : PageModel
    {
        public void OnGet([FromServices] ActivityStore activityStore)
        {
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["ProfilePicture"] = HttpContext.Session.GetString("ProfilePicture");

            ViewData["EncodedList"] = activityStore.Activities;
            ViewData["ActivityCount"] = activityStore.Activities.Count;
        }
    }
}
