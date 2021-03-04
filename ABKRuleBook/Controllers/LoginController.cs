using ABKRuleBook.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;

namespace ABKRuleBook.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            XDocument doc = XDocument.Load(Server.MapPath("~/UserList.xml"));
            List<Users> forms = doc.Root.Descendants("form")
                        .Select(x => new Users
                        {
                            username = (string)x.Element("username"),
                            password = (string)x.Element("password")
                        })
                        .ToList();
            ViewBag.Forms = forms;
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {
            if (User?.Identity.IsAuthenticated == true)
            {
                FormsAuthentication.SignOut();
            }

            string LDAPDomain = ConfigurationManager.AppSettings.Get("LDAPDomain");
            bool LDAPAuthentication = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("LDAPAuthentication"));

            if (LDAPAuthentication == true)
            {
                // Active Directory Authentication
                try
                {
                    using (var context = new PrincipalContext(ContextType.Domain, LDAPDomain))
                    {
                        if (!context.ValidateCredentials(fc["username"], fc["password"]))
                        {
                            TempData["msg"] = "Invalid credentials - Please try again";
                            return View();
                        }
                    }
                }
                catch (PrincipalServerDownException ex)
                {
                    TempData["msg"] = "Oops! Something went wrong - Please contact system administrator";
                    return View();
                }
            }


            string _userName = "";
            string _userID = "";

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, _userName, DateTime.Now,
                    DateTime.Now.AddMinutes(50), true,
                    _userID);

            FormsAuthentication.SetAuthCookie(_userName + "|" + _userID, true);

            return Redirect("DocumentUpload");
        }

        public ActionResult Logout()
        {
            if (User?.Identity.IsAuthenticated == false)
                return RedirectToAction("Index", "Login");

            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
    }
}