using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using EscolaModelo.Models;
using Newtonsoft.Json;

namespace EscolaModelo.Controllers
{
    public class HomeController : Controller
    {
        HttpClient client = new HttpClient();
        Uri rootUrl = new Uri("http://localhost:60877/");
        bool erroFormatXml = false;

        // GET: Home/Index/
        public async Task<ActionResult> Index()
        {
            List<Aluno> alunos = new List<Aluno>();

            // Passa a URL base e o formato
            InitHttp();

            // Envia requisição usando o endPoint
            try
            {
                HttpResponseMessage Resp = await client.GetAsync("Aluno/Consultar/");

                // Verifica se foi bem sucedido
                if (Resp.IsSuccessStatusCode)
                {
                    // Armazena a resposta da API
                    var resposta = Resp.Content.ReadAsStringAsync().Result;

                    // Deserializa a lista
                    alunos = JsonConvert.DeserializeObject<List<Aluno>>(resposta);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                MsgRetorno("alert-danger", "Desculpe", "Não foi possível listar os alunos.");
            }

            return View(alunos);
        }

        // POST: Home/Adicionar/
        public async Task<ActionResult> Adicionar(HttpPostedFileBase import)
        {
            if (import == null)
            {
                MsgRetorno("alert-warning", "Aviso", "É necessário selecionar um arquivo.");

                return RedirectToAction("Index");
            }

            // Seta o caminho onde será salvo o arquivo
            var uploadPath = "C:\\Users\\Notebook05\\Downloads\\";
            // Pasta para armazenamento em produção
            //var uploadPath = Server.MapPath("~/Imports/");

            // Pega a extensão e verifica se é .XML
            var ext = Path.GetExtension(import.FileName).ToLower();
            if (ext != ".xml")
            {
                MsgRetorno("alert-warning", "Aviso", "O arquivo deve ser em formato XML.");

                return RedirectToAction("Index");
            }

            // Monta o nome do arquivo com sua extensão
            string name = DateTime.Now.ToBinary().ToString() + ext;
            string caminhoArquivo = Path.Combine(uploadPath, Path.GetFileName(name));

            // Salva o arquivo .XML
            import.SaveAs(caminhoArquivo);

            // Converte o arquivo importado para uma lista de alunos
            var alunos = ConvertXmlInAluno(caminhoArquivo);

            // Verifica se deu erro na leitura do arquivo XML
            if (!erroFormatXml)
            {
                // Passa a URL base e o formato
                InitHttp();

                // Envia requisição usando o endPoint
                try
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(alunos), Encoding.UTF8, "application/json");
                    HttpResponseMessage Resp = await client.PostAsync("Aluno/Adicionar/", content);

                    // Verifica se foi bem sucedido
                    if (Resp.IsSuccessStatusCode)
                    {
                        // Armazena a resposta da API
                        var resposta = Resp.Content.ReadAsStringAsync().Result;

                        // Verifica se todos o aluno foram cadastrados
                        if (Convert.ToBoolean(resposta))
                            MsgRetorno("alert-success", "Sucesso", "Os alunos foram cadastrados.");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    MsgRetorno("alert-danger", "Desculpe", "Não foi possível adicionar os alunos.");
                }
            }

            return RedirectToAction("Index");
        }

        private void InitHttp()
        {
            client.BaseAddress = rootUrl;
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private List<Aluno> ConvertXmlInAluno(string caminhoDocXml)
        {
            // Instancia objetos relacionados a Aluno
            Aluno obj;
            List<Aluno> alunos = new List<Aluno>();
            
            // Instancia objetos relacionados ao documento XML
            var xmldoc = new XmlDocument();
            XmlNodeList xmlnode;
            FileStream fs = new FileStream(caminhoDocXml, FileMode.Open, FileAccess.Read);

            try
            {
                // Carrega o arquivo e encontra os nós alunos
                xmldoc.Load(fs);
                xmlnode = xmldoc.GetElementsByTagName("Aluno");

                // Carrega a lista do tipo Aluno de acordo com os nós Aluno do documento xml
                for (int i = 0; i <= xmlnode.Count - 1; i++)
                {
                    obj = new Aluno()
                    {
                        CPF = xmlnode[i].ChildNodes.Item(0).InnerText,
                        Nome = xmlnode[i].ChildNodes.Item(1).InnerText,
                        DataNasc = Convert.ToDateTime(xmlnode[i].ChildNodes.Item(2).InnerText),
                        NomeMae = xmlnode[i].ChildNodes.Item(3).InnerText,
                        Bairro = xmlnode[i].ChildNodes.Item(4).InnerText,
                        Logradouro = xmlnode[i].ChildNodes.Item(5).InnerText,
                        Nro = Convert.ToInt32(xmlnode[i].ChildNodes.Item(6).InnerText),
                        Complemento = xmlnode[i].ChildNodes.Item(7).InnerText
                    };

                    alunos.Add(obj);
                }
            }
            catch (Exception ex)
            {
                erroFormatXml = true;
                System.Diagnostics.Debug.WriteLine(ex);
                MsgRetorno("alert-danger", "Desculpe", "O formato do arquivo deve ser válido.");
            }

            return alunos;
        }

        // Método responsável pelos dados inseridos na mensagem de retorno
        private void MsgRetorno(string tipo, string titulo, string msg)
        {
            TempData["tipoMensagem"] = tipo;
            TempData["tituloMensagem"] = titulo;
            TempData["mensagem"] = msg;
        }
    }
}