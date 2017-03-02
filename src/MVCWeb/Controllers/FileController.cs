using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using MVCWeb.Model.Models;
using MVCWeb.DataSvc.Svc;
using TuesPechkin;
using System.Drawing.Printing;

namespace MVCWeb.Controllers
{
    public class FileController : BaseController
    {
        public INullUserDataSvc NullUserDataSvc { get; set; }

        //上传图片
        [HttpPost]
        public ActionResult JqueryUploadImg(HttpPostedFileBase upFile, int pt)
        {
            if (upFile == null || upFile.ContentLength > 1024 * 1024 * 3)
            {
                return Json(new { error = "文件太大" });
            }
            string upPath = "";
            switch (pt)
            {
                case (int)EnumObjectType.博客:
                    upPath = ConfigurationManager.AppSettings["BlogFilePath"];
                    break;
                case (int)EnumObjectType.社区:
                    upPath = ConfigurationManager.AppSettings["NewBeeFilePath"];
                    break;
                //case (int)EnumObjectType.问题:
                //    upPath = ConfigurationManager.AppSettings["QuestionFilePath"];
                //    break;
                //case (int)EnumObjectType.笔记:
                //    upPath = ConfigurationManager.AppSettings["NoteFilePath"];
                //    break;
                default:
                    upPath = ConfigurationManager.AppSettings["NewBeeFilePath"];
                    break;
            }
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            upPath = upPath + date + "\\";
            if (!Directory.Exists(upPath))
            {
                Directory.CreateDirectory(upPath);
            }
            string ext = upFile.FileName == "blob" ? ".png" : Path.GetExtension(upFile.FileName);
            string newName = Guid.NewGuid().ToString() + ext;
            upFile.SaveAs(upPath + newName);

            return Json(new { error = "", path = date + ":" + newName });
        }

        //获取文件content-type
        public string GetContentType(string ext)
        {
            switch (ext)
            {
                case ".jpg":
                    return "application/x-jpg";
                case ".png":
                    return "image/png";
                case ".bmp":
                    return "application/x-bmp";
                case ".gif":
                    return "image/gif";
                default:
                    return "application/octet-stream";
            }
        }

        //下载图片
        public ActionResult DownloadImg(string path, int pt, int w = 0, int h = 0)
        {
            string fPath = "";
            switch (pt)
            {
                case (int)EnumObjectType.博客:
                    fPath = ConfigurationManager.AppSettings["BlogFilePath"];
                    break;
                case (int)EnumObjectType.社区:
                    fPath = ConfigurationManager.AppSettings["NewBeeFilePath"];
                    break;
                //case (int)EnumObjectType.问题:
                //    fPath = ConfigurationManager.AppSettings["QuestionFilePath"];
                //    break;
                //case (int)EnumObjectType.笔记:
                //    fPath = ConfigurationManager.AppSettings["NoteFilePath"];
                //    break;
                default:
                    fPath = ConfigurationManager.AppSettings["NewBeeFilePath"];
                    break;
            }
            fPath = fPath + path.Replace(":", "\\");
            string contentType = GetContentType(Path.GetExtension(fPath));
            if (w == 0 || h == 0)
            {
                return File(fPath, contentType, "temp");
            }
            else
            {
                //下载缩略图
                using (Image image = Image.FromFile(fPath))
                {
                    float iw = image.Width;
                    float ih = image.Height;
                    if (iw > ih)
                    {
                        h = (int)(ih / iw * w);
                    }
                    else
                    {
                        w = (int)(iw / ih * h);
                    }

                    Bitmap newImag = new Bitmap(w, h);
                    Graphics g = Graphics.FromImage(newImag);
                    g.DrawImage(image, new Rectangle(0, 0, w, h), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

                    MemoryStream ms = new MemoryStream();
                    newImag.Save(ms, ImageFormat.Jpeg);

                    Image temp = Image.FromStream(ms);
                    ImageConverter converter = new ImageConverter();
                    byte[] imgbytes = (byte[])converter.ConvertTo(temp, typeof(byte[]));
                    return File(imgbytes, contentType, "temp");
                }
            }
        }

        //html to pdf
        [ValidateInput(false)]
        public ActionResult HtmlToPdf(Guid uid)
        {
            Resume resume = NullUserDataSvc.GetByID(uid).Resumes.FirstOrDefault();
            if (string.IsNullOrEmpty(resume.Html))
            {
                return Content("简历为空");
            }

            var document = new HtmlToPdfDocument
            {
                GlobalSettings =
                {
                    ProduceOutline = true,
                    DocumentTitle = "简历",
                    PaperSize = PaperKind.A4,
                    Margins =
                    {
                        All = 1.375,
                        Unit = Unit.Centimeters
                    }
                },
                Objects = { new ObjectSettings { HtmlText = resume.Html, WebSettings = new WebSettings { DefaultEncoding = "utf-8" } } }
            };

            var tempFolderDeployment = new TempFolderDeployment();
            var win32EmbeddedDeployment = new Win32EmbeddedDeployment(tempFolderDeployment);
            var remotingToolset = new RemotingToolset<PdfToolset>(win32EmbeddedDeployment);

            var converter = new ThreadSafeConverter(remotingToolset);

            byte[] pdfBuf = converter.Convert(document);
            remotingToolset.Unload();

            return File(pdfBuf, "application/pdf", "简历.pdf");
        }
    }
}