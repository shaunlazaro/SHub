using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tutorial.Managers;

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

        }
    }
}
