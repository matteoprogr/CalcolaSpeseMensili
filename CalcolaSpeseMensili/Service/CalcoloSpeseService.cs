namespace CalcolaSpeseMensili.Service;

public class CalcoloSpeseService
{
    public Dictionary<string, double> Calcolaspese(Dictionary<string, double> data)
    {
        Dictionary<string, double> pulisciChiave = new Dictionary<string, double>();

        foreach (var chiave in data)
        {
            string? presso = chiave.Key;
            if (chiave.Key.Contains("presso"))
            {
                int index = presso.IndexOf("presso");
                presso = presso.Substring(index);
            }

            if (pulisciChiave.Keys.Contains(presso))
            {
                double value = pulisciChiave.GetValueOrDefault(presso);
                pulisciChiave[presso] = chiave.Value + value;
            }else
            {
                pulisciChiave.Add(presso, chiave.Value);
            }
        }
        double tot = 0;
        // aggiungo un valore a datiLavorati rappresentante il totale dei valori
        foreach(var v in pulisciChiave)
        {
            tot+=v.Value;
        }
        pulisciChiave.Add("totale", tot);

        return pulisciChiave;
    }
}