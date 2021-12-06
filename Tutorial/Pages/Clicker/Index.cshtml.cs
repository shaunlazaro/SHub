using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tutorial.Managers;
using ElectronNET.API;

namespace Tutorial.Pages.Clicker
{
    public class IndexModel : PageModel
    {   
        public void OnGet()
        {
            InitEvents();
        }

        void InitEvents()
        {
            // APP.GETPATH(home) or something to get save location, load current panels.
        }
    }
}
