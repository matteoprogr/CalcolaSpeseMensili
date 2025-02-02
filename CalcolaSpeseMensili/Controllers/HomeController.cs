using CalcolaSpeseMensili.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using CalcolaSpeseMensili.Service;


namespace CalcolaSpeseMensili.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CalcoloSpeseService calcoloSpeseService = new CalcoloSpeseService();
       
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public Dictionary<string,double> calcolaspese(IFormFile file, string mese)
        {
            Dictionary<string, double> datiLavorati = new Dictionary<string, double>();
            if (file != null && file.Length > 0)
            {
                Dictionary<string,double> data = new Dictionary<string,double>();
                int righeNonDelMeseCalcolato = 0;

                // apre lo stream del file
                using (var stream = file.OpenReadStream())
                {
                    // specifica l'encoding UTF-8 per leggere correttamente il file excel
                    var encoding = Encoding.UTF8; 

                    // configura la lettura del file excel con l'encoding specificato
                    var configuration = new ExcelReaderConfiguration
                        {
                            FallbackEncoding = encoding,// Imposta l'encoding della configurazione
                        };

                    // crea un lettore per il file excel utilizzando la configurazione specificata
                        using (var reader = ExcelReaderFactory.CreateReader(stream, configuration))
                    {
                        // legge il ocntenuto del file excel e lo converte in un DataSet
                        var result = reader.AsDataSet();

                        // verifica se il DataSet contiene almeno una tabella
                        if(result.Tables.Count > 0)
                        {
                            //ottiene la prima tabella del DataSet
                            var dataTable = result.Tables[0];
                            
                            // itera sulle righe della tabella
                            for (int i=0; i<dataTable.Rows.Count;i++)
                            {
                                // itera sulle colonne
                                    string? valoreString = Convert.ToString(dataTable.Rows[i][5]);
                                    string? presso = Convert.ToString(dataTable.Rows[i][4]);
                                    string? dataValuta = Convert.ToString(dataTable.Rows[i][2]);

                                    if ((!string.IsNullOrEmpty(valoreString)|| !string.IsNullOrEmpty(presso)) && !string.IsNullOrEmpty(dataValuta))
                                    {
                                        string[] splitDataValuta = dataValuta.Split('/');
                                        
                                        if (splitDataValuta.Length > 1 
                                            && splitDataValuta[1].Equals(mese) 
                                            && !string.IsNullOrEmpty(presso))
                                        {
                                            var valore = double.Parse(valoreString);
                                            if (valore < 0)
                                            {
                                                data.Add(presso, valore);
                                            }
                                            // aggiunge lal coppia chiave-valore alla dictionary data
                                        }
                                    }
                            }
                        }
                    }
                }

                datiLavorati = calcoloSpeseService.Calcolaspese(data);
                
            }
            return datiLavorati;
            
        }
    }
}
