using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WeatherApp.Models;
using static System.Net.Mime.MediaTypeNames;


namespace WeatherApp
{
    public enum DzienTygodnia
    {
        Brak,
        Poniedziałek,
        Wtorek,
        Środa,
        Czwartek,
        Piątek,
        Sobota,
        Niedziela
    }

    public enum Miasto
    {
        Brak,
        Białystok,
        Bydgoszcz,
        Gdańsk,
        [Description("Gorzów Wielkopolski")] GorzówWielkopolski,
        Katowice,
        Kielce,
        Kraków,
        Lublin,
        Łódź,
        Olsztyn,
        Opole,
        Poznań,
        Rzeszów,
        Szczecin,
        Toruń,
        Warszawa,
        Wrocław,
        [Description("Zielona Góra")] ZielonaGóra
    }

    public partial class MainWindow : Window
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();
        static SpeechSynthesizer ss;
        static SpeechRecognitionEngine sre;
        DzienTygodnia dzienTygodnia;
        Miasto miasto;
        static HttpClient client;

        Dictionary<Miasto, Locate> locations; 

        public MainWindow()
        {
            ss = new SpeechSynthesizer();
            sre = new SpeechRecognitionEngine();
            InitializeComponent();
            label_today.Content = "Dziś: " + DateTime.Now.ToString("dddd, dd MMMM yyyy");

            

            //ss.SetOutputToDefaultAudioDevice();
            //ss.Speak("Witamy w naszej stacji pogodowej");
            //CultureInfo ci = new CultureInfo("pl-PL");
            //sre = new SpeechRecognitionEngine(ci);
            //sre.SetInputToDefaultAudioDevice();
            //worker.DoWork += Worker_DoWork; 
            //worker.RunWorkerAsync();

            client = new HttpClient();
            locations = new Dictionary<Miasto, Locate>();
            UpdateDictionary();
            FakeInvoke();
        }

        private void FakeInvoke()
        {
            dzienTygodnia = DzienTygodnia.Piątek;
            miasto = Miasto.Warszawa;

            GetWeather();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(async () => {
                //icon_weather.Source = await LoadWebP(
                //    "http://cdn.weatherapi.com/weather/64x64/day/311.png");
            }));

            //sre.SpeechRecognized += Sre_SpeechRecognized;
            //sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected;

