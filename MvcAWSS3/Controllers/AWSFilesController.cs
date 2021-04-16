using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcAWSS3.Helpers;
using MvcAWSS3.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MvcAWSS3.Controllers
{
    public class AWSFilesController : Controller
    {
        private UploadHelper uploadhelper;
        public ServiceAWSS3 ServiceS3;
        
        public AWSFilesController(UploadHelper uploadhelper
            , ServiceAWSS3 services3)
        {
            this.uploadhelper = uploadhelper;
            this.ServiceS3 = services3;
        }


        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ListFilesAWS()
        {
            List<String> files = await this.ServiceS3.GetS3FilesAsync();
            return View(files);
        }

        public IActionResult UploadFileAWS()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFileAWS(IFormFile file)
        {
            //PRIMERO A LOCAL CON EL HELPER
            String path =
                await this.uploadhelper.UploadFileAsync(file, Folders.Images);
            //SUBIMOS EL FICHERO LOCAL A AWS
            using (FileStream stream = new FileStream(path
                , FileMode.Open, FileAccess.Read))
            {
                bool respuesta =
                    await this.ServiceS3.UploadFileAsync(stream, file.FileName);
                ViewData["MENSAJE"] = "Archivo en AWS Bucket: " + respuesta;
            };
            return View();
        }

        public async Task<IActionResult> FileAWS(String fileName)
        {
            Stream stream =
                await this.ServiceS3.GetFileAsync(fileName);
            return File(stream, "image/png");
        }

        public async Task<IActionResult> DeleteFileAWS(String fileName)
        {
            await this.ServiceS3.DeleteFileAsync(fileName);
            return RedirectToAction("ListFilesAWS");
        }
    }
}
