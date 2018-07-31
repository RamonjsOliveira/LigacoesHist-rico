using LigacoesHistorico.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LigacoesHistorico.Controllers
{
    public class ArquivoController : Controller
    {
        // GET: Arquivo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Upload()
        {

            return View();
        }

        public ActionResult Enviar()
        {
            
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];

                if (arquivo.ContentLength > 0)
                {
                    var texto = arquivo.ToString();
                    var uploadPath = Server.MapPath("~/Content/Arquivos");
                    string caminhoArquivo = Path.Combine(@uploadPath, Path.GetFileName(arquivo.FileName));

                    arquivo.SaveAs(caminhoArquivo);
                    string[] lines = System.IO.File.ReadAllLines(caminhoArquivo);

                    string line;
                    List<Historico> historico = new List<Historico>();

                    var count = 0;

                    using (System.IO.StreamReader file =
                        new System.IO.StreamReader(caminhoArquivo)) { 
                        while ((line = file.ReadLine()) != null)
                        {
                            string[] words = line.Split(';');
                            if (count != 0)
                            {
                                var dddOrigem = words[1].Substring(0, 2);
                                var dddDestino = words[2].Substring(0, 2);
                                var hist = new Historico
                                {
                                    DataHora = DateTime.Parse(words[0]),
                                    TelefoneOrigem = words[1],
                                    TelefoneDestino = words[2],
                                    Duracao = int.Parse(words[3]),
                                    DDDOrigem = int.Parse(dddOrigem),
                                    DDDDestino = int.Parse(dddDestino)
                                };
                                historico.Add(hist);
                            }

                            count++;
                        }
                    }

                    var dddDiferente = historico.Where(x=> x.DDDOrigem != x.DDDDestino).Count();
                    
                     var duracaoPorDDD = historico.GroupBy(l => l.DDDOrigem)
                    .Select(cl => new
                    {
                        DDD = cl.First().DDDOrigem,
                        NumLigacoes = cl.Count().ToString(),
                        Duracao = cl.Sum(c => c.Duracao).ToString(),
                    }).ToList();

                    using (TextWriter tw = new StreamWriter(uploadPath + "\\LogLigacoes.txt"))
                    {
                        tw.WriteLine("TOTAL_CLIENTES_LIGARAM: " + historico.Count);
                        tw.WriteLine("DURACAO_MEDIA: ");
                        foreach (var item in duracaoPorDDD) {

                            tw.WriteLine(item.DDD + ": " + item.Duracao);
                        }

                        tw.WriteLine("TOTAL_CLIENTES_LIGARAM_OUTRO_DDD: " + dddDiferente);
                    }

                }
            }
            
            Process.Start(Server.MapPath("~/Content/Arquivos/LogLigacoes.txt"));

            ViewBag.Status = "Arquivo processado com sucesso.";
            return RedirectToAction("Index");
        }

    }

}
