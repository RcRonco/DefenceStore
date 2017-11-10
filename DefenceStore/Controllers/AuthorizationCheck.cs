using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DefenceStore.Models;

namespace DefenceStore.Controllers
{
    public class AuthorizationCheck : Controller
    {
        public static bool Authorized(HttpSessionStateBase session)
        {
            return (Customer)session["Client"] != null;
        }

        public static bool AdminAuthorized(HttpSessionStateBase session)
        {
            return Authorized(session) && ((Customer)session["Client"]).IsAdmin;
        }
    }
}