using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace ABKRuleBook.Controllers
{
    public class DocumentUploadController : Controller
    {
        // GET: DocumentUpload
        public ActionResult Index()
        {
            return View();
        }

        public string UploadDocument()
        {
            try
            {
                string FileName = "";
                string FilePath = "";

                HttpFileCollectionBase files = Request.Files;
                string lang = Request.Form["lang"];
                string UploadPath = Server.MapPath("~\\Uploads\\Documents\\" + lang + "\\");

                if (!Directory.Exists(UploadPath))
                {
                    Directory.CreateDirectory(UploadPath);
                }

                //Delete Existing Files.....
                //string[] filePaths = Directory.GetFiles(UploadPath);
                //foreach (var f in filePaths)
                //{
                //    System.IO.File.Delete(f);
                //}

                HttpPostedFileBase file = files[0];

                // Checking for Internet Explorer    
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    FilePath = testfiles[testfiles.Length - 1];
                }
                else
                {
                    FileName = file.FileName;
                }

                string ext = Path.GetExtension(FileName);
                if (ext != ".pdf")
                {
                    var result = new
                    {
                        RCode = 1,
                        Message = "Please select a PDF file",
                        FilePath = "",
                    };

                    return JsonConvert.SerializeObject(result);
                }
                else
                {
                    // Get the complete folder path and store the file inside it.    
                    FilePath = Path.Combine(UploadPath, FileName);
                    file.SaveAs(FilePath);

                    var result = new
                    {
                        RCode = 0,
                        Message = "Success",
                        FilePath = FilePath,
                    };

                    return JsonConvert.SerializeObject(result);
                }
            }
            catch (Exception ex)
            {
                var result = new
                {
                    RCode = 1,
                    Message = ex.Message
                };
                return JsonConvert.SerializeObject(result);
            }
        }
    }
}