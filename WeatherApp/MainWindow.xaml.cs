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

            Grammar stop_grammar = new Grammar(".\\Grammar\\Grammar.xml");
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

            if(confidence > 0.5)
            {
                string mst = e.Result.Semantics["Miasto"].Value.ToString();
                string dzn = e.Result.Semantics["Dzien"].Value.ToString();

               
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
            Grammar stop_grammar = new Grammar(".\\Grammar\\Grammar.xml");
            stop_grammar.Enabled = true;
            sre.LoadGrammar(stop_grammar);
            sre.SpeechRecognized += Sre_SpeechRecognized;
            sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected;
            ss.Speak("Powiedz nam dla jakiego miasta i w którym dniu tygodnia chcesz poznać pogodę");
        }
    }
}