            //Grammar stop_grammar = new Grammar(".\\Grammar\\Grammar.xml");
            //stop_grammar.Enabled = true;
            //sre.LoadGrammar(stop_grammar);
            //ss.Speak("Powiedz nam dla jakiego miasta i w którym dniu tygodnia chcesz poznać pogodę");
            //sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void Sre_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            ss.Speak("Nie rozumniem. Powiedz nam dla jakiego miasta i w którym dniu tygodnia chcesz poznać pogodę");
        }

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            float confidence = e.Result.Confidence;

            if(confidence > 0.5)
            {
                string mst = e.Result.Semantics["First"].Value.ToString();
                string dzn = e.Result.Semantics["Second"].Value.ToString();

               
                if ("Poniedziałek" == dzn)
                {
                    dzienTygodnia = DzienTygodnia.Poniedziałek;
                }
                if ("Wtorek" == dzn)
                {
                    dzienTygodnia = DzienTygodnia.Wtorek;
                }
                if ("Środa" == dzn)
                {
                    dzienTygodnia = DzienTygodnia.Środa;
                }
                if ("Czwartek" == dzn)
                {
                    dzienTygodnia = DzienTygodnia.Czwartek;
                }
                if ("Piątek" == dzn)
                {
                    dzienTygodnia = DzienTygodnia.Piątek;
                }
                if ("Sobota" == dzn)
                {
                    dzienTygodnia = DzienTygodnia.Sobota;
                }
                if ("Niedziela" == dzn)
                {
                    dzienTygodnia = DzienTygodnia.Niedziela;
                }


       

                if ("Białystok" == mst)
                {
                    miasto = Miasto.Białystok;
                }
                if ("Bydgoszcz" == mst)
                {
                    miasto = Miasto.Bydgoszcz;
                }
                if ("Gdańsk" == mst)
                {
                    miasto = Miasto.Gdańsk;
                }
                if ("Gorzów Wielkopolski" == mst)
                {
                    miasto = Miasto.GorzówWielkopolski;
                }
                if ("Katowice" == mst)
                {
                    miasto = Miasto.Katowice;
                }
                if ("Kielce" == mst)
                {
                    miasto = Miasto.Kielce;
                }
                if ("Kraków" == mst)
                {
                    miasto = Miasto.Kraków;
                }
                if ("Lublin" == mst)
                {
                    miasto = Miasto.Lublin;
                }
                if ("Łódź" == mst)
                {
                    miasto = Miasto.Łódź;
                }
                if ("Olsztyn" == mst)
                {
                    miasto = Miasto.Olsztyn;
                }
                if ("Opole" == mst)
                {
                    miasto = Miasto.Opole;
                }
                if ("Poznań" == mst)
                {
                    miasto = Miasto.Poznań;
                }
                if ("Rzeszów" == mst)
                {
                    miasto = Miasto.Rzeszów;
                }
                if ("Szczecin" == mst)
                {
                    miasto = Miasto.Szczecin;
                }
                if ("Toruń" == mst)
                {
                    miasto = Miasto.Toruń;
                }
                if ("Warszawa" == mst)
                {
                    miasto = Miasto.Warszawa;
                }
                if ("Wrocław" == mst)
                {
                    miasto = Miasto.Wrocław;
                }
                if ("Zielona Góra" == mst)
                {
                    miasto = Miasto.ZielonaGóra;
                }
            }

            if(miasto != Miasto.Brak && dzienTygodnia != DzienTygodnia.Brak)
            {
                sre.SpeechRecognized -= Sre_SpeechRecognized;
                sre.SpeechRecognitionRejected -= Sre_SpeechRecognitionRejected;
                sre.UnloadAllGrammars();

                Grammar stop_grammar = new Grammar(".\\Grammar\\Grammar2.xml");
                stop_grammar.Enabled = true;
                sre.LoadGrammar(stop_grammar);

                sre.SpeechRecognized += Sre_SpeechRecognized_potw;
                sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected_potw;
                ss.Speak("Potwierdz wprowadzone informacje");

            }
            else if(miasto == Miasto.Brak)
            {
                ss.Speak("Dla jakiego miasta chcesz usłyszeć pogodę?");
            }
            else if (dzienTygodnia == DzienTygodnia.Brak)
            {
                ss.Speak("Dla jakiego dnia chcesz usłyszeć pogodę?");
            }
        }

        private void Sre_SpeechRecognitionRejected_potw(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            ss.Speak("Nie rozumiem, potwierdz informacje");
        }

        private void Sre_SpeechRecognized_potw(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            float confidence = e.Result.Confidence;

            if (confidence > 0.5)
            {
                if (txt.IndexOf("Tak") >= 0)
                {
                    //GetWeather();
                }
                if (txt.IndexOf("Nie") >= 0)
                {
                    ClearSettings();
                }
            }
        }

        private async void GetWeather()
        {
            label_city.Content = "Miasto: " + miasto;

            Locate loc = locations[miasto];
            string path = "http://api.weatherapi.com/v1/forecast.json?key=181d5bb3b0404ddfa28214722221312&q=" + loc.lat.ToString().Replace(',', '.') + "," + loc.lon.ToString().Replace(',', '.') + "&days=7&aqi=no&alerts=no";

            var json = await GetWeatherAsync(path);
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(json);

            DateTime date = DateTime.Now;
            int dayOfWeek = (int)date.DayOfWeek;

            int dz = ((int)dzienTygodnia);

            int distant = dz - dayOfWeek;

            if (distant < 0)
                distant += 7;

            if (distant > 2)
            {
                //za odległa data
            }
            else
            {
                List<Forecastday> forecast = myDeserializedClass.forecast.forecastday;
                var forecastDay = forecast[distant].day;

                DateTime dateWeather = DateTime.ParseExact(forecast[distant].date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                label_date.Content = "Data: " + dateWeather.ToString("dddd, dd MMMM yyyy");

                label_temp.Content = "Średnia temperatura: " + forecastDay.avgtemp_c + "℃";
                label_temp_max.Content = "Maksymalna temperatura: " + forecastDay.maxtemp_c + "℃";
                label_temp_min.Content = "Minimalna temperatura: " + forecastDay.mintemp_c + "℃";

                label_wiatr.Content = "Wiatr: " + forecastDay.maxwind_kph + "km/h";
                label_opad.Content = "Opady: " + forecastDay.totalprecip_mm + "mm";

                LoadIcon(forecastDay.condition.icon);
            }


            //ClearSettings();
        }

        private void ClearSettings()
        {
            miasto = Miasto.Brak;
            dzienTygodnia = DzienTygodnia.Brak;

            sre.SpeechRecognized -= Sre_SpeechRecognized_potw;
            sre.SpeechRecognitionRejected -= Sre_SpeechRecognitionRejected_potw;
            sre.UnloadAllGrammars();
            Grammar stop_grammar = new Grammar(".\\Grammar\\Grammar.xml");
            stop_grammar.Enabled = true;
            sre.LoadGrammar(stop_grammar);
            sre.SpeechRecognized += Sre_SpeechRecognized;
            sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected;
            ss.Speak("Powiedz nam dla jakiego miasta i w którym dniu tygodnia chcesz poznać pogodę");
        }

        private void UpdateDictionary()
        {
            locations[Miasto.Białystok] = new Locate(53.105010, 23.167102);
            locations[Miasto.Bydgoszcz] = new Locate(53.117735, 18.007736);
            locations[Miasto.Gdańsk] = new Locate(54.343229, 18.628444);
            locations[Miasto.GorzówWielkopolski] = new Locate(52.720899, 15.251780);
            locations[Miasto.Katowice] = new Locate(50.249859, 19.017038);
            locations[Miasto.Kielce] = new Locate(50.851569, 20.621034);
            locations[Miasto.Kraków] = new Locate(50.056293, 19.944799);
            locations[Miasto.Lublin] = new Locate(51.230485, 22.570839);
            locations[Miasto.Łódź] = new Locate(51.750786, 19.459598);
            locations[Miasto.Olsztyn] = new Locate(53.770330, 20.485700);
            locations[Miasto.Opole] = new Locate(50.656383, 17.944921);
            locations[Miasto.Poznań] = new Locate(52.383711, 16.920281);
            locations[Miasto.Rzeszów] = new Locate(50.000451, 21.999156);
            locations[Miasto.Szczecin] = new Locate(53.408263, 14.541918);
            locations[Miasto.Toruń] = new Locate(53.003502, 18.627307);
            locations[Miasto.Warszawa] = new Locate(52.199253, 21.022641);
            locations[Miasto.Wrocław] = new Locate(51.085213, 17.048414);
            locations[Miasto.ZielonaGóra] = new Locate(51.922847, 15.498385);
        }

        static async Task<string> GetWeatherAsync(string path)
        {
            string ss = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                ss = await response.Content.ReadAsStringAsync();
            }
            return ss;
        }
        private void LoadIcon(string url)
        {
            // Create source.
            BitmapImage bi = new BitmapImage();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            bi.UriSource = new Uri("http:" + url, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            // Set the image source.
            icon_weather.Source = bi;
        }
    }
}
