using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tutorial.Managers;

namespace Tutorial.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            //SidebarManager.Init();
            //TitlebarManager.Init();
        }
    }
}
