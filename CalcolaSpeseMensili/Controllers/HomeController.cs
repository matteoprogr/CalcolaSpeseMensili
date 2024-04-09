using CalcolaSpeseMensili.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CalcolaSpeseMensili.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

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
        public Dictionary<string,double> calcolaspese(IFormFile file)
        {
          
            Dictionary<string, double> datiLavorati = new Dictionary<string, double>();
            if (file != null && file.Length > 0)
            {
                Dictionary<string,string> data = new Dictionary<string,string>();

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

                    // crea un lettore per ilfile excel utilizzando la configurazione specificata
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
                                for (int j=0;j<dataTable.Columns.Count;j++)
                                {
                                    // crea una chiave composta dall'indice di riga e colonna
                                    string? chiave = $"{i},{j}";

                                    // ottiene il valore dalla cella corrispondente alla riga e colonna specificata
                                    string? valore = Convert.ToString(dataTable.Rows[i][j]);

                                    //verifica se il valore non è vuoto
                                    if (valore!="")
                                    {
                                        // aggiunge lal coppia chiave-valore alla dictionary data
                                        data.Add(chiave, valore);
                                    }
                                    
                                }
                            }
                        }
                    }
                }
                
                //dichiaro la matrice ricavado le righe dalla lunghezza della dictioanary / le colonne
                string? s = "";
                double righeD = data.Count / 5.0;
                double dif=righeD-Math.Floor(righeD);
                int righe = 0;
                if (dif > 0.5)
                {
                    righe = (int)Math.Ceiling(righeD);
                }
                else
                {
                    righe = (int)Math.Floor(righeD);
                }
                
                string[,] matrice = new string[righe, 5];
               

               // inizializzo la matrice 
                foreach (var v in data)
                {
                    // ricavo la posizione in cui inserire il valore dalla chiave della matrice
                    string [] arr= v.Key.Split(',');
                    int[]colRiga=new int[arr.Length];

                   
                    for (int i=0;i<arr.Length;i++)
                    {
                        colRiga[i] = int.Parse(arr[i]);
                        
                    }

                    if (v.Value.Contains("presso"))
                    {
                        s = v.Value;
                        int index = s.IndexOf("presso");
                        s=s.Substring(index);
                        matrice[colRiga[0], colRiga[1]] = s;

                    }
                    else
                    {
                        matrice[colRiga[0], colRiga[1]] = v.Value;
                       
                    }
                   


                }
                // inserisco i dati nella matrice in una nuova dictionary
                for (int i=0;i<matrice.GetLength(0);i++) 
                {
                    for (int j = 0; j < matrice.GetLength(1); j++)
                    {
                        if (matrice[i,j]!=null)
                        {
                            if (matrice[i, j].Contains("presso"))
                            {

                                // se la dataiLavorati contiene già una chiave uguale alla string matrice[i, j] 
                                // allora unisco il valore prensente nella chiave con il valore presente nella colonna successiva
                                if (datiLavorati.ContainsKey(matrice[i, j]))
                                {
                                    double valore = datiLavorati[matrice[i, j]];
                                    datiLavorati[matrice[i, j]] = valore + double.Parse(matrice[i, j + 1]);
                                }
                                else
                                {
                                    datiLavorati[matrice[i, j]] = double.Parse(matrice[i, j + 1]);
                                }
                                

                            }
                            else if(j==3 && i!=0)
                            {
                                if (matrice[i, j + 1]!=null)
                                {
                                    double valore = double.Parse(matrice[i, j + 1]);
                                    if (valore < 0)
                                        datiLavorati.Add(matrice[i, j], double.Parse(matrice[i, j + 1]));
                                }
                                
                            }
                        }
                        
                    }
                }
                double tot = 0;
                // aggiungo un valore a datiLavorati rappresentante il totale dei valori
                foreach(var v in datiLavorati)
                {
                    tot+=v.Value;
                }
                

                datiLavorati.Add("totale", tot);
                

            }

            return datiLavorati;
        }
    }
}
