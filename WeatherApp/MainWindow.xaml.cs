using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Threading;
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
            panel.Visibility = Visibility.Hidden;

            //worker.DoWork += Worker_DoWork; 
            //worker.RunWorkerAsync();

            client = new HttpClient();
            locations = new Dictionary<Miasto, Locate>();
            UpdateDictionary();
            //FakeInvoke();

            new Task(async() =>
            {
                ss.SetOutputToDefaultAudioDevice();
                ss.Speak("Witamy w naszej stacji pogodowej");
                CultureInfo ci = new CultureInfo("pl-PL");
                sre = new SpeechRecognitionEngine(ci);
                sre.SetInputToDefaultAudioDevice();

                sre.SpeechRecognized += Sre_SpeechRecognized;
                sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected;

                Grammar stop_grammar = new Grammar(".\\Grammar\\MainGrammar.xml");
                stop_grammar.Enabled = true;
                sre.LoadGrammar(stop_grammar);
                ss.Speak("Powiedz nam dla jakiego miasta i w którym dniu tygodnia chcesz poznać pogodę");
                sre.RecognizeAsync(RecognizeMode.Multiple);
            }).Start();
            
        }

        private void FakeInvoke()
        {
            dzienTygodnia = DzienTygodnia.Piątek;
            miasto = Miasto.Warszawa;

            GetWeather();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            ss.SetOutputToDefaultAudioDevice();
            ss.Speak("Witamy w naszej stacji pogodowej");
            CultureInfo ci = new CultureInfo("pl-PL");
            sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();

            sre.SpeechRecognized += Sre_SpeechRecognized;
            sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected;

            Grammar stop_grammar = new Grammar(".\\Grammar\\MainGrammar.xml");
            stop_grammar.Enabled = true;
            sre.LoadGrammar(stop_grammar);
            ss.Speak("Powiedz nam dla jakiego miasta i w którym dniu tygodnia chcesz poznać pogodę");
            sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void Sre_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            ss.Speak("Nie rozumniem. Powiedz nam dla jakiego miasta i w którym dniu tygodnia chcesz poznać pogodę");
        }

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            float confidence = e.Result.Confidence;

            if (confidence > 0.5)
            {
                string frst = e.Result.Semantics["First"].Value.ToString();
                string scnd = e.Result.Semantics["Second"].Value.ToString();



                if ("Dziś" == frst)
                    dzienTygodnia = (DzienTygodnia)(int)DateTime.Now.DayOfWeek;
                else if ("Jutro" == frst)
                    dzienTygodnia = (DzienTygodnia)(int)DateTime.Now.AddDays(1).DayOfWeek;
                else if ("Pojutrze" == frst)
                    dzienTygodnia = (DzienTygodnia)(int)DateTime.Now.AddDays(2).DayOfWeek;
                else if ("Poniedziałek" == frst)
                    dzienTygodnia = DzienTygodnia.Poniedziałek;
                else if ("Wtorek" == frst)
                    dzienTygodnia = DzienTygodnia.Wtorek;
                else if ("Środa" == frst)
                    dzienTygodnia = DzienTygodnia.Środa;
                else if ("Czwartek" == frst)
                    dzienTygodnia = DzienTygodnia.Czwartek;
                else if ("Piątek" == frst)
                    dzienTygodnia = DzienTygodnia.Piątek;
                else if ("Sobota" == frst)
                    dzienTygodnia = DzienTygodnia.Sobota;
                else if ("Niedziela" == frst)
                    dzienTygodnia = DzienTygodnia.Niedziela;
                else if ("Białystok" == frst)
                    miasto = Miasto.Białystok;
                else if ("Bydgoszcz" == frst)
                    miasto = Miasto.Bydgoszcz;
                else if ("Gdańsk" == frst)
                    miasto = Miasto.Gdańsk;
                else if ("Gorzów Wielkopolski" == frst)
                    miasto = Miasto.GorzówWielkopolski;
                else if ("Katowice" == frst)
                    miasto = Miasto.Katowice;
                else if ("Kielce" == frst)
                    miasto = Miasto.Kielce;
                else if ("Kraków" == frst)
                    miasto = Miasto.Kraków;
                else if ("Lublin" == frst)
                    miasto = Miasto.Lublin;
                else if ("Łódź" == frst)
                    miasto = Miasto.Łódź;
                else if ("Olsztyn" == frst)
                    miasto = Miasto.Olsztyn;
                else if ("Opole" == frst)
                    miasto = Miasto.Opole;
                else if ("Poznań" == frst)
                    miasto = Miasto.Poznań;
                else if ("Rzeszów" == frst)
                    miasto = Miasto.Rzeszów;
                else if ("Szczecin" == frst)
                    miasto = Miasto.Szczecin;
                else if ("Toruń" == frst)
                    miasto = Miasto.Toruń;
                else if ("Warszawa" == frst)
                    miasto = Miasto.Warszawa;
                else if ("Wrocław" == frst)
                    miasto = Miasto.Wrocław;
                else if ("Zielona Góra" == frst)
                    miasto = Miasto.ZielonaGóra;

                if ("Dziś" == scnd)
                    dzienTygodnia = (DzienTygodnia)(int)DateTime.Now.DayOfWeek;
                else if ("Jutro" == scnd)
                    dzienTygodnia = (DzienTygodnia)(int)DateTime.Now.AddDays(1).DayOfWeek;
                else if ("Pojutrze" == scnd)
                    dzienTygodnia = (DzienTygodnia)(int)DateTime.Now.AddDays(2).DayOfWeek;
                else if ("Poniedziałek" == scnd)
                    dzienTygodnia = DzienTygodnia.Poniedziałek;
                else if ("Wtorek" == scnd)
                    dzienTygodnia = DzienTygodnia.Wtorek;
                else if ("Środa" == scnd)
                    dzienTygodnia = DzienTygodnia.Środa;
                else if ("Czwartek" == scnd)
                    dzienTygodnia = DzienTygodnia.Czwartek;
                else if ("Piątek" == scnd)
                    dzienTygodnia = DzienTygodnia.Piątek;
                else if ("Sobota" == scnd)
                    dzienTygodnia = DzienTygodnia.Sobota;
                else if ("Niedziela" == scnd)
                    dzienTygodnia = DzienTygodnia.Niedziela;
                else if ("Białystok" == scnd)
                    miasto = Miasto.Białystok;
                else if ("Bydgoszcz" == scnd)
                    miasto = Miasto.Bydgoszcz;
                else if ("Gdańsk" == scnd)
                    miasto = Miasto.Gdańsk;
                else if ("Gorzów Wielkopolski" == scnd)
                    miasto = Miasto.GorzówWielkopolski;
                else if ("Katowice" == scnd)
                    miasto = Miasto.Katowice;
                else if ("Kielce" == scnd)
                    miasto = Miasto.Kielce;
                else if ("Kraków" == scnd)
                    miasto = Miasto.Kraków;
                else if ("Lublin" == scnd)
                    miasto = Miasto.Lublin;
                else if ("Łódź" == scnd)
                    miasto = Miasto.Łódź;
                else if ("Olsztyn" == scnd)
                    miasto = Miasto.Olsztyn;
                else if ("Opole" == scnd)
                    miasto = Miasto.Opole;
                else if ("Poznań" == scnd)
                    miasto = Miasto.Poznań;
                else if ("Rzeszów" == scnd)
                    miasto = Miasto.Rzeszów;
                else if ("Szczecin" == scnd)
                    miasto = Miasto.Szczecin;
                else if ("Toruń" == scnd)
                    miasto = Miasto.Toruń;
                else if ("Warszawa" == scnd)
                    miasto = Miasto.Warszawa;
                else if ("Wrocław" == scnd)
                    miasto = Miasto.Wrocław;
                else if ("Zielona Góra" == scnd)
                    miasto = Miasto.ZielonaGóra;

            }

            if (miasto != Miasto.Brak && dzienTygodnia != DzienTygodnia.Brak)
            {
                sre.SpeechRecognized -= Sre_SpeechRecognized;
                sre.SpeechRecognitionRejected -= Sre_SpeechRecognitionRejected;
                sre.UnloadAllGrammars();

                Grammar stop_grammar = new Grammar(".\\Grammar\\ConfirmGrammar.xml");
                stop_grammar.Enabled = true;
                sre.LoadGrammar(stop_grammar);

                sre.SpeechRecognized += Sre_SpeechRecognized_potw;
                sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected_potw;
                ss.Speak("Twój wybór to " + miasto + ", pogoda na " + dzienTygodnia + "Potwierdz wprowadzone informacje");

            }
            else if (miasto == Miasto.Brak)
            {
                sre.SpeechRecognized -= Sre_SpeechRecognized;
                sre.SpeechRecognitionRejected -= Sre_SpeechRecognitionRejected;
                sre.UnloadAllGrammars();

                Grammar stop_grammar = new Grammar(".\\Grammar\\CityGrammar.xml");
                stop_grammar.Enabled = true;
                sre.LoadGrammar(stop_grammar);

                sre.SpeechRecognized += Sre_SpeechRecognized_city;
                sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected_city;

                ss.Speak("Dla jakiego miasta chcesz usłyszeć pogodę?");
            }
            else if (dzienTygodnia == DzienTygodnia.Brak)
            {
                sre.SpeechRecognized -= Sre_SpeechRecognized;
                sre.SpeechRecognitionRejected -= Sre_SpeechRecognitionRejected;
                sre.UnloadAllGrammars();

                Grammar stop_grammar = new Grammar(".\\Grammar\\DayGrammar.xml");
                stop_grammar.Enabled = true;
                sre.LoadGrammar(stop_grammar);

                sre.SpeechRecognized += Sre_SpeechRecognized_day;
                sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected_day;

                ss.Speak("Dla jakiego dnia chcesz usłyszeć pogodę?");
            }
        }

        private void Sre_SpeechRecognitionRejected_city(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            ss.Speak("Nie rozumiem, dla jakiego miasta chcesz usłyszeć pogodę?");
        }

        private void Sre_SpeechRecognized_city(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            float confidence = e.Result.Confidence;

            if (confidence > 0.5)
            {
                string frst = e.Result.Semantics["First"].Value.ToString();

                if ("Białystok" == frst)
                    miasto = Miasto.Białystok;
                else if ("Bydgoszcz" == frst)
                    miasto = Miasto.Bydgoszcz;
                else if ("Gdańsk" == frst)
                    miasto = Miasto.Gdańsk;
                else if ("Gorzów Wielkopolski" == frst)
                    miasto = Miasto.GorzówWielkopolski;
                else if ("Katowice" == frst)
                    miasto = Miasto.Katowice;
                else if ("Kielce" == frst)
                    miasto = Miasto.Kielce;
                else if ("Kraków" == frst)
                    miasto = Miasto.Kraków;
                else if ("Lublin" == frst)
                    miasto = Miasto.Lublin;
                else if ("Łódź" == frst)
                    miasto = Miasto.Łódź;
                else if ("Olsztyn" == frst)
                    miasto = Miasto.Olsztyn;
                else if ("Opole" == frst)
                    miasto = Miasto.Opole;
                else if ("Poznań" == frst)
                    miasto = Miasto.Poznań;
                else if ("Rzeszów" == frst)
                    miasto = Miasto.Rzeszów;
                else if ("Szczecin" == frst)
                    miasto = Miasto.Szczecin;
                else if ("Toruń" == frst)
                    miasto = Miasto.Toruń;
                else if ("Warszawa" == frst)
                    miasto = Miasto.Warszawa;
                else if ("Wrocław" == frst)
                    miasto = Miasto.Wrocław;
                else if ("Zielona Góra" == frst)
                    miasto = Miasto.ZielonaGóra;
            }

            if (miasto != Miasto.Brak && dzienTygodnia != DzienTygodnia.Brak)
            {
                sre.SpeechRecognized -= Sre_SpeechRecognized_city;
                sre.SpeechRecognitionRejected -= Sre_SpeechRecognitionRejected_city;
                sre.UnloadAllGrammars();

                Grammar stop_grammar = new Grammar(".\\Grammar\\ConfirmGrammar.xml");
                stop_grammar.Enabled = true;
                sre.LoadGrammar(stop_grammar);

                sre.SpeechRecognized += Sre_SpeechRecognized_potw;
                sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected_potw;
                ss.Speak("Twój wybór to " + miasto + ", pogoda na " + dzienTygodnia + "Potwierdz wprowadzone informacje");

            }
            else if (miasto == Miasto.Brak)
            {
                ss.Speak("Dla jakiego miasta chcesz usłyszeć pogodę?");
            }
            else if (dzienTygodnia == DzienTygodnia.Brak)
            {
                sre.SpeechRecognized -= Sre_SpeechRecognized_city;
                sre.SpeechRecognitionRejected -= Sre_SpeechRecognitionRejected_city;
                sre.UnloadAllGrammars();

                Grammar stop_grammar = new Grammar(".\\Grammar\\DayGrammar.xml");
                stop_grammar.Enabled = true;
                sre.LoadGrammar(stop_grammar);

                sre.SpeechRecognized += Sre_SpeechRecognized_day;
                sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected_day;

                ss.Speak("Dla jakiego dnia chcesz usłyszeć pogodę?");
            }
        }

        private void Sre_SpeechRecognitionRejected_day(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            ss.Speak("Nie rozumiem, dla jakiego dnia chcesz usłyszeć pogodę?");
        }

        private void Sre_SpeechRecognized_day(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            float confidence = e.Result.Confidence;

            if (confidence > 0.5)
            {
                string frst = e.Result.Semantics["First"].Value.ToString();


                if ("Dziś" == frst)
                    dzienTygodnia = (DzienTygodnia)(int)DateTime.Now.DayOfWeek;
                else if ("Jutro" == frst)
                    dzienTygodnia = (DzienTygodnia)(int)DateTime.Now.AddDays(1).DayOfWeek;
                else if ("Pojutrze" == frst)
                    dzienTygodnia = (DzienTygodnia)(int)DateTime.Now.AddDays(2).DayOfWeek;
                else if ("Poniedziałek" == frst)
                    dzienTygodnia = DzienTygodnia.Poniedziałek;
                else if ("Wtorek" == frst)
                    dzienTygodnia = DzienTygodnia.Wtorek;
                else if ("Środa" == frst)
                    dzienTygodnia = DzienTygodnia.Środa;
                else if ("Czwartek" == frst)
                    dzienTygodnia = DzienTygodnia.Czwartek;
                else if ("Piątek" == frst)
                    dzienTygodnia = DzienTygodnia.Piątek;
                else if ("Sobota" == frst)
                    dzienTygodnia = DzienTygodnia.Sobota;
                else if ("Niedziela" == frst)
                    dzienTygodnia = DzienTygodnia.Niedziela;
            }

            if (miasto != Miasto.Brak && dzienTygodnia != DzienTygodnia.Brak)
            {
                sre.SpeechRecognized -= Sre_SpeechRecognized_day;
                sre.SpeechRecognitionRejected -= Sre_SpeechRecognitionRejected_day;
                sre.UnloadAllGrammars();

                Grammar stop_grammar = new Grammar(".\\Grammar\\ConfirmGrammar.xml");
                stop_grammar.Enabled = true;
                sre.LoadGrammar(stop_grammar);

                sre.SpeechRecognized += Sre_SpeechRecognized_potw;
                sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected_potw;

                ss.Speak("Twój wybór to " + miasto + ", pogoda na " + dzienTygodnia + "Potwierdz wprowadzone informacje");

            }
            else if (miasto == Miasto.Brak)
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
                string frst = e.Result.Semantics["First"].Value.ToString();

                if ("Tak" == frst)
                {
                    GetWeather();
                }
                else if ("Nie" == frst)
                {
                    ClearSettings();
                }
                else
                    ss.Speak("Nie rozumiem, potwierdz informacje");
            }
            else
                ss.Speak("Nie rozumiem, potwierdz informacje");
        }

        private async void GetWeather()
        {
            string txt = "";

            try
            {
                Locate loc = locations[miasto];
                string path = "http://api.weatherapi.com/v1/forecast.json?key=181d5bb3b0404ddfa28214722221312&q=" + loc.lat.ToString().Replace(',', '.') 
                    + "," + loc.lon.ToString().Replace(',', '.') + "&days=7&aqi=no&alerts=no";

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
                    ss.SpeakAsync("Zbyt odległą data aby pobrać pogodę, możesz usłyszeć podogę maksymalnie na dwa dni do przodu");
                }
                else
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        label_city.Content = "Miasto: " + miasto;
                        panel.Visibility = Visibility.Visible;
                        SetMap();

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

                        string text = "Pogoda na " + dzienTygodnia + ", średzinia temperatura w dzień będzie wynosić ";
                        if (forecastDay.avgtemp_c != 0)
                        {
                            if (forecastDay.avgtemp_c > 0)
                                text += String.Join(" przecinek ", forecastDay.avgtemp_c.ToString().Split(',').ToArray().Select(x => x.Replace("0", "zero")).ToArray()) + " stopni celsjusza.";
                            else
                                text += String.Join(" przecinek ", (forecastDay.avgtemp_c * -1).ToString().Split(',').ToArray().Select(x => x.Replace("0", "zero")).ToArray()) + " stopni celsjusza.";
                        }
                        else
                            text += "zero stopni celsjusza.";

                        if (forecastDay.maxwind_kph > 0)
                            text += " Porywy wiatru do " + String.Join(" przecinek ", forecastDay.maxwind_kph.ToString().Split(',').ToArray().Select(x => x.Replace("0", "zero")).ToArray()) + " kilometrów na godzinę";
                        else
                            text += "Brak wiatru";

                        if (forecastDay.totalprecip_mm > 0)
                            text += ". Suma opadów wyniesie " + String.Join(" przecinek ", forecastDay.totalprecip_mm.ToString().Split(',').ToArray().Select(x => x.Replace("0", "zero")).ToArray()) + " milimetrów.";
                        else
                            text += " oraz brak opadów.";

                        txt = text;


                    }));
                    if (txt != "")
                        ss.Speak(txt);
                }

                //new Task(() => {
                //    if (txt != "")
                //        ss.SpeakAsync(txt);
                //}).Start();
            }
            catch (Exception)
            {
                ss.Speak("Przepraszamy, wystąpił błąd serwera");
            }

            ClearSettings();
        }

        private void ClearSettings()
        {
            miasto = Miasto.Brak;
            dzienTygodnia = DzienTygodnia.Brak;

            sre.SpeechRecognized -= Sre_SpeechRecognized_potw;
            sre.SpeechRecognitionRejected -= Sre_SpeechRecognitionRejected_potw;
            sre.UnloadAllGrammars();
            Grammar stop_grammar = new Grammar(".\\Grammar\\MainGrammar.xml");
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

        private void SetMap()
        {
            if (miasto == Miasto.Białystok)
            {
                label_bialystok.FontWeight = FontWeights.Bold;
                label_bialystok.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label_bialystok.FontWeight = FontWeights.Regular;
                label_bialystok.Foreground = new SolidColorBrush(Colors.Black);

            }
            if (miasto == Miasto.Bydgoszcz)
            {
                label_bydgoszcz.FontWeight = FontWeights.Bold;
                label_bydgoszcz.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label_bydgoszcz.FontWeight = FontWeights.Regular;
                label_bydgoszcz.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (miasto == Miasto.Gdańsk)
            {
                label_gdansk.FontWeight = FontWeights.Bold;
                label_gdansk.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label_gdansk.FontWeight = FontWeights.Regular;
                label_gdansk.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (miasto == Miasto.GorzówWielkopolski)
            {
                label_gorzow.FontWeight = FontWeights.Bold;
                label_gorzow.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label_gorzow.FontWeight = FontWeights.Regular;
                label_gorzow.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (miasto == Miasto.Katowice)
            {
                label_katowice.FontWeight = FontWeights.Bold;
                label_katowice.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label_katowice.FontWeight = FontWeights.Regular;
                label_katowice.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (miasto == Miasto.Kielce)
            {
                label_kielce.FontWeight = FontWeights.Bold;
                label_kielce.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label_kielce.FontWeight = FontWeights.Regular;
                label_kielce.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (miasto == Miasto.Kraków)
            {
                label_krakow.FontWeight = FontWeights.Bold;
                label_krakow.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label_krakow.FontWeight = FontWeights.Regular;
                label_krakow.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (miasto == Miasto.Lublin)
            {
                label_lublin.FontWeight = FontWeights.Bold;
                label_lublin.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label_lublin.FontWeight = FontWeights.Regular;
                label_lublin.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (miasto == Miasto.Łódź)
            {
                label_lodz.FontWeight = FontWeights.Bold;
                label_lodz.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label_lodz.FontWeight = FontWeights.Regular;
                label_lodz.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (miasto == Miasto.Olsztyn)
            {
                label_olsztyn.FontWeight = FontWeights.Bold;
                label_olsztyn.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label_olsztyn.FontWeight = FontWeights.Regular;
                label_olsztyn.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (miasto == Miasto.Opole)
            {
                label_opole.FontWeight = FontWeights.Bold;
                label_opole.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label_opole.FontWeight = FontWeights.Regular;
                label_opole.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (miasto == Miasto.Poznań)
            {
                label_poznan.FontWeight = FontWeights.Bold;
                label_poznan.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label_poznan.FontWeight = FontWeights.Regular;
                label_poznan.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (miasto == Miasto.Rzeszów)
            {
                label_rzeszow.FontWeight = FontWeights.Bold;
                label_rzeszow.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label_rzeszow.FontWeight = FontWeights.Regular;
                label_rzeszow.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (miasto == Miasto.Szczecin)
            {
                label_szczecin.FontWeight = FontWeights.Bold;
                label_szczecin.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label_szczecin.FontWeight = FontWeights.Regular;
                label_szczecin.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (miasto == Miasto.Toruń)
            {
                label_torun.FontWeight = FontWeights.Bold;
                label_torun.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label_torun.FontWeight = FontWeights.Regular;
                label_torun.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (miasto == Miasto.Warszawa)
            {
                label_warszawa.FontWeight = FontWeights.Bold;
                label_warszawa.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label_warszawa.FontWeight = FontWeights.Regular;
                label_warszawa.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (miasto == Miasto.Wrocław)
            {
                label_wroclaw.FontWeight = FontWeights.Bold;
                label_wroclaw.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label_wroclaw.FontWeight = FontWeights.Regular;
                label_wroclaw.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (miasto == Miasto.ZielonaGóra)
            {
                label_zielona.FontWeight = FontWeights.Bold;
                label_zielona.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label_zielona.FontWeight = FontWeights.Regular;
                label_zielona.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
    }
}
