using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        public MainWindow()
        {
            ss = new SpeechSynthesizer();
            sre = new SpeechRecognitionEngine();
            InitializeComponent();
            ss.SetOutputToDefaultAudioDevice();
            ss.Speak("Witamy w naszej stacji pogodowej");
            CultureInfo ci = new CultureInfo("pl-PL");
            sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();
            worker.DoWork += Worker_DoWork; 
            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //this.Dispatcher.BeginInvoke(new Action(() => { Ulgowy.Visibility = Visibility.Visible; Normalny.Visibility = Visibility.Hidden; }));

            sre.SpeechRecognized += Sre_SpeechRecognized;
            sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected;

            Choices pogoda_type = new Choices();
            {
                pogoda_type.Add("Poniedziałek");
                pogoda_type.Add("Wtorek");
                pogoda_type.Add("Środa");
                pogoda_type.Add("Czwartek");
                pogoda_type.Add("Piątek");
                pogoda_type.Add("Sobota");
                pogoda_type.Add("Niedziela");
                pogoda_type.Add("Białystok");
                pogoda_type.Add("Bydgoszcz");
                pogoda_type.Add("Gdańsk");
                pogoda_type.Add("Gorzów Wielkopolski");
                pogoda_type.Add("Katowice");
                pogoda_type.Add("Kielce");
                pogoda_type.Add("Kraków");
                pogoda_type.Add("Lublin");
                pogoda_type.Add("Łódź");
                pogoda_type.Add("Olsztyn");
                pogoda_type.Add("Opole");
                pogoda_type.Add("Poznań");
                pogoda_type.Add("Rzeszów");
                pogoda_type.Add("Szczecin");
                pogoda_type.Add("Toruń");
                pogoda_type.Add("Warszawa");
                pogoda_type.Add("Wrocław");
                pogoda_type.Add("Zielona Góra");
            }

            GrammarBuilder grammar_stop_builder = new GrammarBuilder();
            grammar_stop_builder.Append(pogoda_type);
            Grammar stop_grammar = new Grammar(grammar_stop_builder);
            sre.LoadGrammarAsync(stop_grammar);
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

            if(confidence > 0.5)
            {
                if(txt.IndexOf("Poniedziałek") >= 0)
                {
                    dzienTygodnia = DzienTygodnia.Poniedziałek;
                }
                if (txt.IndexOf("Wtorek") >= 0)
                {
                    dzienTygodnia = DzienTygodnia.Wtorek;
                }
                if (txt.IndexOf("Środa") >= 0)
                {
                    dzienTygodnia = DzienTygodnia.Środa;
                }
                if (txt.IndexOf("Czwartek") >= 0)
                {
                    dzienTygodnia = DzienTygodnia.Czwartek;
                }
                if (txt.IndexOf("Piątek") >= 0)
                {
                    dzienTygodnia = DzienTygodnia.Piątek;
                }
                if (txt.IndexOf("Sobota") >= 0)
                {
                    dzienTygodnia = DzienTygodnia.Sobota;
                }
                if (txt.IndexOf("Niedziela") >= 0)
                {
                    dzienTygodnia = DzienTygodnia.Niedziela;
                }


       

                if (txt.IndexOf("Białystok") >= 0)
                {
                    miasto = Miasto.Białystok;
                }
                if (txt.IndexOf("Bydgoszcz") >= 0)
                {
                    miasto = Miasto.Bydgoszcz;
                }
                if (txt.IndexOf("Gdańsk") >= 0)
                {
                    miasto = Miasto.Gdańsk;
                }
                if (txt.IndexOf("Gorzów Wielkopolski") >= 0)
                {
                    miasto = Miasto.GorzówWielkopolski;
                }
                if (txt.IndexOf("Katowice") >= 0)
                {
                    miasto = Miasto.Katowice;
                }
                if (txt.IndexOf("Kielce") >= 0)
                {
                    miasto = Miasto.Kielce;
                }
                if (txt.IndexOf("Kraków") >= 0)
                {
                    miasto = Miasto.Kraków;
                }
                if (txt.IndexOf("Lublin") >= 0)
                {
                    miasto = Miasto.Lublin;
                }
                if (txt.IndexOf("Łódź") >= 0)
                {
                    miasto = Miasto.Łódź;
                }
                if (txt.IndexOf("Olsztyn") >= 0)
                {
                    miasto = Miasto.Olsztyn;
                }
                if (txt.IndexOf("Opole") >= 0)
                {
                    miasto = Miasto.Opole;
                }
                if (txt.IndexOf("Poznań") >= 0)
                {
                    miasto = Miasto.Poznań;
                }
                if (txt.IndexOf("Rzeszów") >= 0)
                {
                    miasto = Miasto.Rzeszów;
                }
                if (txt.IndexOf("Szczecin") >= 0)
                {
                    miasto = Miasto.Szczecin;
                }
                if (txt.IndexOf("Toruń") >= 0)
                {
                    miasto = Miasto.Toruń;
                }
                if (txt.IndexOf("Warszawa") >= 0)
                {
                    miasto = Miasto.Warszawa;
                }
                if (txt.IndexOf("Wrocław") >= 0)
                {
                    miasto = Miasto.Wrocław;
                }
                if (txt.IndexOf("Zielona Góra") >= 0)
                {
                    miasto = Miasto.ZielonaGóra;
                }
            }

            if(miasto != Miasto.Brak && dzienTygodnia != DzienTygodnia.Brak)
            {
                sre.SpeechRecognized -= Sre_SpeechRecognized;
                sre.SpeechRecognitionRejected -= Sre_SpeechRecognitionRejected;
                sre.UnloadAllGrammars();
                Choices choices = new Choices();
                choices.Add("Tak");
                choices.Add("Nie");
                GrammarBuilder grammar_potw = new GrammarBuilder();
                grammar_potw.Append(choices);
                Grammar new_grammar_potw = new Grammar(grammar_potw);
                sre.LoadGrammar(new_grammar_potw);
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
                    GetWeather();
                }
                if (txt.IndexOf("Nie") >= 0)
                {
                    ClearSettings();
                }
            }
        }

        private void GetWeather()
        {
            ss.Speak("Podoga w Warszawie jest super");
            ClearSettings();

        }
        private void ClearSettings()
        {
            miasto = Miasto.Brak;
            dzienTygodnia = DzienTygodnia.Brak;

            sre.SpeechRecognized -= Sre_SpeechRecognized_potw;
            sre.SpeechRecognitionRejected -= Sre_SpeechRecognitionRejected_potw;
            sre.UnloadAllGrammars();
            Choices pogoda_type = new Choices();
            { 
                pogoda_type.Add("Poniedziałek");
                pogoda_type.Add("Wtorek");
                pogoda_type.Add("Środa");
                pogoda_type.Add("Czwartek");
                pogoda_type.Add("Piątek");
                pogoda_type.Add("Sobota");
                pogoda_type.Add("Niedziela");
                pogoda_type.Add("Białystok");
                pogoda_type.Add("Bydgoszcz");
                pogoda_type.Add("Gdańsk");
                pogoda_type.Add("Gorzów Wielkopolski");
                pogoda_type.Add("Katowice");
                pogoda_type.Add("Kielce");
                pogoda_type.Add("Kraków");
                pogoda_type.Add("Lublin");
                pogoda_type.Add("Łódź");
                pogoda_type.Add("Olsztyn");
                pogoda_type.Add("Opole");
                pogoda_type.Add("Poznań");
                pogoda_type.Add("Rzeszów");
                pogoda_type.Add("Szczecin");
                pogoda_type.Add("Toruń");
                pogoda_type.Add("Warszawa");
                pogoda_type.Add("Wrocław");
                pogoda_type.Add("Zielona Góra");
            }
            GrammarBuilder grammar_stop_builder = new GrammarBuilder();
            grammar_stop_builder.Append(pogoda_type);
            Grammar stop_grammar = new Grammar(grammar_stop_builder);
            sre.LoadGrammarAsync(stop_grammar);
            sre.SpeechRecognized += Sre_SpeechRecognized;
            sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected;
            ss.Speak("Powiedz nam dla jakiego miasta i w którym dniu tygodnia chcesz poznać pogodę");
        }
    }
}
