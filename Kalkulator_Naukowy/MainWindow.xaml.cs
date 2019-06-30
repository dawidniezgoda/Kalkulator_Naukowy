using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kalkulator_Naukowy
{
    /// <summary>
    /// Interaction logic with MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Rozmiar pamięci. Maksymalny rozmiar liczby jaka zostanie wyświetlona w pamięci.
        const int dlugoscPamieci = 6;
        //Rozmiar czcionki w TextBoxie Wynik.
        const int rozmiarCzcionki = 36;

        //Zmienna ustawiana na wartość true, jeśli wykonywane jest działanie
        bool operacjaBool;
        //Zmienna ustawiana na wartość true, jeśli została wywołana funkcja SIN COS TAN LN lub LOG.
        bool funkcjaBool;
        //Zmienna ustawiana na wartość true, jeśli ma zostać wyczyszczony TextBox Wynik po wprowadzeniu liczby.
        bool zerujBool;
        //Zmienna ustawiana na wartość true, jeśli w TextBoxie Wynik jest rezultat jakiegoś działania.
        bool rezultatBool;
        //Zmienna ustawiana na wartość true, jeśli pole tekstowe Wynik nie zostało zmienione po naciśnięciu operacji.
        bool staryTekstBool;
        //Przechowywana liczba w pamięci.
        double pamiec = 0;
        //Przechowuje tekst po wybraniu nowej operacji matematycznej.
        string poprzedniTekst;

        // Komunikaty o błędach
        static string NADMIAR = "Nadmiar!";
        static string BLAD = "Błąd!";
        static string NIE_LICZBA = "NaN!";
        string[] bledy = { NADMIAR, BLAD, NIE_LICZBA };

        operacje operacja = operacje.BRAK;

        //Typ wyliczeniowy zawierający podstawowe operacje matematyczne.
        enum operacje
        {
            DODAWANIE,
            ODEJMOWANIE,
            DZIELENIE,
            MNOZENIE,
            POTEGA,
            BRAK
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Wyświetla tekst podany jako parametr w TextBoxie Wynik
        /// </summary>
        /// <param name="tekst">Łańcuch znaków przechowujący wynik operacji.</param>
        /// <param name="c">Typ boolean przechowujący informację o tym, czy ma być wyczyszczony ekran po wprowadzeniu liczby. Domyślna wartość to prawda.</param>
        private void wyswietlWynik(string tekst, bool c = true)
        {
            try
            {
                if (double.Parse(tekst) == 0)
                    tekst = "0";
            }
            catch (Exception)
            {
                pokazBlad(BLAD);
                return;
            }

            if (tekst.Length > 30)
                return;
            if (tekst.Length > 14)
                Wynik.FontSize = 24;
            if (tekst.Length > 21)
                Wynik.FontSize = 18;

            zerujBool = c;
            Wynik.Text = tekst;
        }

        /// <summary>
        /// Jeżeli wystąpi błąd to zostaje wyświetlany na ekran.
        /// </summary>
        /// <param name="tekst">Łańcuch znaków przechowujący przechowujący informację o błędzie.</param>
        private void pokazBlad(string tekst)
        {
            Wynik.Text = tekst;
            poprzedniTekst = null;
            operacjaBool = false;
            zerujBool = true;
            zmienRownanie("");
            operacja = operacje.BRAK;
            zresetujRozmiarCzcionki();
        }

        /// <summary>
        /// Aktualizuje TextBox Równanie jeżeli dołączane równanie jest prawdziwe
        /// </summary>
        /// <param name="rownanie">Łańcuch znaków przechowujący równanie.</param>
        /// <param name="dodatek">Typ boolean informujący o tym czy równanie jest częścią dołączoną do innego równania. Domyślnie jest to fałsz.</param>
        private void zmienRownanie(string rownanie, bool dodatek = false)
        {
            if (rownanie.Length > 10)
                Rownanie.FontSize = 18;

            if (!dodatek)
                Rownanie.Text = rownanie;
            else
                Rownanie.Text += rownanie;
        }

        /// <summary>
        /// Modyfikowanie pamięci.
        /// </summary>
        private void zmienPamiec()
        {
            Pamiec.Content = pamiec.ToString();
            if (Pamiec.Content.ToString().Length > dlugoscPamieci)
                Pamiec.Content = Pamiec.Content.ToString().Substring(0, 5) + "...";
        }

        /// <summary>
        /// Parsowanie stringa na double i zwracanie go.
        /// </summary>
        private double zwrocLiczbe()
        {
            double liczba = double.Parse(Wynik.Text);
            return liczba;
        }

        /// <summary>
        /// Resetowanie rozmiaru czcionki.
        /// </summary>
        private void zresetujRozmiarCzcionki()
        {
            Wynik.FontSize = rozmiarCzcionki;
        }

        /// <summary>
        /// Obliczanie wyniku
        /// </summary>
        private void oblicz()
        {
            if (operacja == operacje.BRAK)
                return;

            double pierwszaLiczba = double.Parse(poprzedniTekst);
            double drugaLiczba = double.Parse(Wynik.Text);
            double wynik;

            switch (operacja)
            {
                case operacje.DZIELENIE:
                    wynik = pierwszaLiczba / drugaLiczba;
                    break;
                case operacje.MNOZENIE:
                    wynik = pierwszaLiczba * drugaLiczba;
                    break;
                case operacje.DODAWANIE:
                    wynik = pierwszaLiczba + drugaLiczba;
                    break;
                case operacje.ODEJMOWANIE:
                    wynik = pierwszaLiczba - drugaLiczba;
                    break;
                case operacje.POTEGA:
                    wynik = Math.Pow(pierwszaLiczba, drugaLiczba);
                    break;
                default:
                    return;
            }

            if (bledy.Contains(Wynik.Text))
                return;

            operacjaBool = false;
            poprzedniTekst = null;
            string rownanie;
            if (!funkcjaBool)
                rownanie = Rownanie.Text + drugaLiczba.ToString();
            else
            {
                rownanie = Rownanie.Text;
                funkcjaBool = false;
            }
            zmienRownanie(rownanie);
            wyswietlWynik(wynik.ToString());
            operacja = operacje.BRAK;
            rezultatBool = true;
        }

        /// <summary>
        /// Pobieranie cyfry od 0 do 9
        /// </summary>
        private void liczba_Click(object sender, RoutedEventArgs e)
        {
            rezultatBool = false;
            Button przycisk = (Button)sender;

            if (Wynik.Text == "0" || bledy.Contains(Wynik.Text))
                Wynik.Clear();

            string tekst;

            if (zerujBool)
            {
                zresetujRozmiarCzcionki();
                tekst = przycisk.Content.ToString();
                staryTekstBool = false;
            }
            else
                tekst = Wynik.Text + przycisk.Content.ToString();

            if (!operacjaBool && Rownanie.Text != "")
                zmienRownanie("");
            wyswietlWynik(tekst, false);
        }

        /// <summary>
        /// Działania negacji, pierwiastka, silni i logarytmów
        /// </summary>
        private void funkcja_Click(object sender, RoutedEventArgs e)
        {
            if (bledy.Contains(Wynik.Text))
                return;

            Button przycisk = (Button)sender;
            string przyciskTekst = przycisk.Content.ToString();
            double liczba = zwrocLiczbe();
            string rownanie = "";
            string wynik = "";

            switch (przyciskTekst)
            {
                case "!":
                    if (liczba < 0 || liczba.ToString().Contains("."))
                    {
                        pokazBlad(BLAD);
                        return;
                    }

                    if (liczba > 3248)
                    {
                        pokazBlad(NADMIAR);
                        return;
                    }
                    double wyn = 1;
                    if (liczba == 1 || liczba == 0)
                        wynik = wyn.ToString();
                    else
                    {
                        for (int i = 2; i <= liczba; i++)
                        {
                            wyn *= i;
                        }
                    }
                    rownanie = "!(" + liczba.ToString() + ")";
                    wynik = wyn.ToString();
                    break;

                case "ln":
                    rownanie = "ln(" + liczba + ")";
                    wynik = Math.Log(liczba).ToString();
                    break;

                case "log":
                    rownanie = "log(" + liczba + ")";
                    wynik = Math.Log10(liczba).ToString();
                    break;

                case "√":
                    rownanie = "√(" + liczba + ")";
                    wynik = Math.Sqrt(liczba).ToString();
                    break;

                case "-n":
                    rownanie = "-(" + liczba + ")";
                    wynik = decimal.Negate((decimal)liczba).ToString();
                    break;
            }

            if (operacjaBool)
            {
                rownanie = Rownanie.Text + rownanie;
                funkcjaBool = true;
            }

            zmienRownanie(rownanie);
            wyswietlWynik(wynik);
        }

        /// <summary>
        /// Obliczanie funkcji trygonometrycznych
        /// </summary>
        private void funkcjeTrygonometryczne_Click(object sender, RoutedEventArgs e)
        {
            if (bledy.Contains(Wynik.Text))
                return;

            Button przycisk = (Button)sender;
            string przyciskTekst = przycisk.Content.ToString();
            string rownanie = "";
            string wynik = "";
            double liczba = zwrocLiczbe();
            switch (przyciskTekst)
            {
                case "sin":
                    rownanie = "sin(" + liczba.ToString() + ")";
                    wynik = Math.Sin(liczba).ToString();
                    break;

                case "cos":
                    rownanie = "cos(" + liczba.ToString() + ")";
                    wynik = Math.Cos(liczba).ToString();
                    break;

                case "tan":
                    rownanie = "tan(" + liczba.ToString() + ")";
                    wynik = Math.Tan(liczba).ToString();
                    break;
            }
            wyswietlWynik(wynik);
        }

        /// <summary>
        /// Pobieranie podstawowych operacji: dodawanie, odejmowanie, dzielenie, mnożenie, potęgowanie
        /// </summary>
        private void podstawoweOperacje_Click(object sender, RoutedEventArgs e)
        {
            if (bledy.Contains(Wynik.Text))
                return;

            if (operacjaBool && !staryTekstBool)
                oblicz();

            Button przycisk = (Button)sender;
            operacjaBool = true;
            poprzedniTekst = Wynik.Text;
            string przyciskTekst = przycisk.Content.ToString();
            string rownanie = poprzedniTekst + " " + przyciskTekst + " ";
            switch (przyciskTekst)
            {
                case "/":
                    operacja = operacje.DZIELENIE;
                    break;
                case "x":
                    operacja = operacje.MNOZENIE;
                    break;
                case "-":
                    operacja = operacje.ODEJMOWANIE;
                    break;
                case "+":
                    operacja = operacje.DODAWANIE;
                    break;
                case "^":
                    operacja = operacje.POTEGA;
                    break;
            }
            zmienRownanie(rownanie);
            zresetujRozmiarCzcionki();
            wyswietlWynik(Wynik.Text);
            staryTekstBool = true;
        }

        /// <summary>
        /// Informacja o zmianie liczby na rzeczywistą.
        /// </summary>
        private void liczbaDecymalna_Click(object sender, RoutedEventArgs e)
        {
            if (!Wynik.Text.Contains("."))
            {
                string tekst = Wynik.Text += ".";
                wyswietlWynik(tekst, false);
            }
        }

        /// <summary>
        /// Wyświetlenie stałej PI
        /// </summary>
        private void przyciskPi_Click(object sender, RoutedEventArgs e)
        {
            if (!operacjaBool)
                zmienRownanie("");
            wyswietlWynik(Math.PI.ToString());
            rezultatBool = true;
        }

        /// <summary>
        /// Wyświetlenie stałej E
        /// </summary>
        private void przyciskE_Click(object sender, RoutedEventArgs e)
        {
            if (!operacjaBool)
                zmienRownanie("");
            wyswietlWynik(Math.E.ToString());
            rezultatBool = true;
        }

        /// <summary>
        /// Dodanie wartości do pamięci.
        /// </summary>
        private void przyciskMDodaj_Click(object sender, RoutedEventArgs e)
        {
            if (bledy.Contains(Wynik.Text))
                return;
            pamiec += zwrocLiczbe();
            zmienPamiec();
        }

        /// <summary>
        /// Odjęcie wartości z pamięci.
        /// </summary>
        private void przyciskModejmij_Click(object sender, RoutedEventArgs e)
        {
            if (bledy.Contains(Wynik.Text))
                return;
            pamiec -= zwrocLiczbe();
            zmienPamiec();
        }

        /// <summary>
        /// Resetowanie pamięci.
        /// </summary>
        private void przyciskMC_Click(object sender, RoutedEventArgs e)
        {
            pamiec = 0;
            zmienPamiec();
        }

        /// <summary>
        /// Wyświetlenie wartości z pamięci.
        /// </summary>
        private void przyciskMR_Click(object sender, RoutedEventArgs e)
        {
            wyswietlWynik(pamiec.ToString());
            if (!operacjaBool)
                zmienRownanie("");
        }

        /// <summary>
        /// Resetowanie do ustawień domyślnych.
        /// </summary>
        private void przyciskC_Click(object sender, RoutedEventArgs e)
        {
            Wynik.Text = "0";
            operacjaBool = false;
            poprzedniTekst = null;
            zmienRownanie("");
            zresetujRozmiarCzcionki();
        }

        /// <summary>
        /// Resetowanie aktualnej wartości.
        /// </summary>
        private void przyciskCe_Click(object sender, RoutedEventArgs e)
        {
            Wynik.Text = "0";
            zresetujRozmiarCzcionki();
        }

        /// <summary>
        /// Usuwanie ostatniego znaku.
        /// </summary>
        private void przyciskCofnij_Click(object sender, RoutedEventArgs e)
        {
            if (rezultatBool)
                return;

            string text;

            if (Wynik.Text.Length == 1)
                text = "0";
            else
                text = Wynik.Text.Substring(0, Wynik.Text.Length - 1);

            wyswietlWynik(text, false);

        }

        /// <summary>
        /// Wywołanie funkcji Oblicz
        /// </summary>
        private void przyciskWynik_Click(object sender, RoutedEventArgs e)
        {
            oblicz();
        }
    }
}