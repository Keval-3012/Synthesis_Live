using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using EntityModels.Models;
using Syncfusion.EJ2.DocumentEditor;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace SynthesisRepo.Controllers
{
    public class DocumentEditorController : ApiController
    {
        private DBContext db = new DBContext();
        [HttpPost]
        public HttpResponseMessage Import()
        {
            if (HttpContext.Current.Request.Files.Count == 0)
                return null;

            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            int index = file.FileName.LastIndexOf('.');
            string type = index > -1 && index < file.FileName.Length - 1 ?
                file.FileName.Substring(index) : ".docx";
            Stream stream = file.InputStream;
            stream.Position = 0;

            Syncfusion.EJ2.DocumentEditor.WordDocument document = Syncfusion.EJ2.DocumentEditor.WordDocument.Load(stream, GetFormatType(type.ToLower()));
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
            document.Dispose();
            return new HttpResponseMessage() { Content = new StringContent(json) };
        }
        [HttpPost]
        public HttpResponseMessage Import(string fileName)
        {
            var Array = fileName.Split('_');
            Document doc = new Document();
            doc = db.Documents.Find(Convert.ToInt32(Array[2].Split('.')[0]));
            string path = System.Web.HttpContext.Current.Server.MapPath("~\\UserFiles\\DocFiles" + doc.FilePath);
            path += @"\" + fileName;
            if (!File.Exists(path))
                return null;
            Stream stream = File.OpenRead(path);
            WordDocument document = WordDocument.Load(stream, Syncfusion.EJ2.DocumentEditor.FormatType.Docx);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
            // Releases unmanaged and optionally managed resources.
            document.Dispose();
            stream.Close();
            return new HttpResponseMessage() { Content = new StringContent(json, Encoding.UTF8, "text/plain") };
        }
        [HttpPost]
        public string Save([FromBody] ExportData exportData)
        {
            Byte[] data = Convert.FromBase64String(exportData.documentData.Split(',')[1]);
            MemoryStream stream = new MemoryStream();
            stream.Write(data, 0, data.Length);
            Syncfusion.EJ2.DocumentEditor.FormatType type = GetFormatType(exportData.fileName);
            stream.Position = 0;
            try
            {
                var Array = exportData.fileName.Split('_');
                Document doc = new Document();
                doc = db.Documents.Find(Convert.ToInt32(Array[2].Split('.')[0]));
                int Id = Convert.ToInt32(Array[2].Split('.')[0]);
                string path = System.Web.HttpContext.Current.Server.MapPath("~\\UserFiles\\DocFiles" + doc.FilePath);

                string Fullpath = path + @"\" + exportData.fileName;
                if (File.Exists(Fullpath))
                {
                    File.Delete(Fullpath);
                }

                FileStream fileStream = new FileStream(path + @"/" + exportData.fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fileStream.Position = 0;
                if (type != Syncfusion.EJ2.DocumentEditor.FormatType.Docx)
                {
                    Syncfusion.DocIO.DLS.WordDocument document = new Syncfusion.DocIO.DLS.WordDocument(stream, Syncfusion.DocIO.FormatType.Docx);
                    document.Save(fileStream, GetDocIOFomatType(type));
                    document.Close();
                }
                else
                {
                    stream.CopyTo(fileStream);
                }
                fileStream.Close();
                stream.Dispose();

                var version = db.DocumentFileVersions.Where(w => w.DocumentId == Id).Count() > 0 ? db.DocumentFileVersions.Where(w => w.DocumentId == Id).Max(m => m.Version + 1).ToString() : "1";
                DocumentFileVersion dfv = new DocumentFileVersion();
                dfv.AttachFile = doc.AttachFile + "_" + version + doc.AttachExtention;
                dfv.DocumentId = Id;
                dfv.Version = Convert.ToInt32(version);
                dfv.CreatedOn = DateTime.Now;
                dfv.CreatedBy = UserModule.getUserId().ToString();
                db.DocumentFileVersions.Add(dfv);
                db.SaveChangesAsync();

                string path1 = System.Web.HttpContext.Current.Server.MapPath("~\\UserFiles\\DocFilesVersion" + doc.FilePath);
                if (!Directory.Exists(path1))
                {
                    Directory.CreateDirectory(path1);
                }
                string Fullpath1 = path1 + @"\" + dfv.AttachFile;

                File.Copy(Fullpath, Fullpath1, true);

                return "Success";
            }
            catch
            {
                return "Failed";
            }
        }

        internal static Syncfusion.EJ2.DocumentEditor.FormatType GetFormatType(string fileName)
        {
            int index = fileName.LastIndexOf('.');
            string format = index > -1 && index < fileName.Length - 1 ? fileName.Substring(index + 1) : "";
            if (string.IsNullOrEmpty(format))
                throw new NotSupportedException("EJ2 DocumentEditor does not support this file format.");
            switch (format.ToLower())
            {
                case "dotx":
                case "docx":
                case "docm":
                case "dotm":
                    return Syncfusion.EJ2.DocumentEditor.FormatType.Docx;
                case "dot":
                case "doc":
                    return Syncfusion.EJ2.DocumentEditor.FormatType.Doc;
                case "rtf":
                    return Syncfusion.EJ2.DocumentEditor.FormatType.Rtf;
                case "txt":
                    return Syncfusion.EJ2.DocumentEditor.FormatType.Txt;
                case "xml":
                    return Syncfusion.EJ2.DocumentEditor.FormatType.WordML;
                default:
                    throw new NotSupportedException("EJ2 DocumentEditor does not support this file format.");
            }
        }

        internal static Syncfusion.DocIO.FormatType GetDocIOFomatType(Syncfusion.EJ2.DocumentEditor.FormatType type)
        {
            switch (type)
            {
                case Syncfusion.EJ2.DocumentEditor.FormatType.Docx:
                    return Syncfusion.DocIO.FormatType.Docx;
                case Syncfusion.EJ2.DocumentEditor.FormatType.Doc:
                    return Syncfusion.DocIO.FormatType.Doc;
                case Syncfusion.EJ2.DocumentEditor.FormatType.Rtf:
                    return Syncfusion.DocIO.FormatType.Rtf;
                case Syncfusion.EJ2.DocumentEditor.FormatType.Txt:
                    return Syncfusion.DocIO.FormatType.Txt;
                case Syncfusion.EJ2.DocumentEditor.FormatType.WordML:
                    return Syncfusion.DocIO.FormatType.WordML;
                default:
                    throw new NotSupportedException("DocIO does not support this file format.");
            }
        }
    }
}
