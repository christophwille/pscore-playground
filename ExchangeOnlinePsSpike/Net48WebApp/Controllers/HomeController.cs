using ExchangeOnlinePowerShellSpike;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace Net48WebApp.Controllers
{
	public class HomeController : Controller
	{


		public ActionResult Index()
		{
			return View();
		}

		public ActionResult About()
		{
			string result = ExoCertAuthN.HardcodedDemo();
			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}