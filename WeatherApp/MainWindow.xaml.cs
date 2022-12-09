using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
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
        Poniedziałek,
        Wtorek,
        Środa,
        Czwartek,
        Piątek,
        Sobota,
        Niedziela
    }

    public partial class MainWindow : Window
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();
        static SpeechSynthesizer ss;
        static SpeechRecognitionEngine sre;
        DzienTygodnia dzienTygodnia;
        





        public MainWindow()
        {
            ss = new SpeechSynthesizer();
            sre = new SpeechRecognitionEngine();
            InitializeComponent();
            ss.SetOutputToDefaultAudioDevice();
            ss.Speak("Witamy w naszej stacji pogodowej");
            CultureInfo ci = new CultureInfo("pl-PL");
            sre = new SpeechRecognitionEngine(ci);
            worker.DoWork += Worker_DoWork; 
            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //this.Dispatcher.BeginInvoke(new Action(() => { Ulgowy.Visibility = Visibility.Visible; Normalny.Visibility = Visibility.Hidden; }));

            sre.SpeechRecognized += Sre_SpeechRecognized;
            sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected;

            Choices pogoda_type = new Choices();
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

            GrammarBuilder grammar_stop_builder = new GrammarBuilder();
            grammar_stop_builder.Append(pogoda_type);
            Grammar stop_grammar = new Grammar(grammar_stop_builder);
            sre.LoadGrammarAsync(stop_grammar);
            ss.Speak("Powiedz nam dla jakiego miasta i w którym dniu tygodnia chcesz poznać pogodę");
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
                if(txt.IndexOf("Poniedziałek") > 0)
                {

                }
            }
        }
    }
}
