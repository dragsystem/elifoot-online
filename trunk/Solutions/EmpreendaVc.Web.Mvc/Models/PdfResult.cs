using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gerando_PDF.Models;
using System.IO;
using iTextSharp.text;

namespace EmpreendaVc.Web.Mvc.Models
{
    public class PdfResult : ActionResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            int i = (int)HttpContext.Current.Session["idprojeto"];
            HtmlToPdfBuilder builder = new HtmlToPdfBuilder(PageSize.LETTER);

            HtmlPdfPage page1 = builder.AddPage();

            #region
            page1.AppendHtml(@"<h2>Descrição Geral</h2>");
            #endregion
            byte[] file = builder.RenderPdf();
            byte[] buffer = new byte[4096];

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/pdf";

            MemoryStream pdfStream = new MemoryStream(file);

            while (true)
            {
                int read = pdfStream.Read(buffer, 0, buffer.Length);
                if (read == 0)
                    break;
                response.OutputStream.Write(buffer, 0, read);
            }
            response.End();
        }
    }
}