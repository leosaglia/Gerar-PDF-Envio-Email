using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PDF_EMAIL.Models;
using PDF_EMAIL.Services;
using PdfSharp.Drawing;

namespace PDF_EMAIL.Controllers
{
    public class NovoColaboradorController: Controller
    {
        private readonly IEnviarEmail _enviarEmail;

        public NovoColaboradorController(IEnviarEmail enviarEmail)
        {
            _enviarEmail = enviarEmail;
        }

        public IActionResult Index()
        {            
            return View();
        }

        static string GerarPdf(NovoColaborador novoColaborador)
        {
             string filename = $"FichaNovoFuncionário_{novoColaborador.NomeColaborador}";

            using(var doc = new PdfSharp.Pdf.PdfDocument())
            {
                // https://stackoverflow.com/questions/50858209/system-notsupportedexception-no-data-is-available-for-encoding-1252
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                // https://www.youtube.com/watch?v=5-YznWj2RfQ
                var page = doc.AddPage();
                var graphics = PdfSharp.Drawing.XGraphics.FromPdfPage(page);

                var textFormatter = new PdfSharp.Drawing.Layout.XTextFormatter(graphics);
                var textFormatterCenter = new PdfSharp.Drawing.Layout.XTextFormatter(graphics);

                var fontNormal = new XFont("Arial", 12);    
                var fontBold = new XFont("Arial", 14, XFontStyle.Bold);
                var fontBoldSmall = new XFont("Arial", 12, XFontStyle.Bold);
            
                textFormatterCenter.Alignment = PdfSharp.Drawing.Layout.XParagraphAlignment.Center;

                
                textFormatterCenter.DrawString("Ficha de novo funcionário", fontBold, 
                    PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XRect(0, 50, page.Width, page.Height));

                textFormatter.DrawString("Funcionário:", fontBoldSmall, 
                    PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XRect(50, 100, page.Width, page.Height));
                textFormatter.DrawString(novoColaborador.NomeColaborador, fontNormal, 
                    PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XRect(130, 100, page.Width, page.Height));

                textFormatter.DrawString("Cargo:", fontBoldSmall, 
                    PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XRect(50, 120, page.Width, page.Height));
                textFormatter.DrawString(novoColaborador.Cargo, fontNormal, 
                    PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XRect(95, 120, page.Width, page.Height));

                textFormatter.DrawString("Área:", fontBoldSmall, 
                    PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XRect(50, 140, page.Width, page.Height));
                textFormatter.DrawString(novoColaborador.Area, fontNormal, 
                    PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XRect(90, 140, page.Width, page.Height));

                graphics.DrawLine(PdfSharp.Drawing.XPens.Black, 0, 180, page.Width, 180);

                textFormatterCenter.DrawString("Acessos", fontBold, 
                    PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XRect(0, 220, page.Width, page.Height));

                textFormatter.DrawString("AX?", fontBoldSmall, 
                    PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XRect(50, 260, page.Width, page.Height));
                textFormatter.DrawString(novoColaborador.AcessoAX ? "Sim" : "Não", fontNormal, 
                    PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XRect(80, 260, page.Width, page.Height));
                
                doc.Save($"tmp/{filename}.pdf");
            }   

            return filename;
        }

        private async Task SendEmail(string filename)
        {
            await _enviarEmail.SendEmailAsync(filename);

            return;
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {                      
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(NovoColaborador novoColaborador)
        {                   
            string filename;

            if (ModelState.IsValid)   
            {

                filename = GerarPdf(novoColaborador);

                try
                {
                    SendEmail(filename).GetAwaiter();

                    if (System.IO.File.Exists(@"tmp\" + filename + ".pdf"))
                    {
                        System.IO.File.Delete(@"tmp\" + filename + ".pdf");
                    }
                    return RedirectToAction("CadastroSucesso");
                }
                catch (Exception)
                {
                    return RedirectToAction("CadastroErro");
                } 
                finally
                {
                    ModelState.Clear();
                   
                }
                
            }           

            return View(novoColaborador);
        }

        public IActionResult CadastroSucesso()
        {
            return View();
        }

        public IActionResult CadastroErro()
        {
            return View();
        }
    }
}