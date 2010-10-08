using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NzbDrone.Core.Providers;

namespace NzbDrone.Web.Controllers
{
    public class MediaControlController : Controller
    {
        //
        // GET: /MediaControl/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult MediaDetect()
        {
            Core.Providers.IMediaDiscoveryProvider disco = new Core.Providers.MediaDiscoveryProvider();
            return Json(new { Discovered = disco.DiscoveredMedia }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LightUpMedia()
        {
            Core.Providers.IMediaDiscoveryProvider disco = new Core.Providers.MediaDiscoveryProvider();
            if (disco.Providers.Count > 0)
            {
                IMediaProvider p = disco.Providers[0];
                return Json(new { ID = 0, FriendlyName = p.FriendlyName, HTML = "<span class='MediaRenderer XBMC'><span class='Play'>Play</span><span class='Pause'>Pause</span><span class='Stop'>Stop</span></span>" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { ID = 0, FriendlyName = "", HTML = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ControlMedia()
        {
            Core.Providers.IMediaDiscoveryProvider disco = new Core.Providers.MediaDiscoveryProvider();
            IMediaProvider p = disco.Providers[0];
            string action = Request["Action"];
            switch (action)
            {
                case "Play":
                    p.Play();
                    break;
                case "Pause":
                    p.Pause();
                    break;
                case "Stop":
                    p.Stop();
                    break;
                default:
                    break;
            }
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}
