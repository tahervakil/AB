using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ABKRuleBook.Controllers
{
    public class ViewDocumentController : Controller
    {
        // GET: ViewDocument
        public ActionResult Index()
        {
            return View();
        }

        public FileResult OpenPDF(string ln)
        {
            string filePath = Server.MapPath("~\\Uploads\\Documents\\" + ln);
            var directory = new DirectoryInfo(filePath);
            var myFile = (from f in directory.GetFiles()
                          orderby f.LastWriteTime descending
                          select f).First();

            string PDFURL = filePath + "\\" + myFile;
            byte[] FileBytes = System.IO.File.ReadAllBytes(PDFURL);
            return File(FileBytes, "application/pdf");
        }
    }
}