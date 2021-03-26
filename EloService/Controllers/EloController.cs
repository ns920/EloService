﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EloService.Controllers
{
    //[Produces("application/json")]

    public class EloController : Controller
    {
        [Route("api/Elo")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("api/Elo/Calculate")]
        [HttpPost]
        public IActionResult Calculate(int elo1, int elo2)
        {
            try
            {
                var context = new
                {
                    elo1 = elo1,
                    elo2 = elo2,
                    message = "Success"
                };
                return Json(context);
            }
            catch (System.Exception ex)
            {
                var context = new
                {
                    message = "Error",
                };
                return Json(context);
            }
        }
    }
}