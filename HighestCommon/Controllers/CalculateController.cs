using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HighestCommon.Controllers
{
    [Route("/")]
    public class CalculateController : Controller
    {
        [HttpGet("{a}/{b}")]
        public int Get(int a, int b)
        {
            var max = Math.Min(a, b);
            for (int x = max; x > 0; x--)
                if (a % x == 0 && b % x == 0)
                    return x;

            return 0;
        }

    }
}
