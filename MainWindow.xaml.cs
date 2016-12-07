using System;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Gierka
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private int licznik;
        private int NajlepszyWynik;


        Random random = new Random();
        DispatcherTimer enemyTimer = new DispatcherTimer();
        DispatcherTimer targetTimer = new DispatcherTimer();
        bool humanCaptured = false;
        public MainWindow()
        {

            InitializeComponent();

            enemyTimer.Tick += EnemyTimer_Tick;// licznik czasu
            enemyTimer.Interval = TimeSpan.FromSeconds(2);

            targetTimer.Tick += TargetTimer_Tick;
            targetTimer.Interval = TimeSpan.FromSeconds(.1);
            playArea.Children.Clear();
        }

        private void TargetTimer_Tick(object sender, EventArgs e) // metoda końca gry(EndTheGame), która wywoluje sie po przekroczeniu licznika
        {
            progressBar.Value += 1;
            if (progressBar.Value >= progressBar.Maximum)
                EndTheGame();
        }



        private void EndTheGame() // metoda, ktora zatrzymuje grę i wyświetla wynik
        {
            if (!playArea.Children.Contains(gameOverText))
            {
                enemyTimer.Stop();
                targetTimer.Stop();
                humanCaptured = false;
                startButton.Visibility = Visibility.Visible;
                quitButton.Visibility = Visibility.Visible;
                playArea.Children.Add(gameOverText);
                if (licznik >= NajlepszyWynik)
                {
                    NajlepszyWynik = licznik;
                    BestScore.Text = Convert.ToString(NajlepszyWynik);
                }
                if (licznik == 1)
                {
                    MessageBox.Show("No slabo zdobyles tylko " + licznik + " punkt :(");
                }
                if (licznik > 1 && licznik < 5)
                {
                    MessageBox.Show("No no udalo ci sie zdobyc " + licznik + " punkty !!!");
                }
                if (licznik > 5 && licznik < 9999999)
                {
                    MessageBox.Show("Brawo uzyskales " + licznik + " punktow !!!");
                }
            }
        }

        private void EnemyTimer_Tick(object sender, EventArgs e) //wywołanie metody, która dodaje wrogów
        {
            AddEnemy();
        }

        private void startButton_Click(object sender, RoutedEventArgs e) // wywołanie metody, która wykona sie po wciśnięciu "start"
        {
            licznik = 0;
            Score.Text = Convert.ToString(licznik);
            BestScore.Text = Convert.ToString(NajlepszyWynik);
            StartGame();
        }

        private void StartGame() // metoda, która rozpoczyna grę
        {
            human.IsHitTestVisible = true;
            humanCaptured = false;
            progressBar.Value = 0;
            startButton.Visibility = Visibility.Collapsed;
            quitButton.Visibility = Visibility.Collapsed;
            playArea.Children.Clear();
            playArea.Children.Add(target);
            playArea.Children.Add(human);
            enemyTimer.Start();
            targetTimer.Start();
            Canvas.SetLeft(target, random.Next(100, (int)playArea.ActualWidth - 100));
            Canvas.SetTop(target, random.Next(100, (int)playArea.ActualHeight - 100));
            Canvas.SetLeft(human, random.Next(100, (int)playArea.ActualWidth - 100));
            Canvas.SetTop(human, random.Next(100, (int)playArea.ActualHeight - 100));
        }

        private void AddEnemy() // metoda, która tworzy nowy obiekt wroga
        {
            ContentControl enemy = new ContentControl();
            enemy.Template = Resources["EnemyTemplate"] as ControlTemplate;
            AnimateEnemy(enemy, 0, playArea.ActualWidth - 100, "(Canvas.Left)");
            AnimateEnemy(enemy, random.Next((int)playArea.ActualHeight - 100),
                random.Next((int)playArea.ActualHeight - 100), "(Canvas.Top)");
            playArea.Children.Add(enemy);

            enemy.MouseEnter += Enemy_MouseEnter;
        }

        private void Enemy_MouseEnter(object sender, MouseEventArgs e) // jeżeli dotkniemy wroga, wywołuje się metoda końca gry
        {
            if (humanCaptured)
                EndTheGame();
        }

        private void AnimateEnemy(ContentControl enemy, double from, double to, string propertyToAnimate) // kod sprawiający, ze wróg będzie poruszał się po ekranie
        {
            Storyboard storyboard = new Storyboard() { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever };
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(random.Next(4, 6))),
            };
            Storyboard.SetTarget(animation, enemy);
            Storyboard.SetTargetProperty(animation, new PropertyPath(propertyToAnimate));
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        private void human_MouseDown(object sender, MouseButtonEventArgs e) // po wciśnieciu myszy nasz bohater przypisywany jest kursorowi
        {
            enemyTimer.Start();
            if (enemyTimer.IsEnabled)
            {
                humanCaptured = true;
                if (humanCaptured)
                {
                    playArea.Children.Remove(Level);
                    targetTimer.Start();
                }
                human.IsHitTestVisible = false;
            }
        }

        private void target_MouseEnter(object sender, MouseEventArgs e) // Funkcja odpowiada za dodanie punktów do Scorw oraz zresetowania pozycji Targetu jak i naszego po zderzeniu się
                                                                        // Gdy uzyskamy odpowiednia ilosc punktów wywyoluje się medtoda NextLevel(). Po uzyskaniu 100 punktów wywoluje się metoda PrzeszedlesGre.
        {
            if (targetTimer.IsEnabled && humanCaptured)
            {
                licznik += 1;
                Score.Text = Convert.ToString(licznik);
                progressBar.Value = 0;
                Canvas.SetLeft(target, random.Next(100, (int)playArea.ActualWidth - 100));
                Canvas.SetTop(target, random.Next(100, (int)playArea.ActualHeight - 100));
                Canvas.SetLeft(human, random.Next(100, (int)playArea.ActualWidth - 100));
                Canvas.SetTop(human, random.Next(100, (int)playArea.ActualHeight - 100));
                humanCaptured = false;
                human.IsHitTestVisible = true;
                if (licznik == 5)
                {
                    NextLevel();
                }
                if (licznik == 10)
                {
                    NextLevel();
                }
                if (licznik == 25)
                {
                    NextLevel();
                }
                if (licznik == 40)
                {
                    NextLevel();
                }
                if (licznik == 65)
                {
                    NextLevel();
                }
                if (licznik == 100)
                {
                    PrzeszedlesGre();
                }
            }
        }

        private void playArea_MouseMove(object sender, MouseEventArgs e) // metoda powdujaca interakcję bohatera z kursorem myszy(po złapaniu ludzika sterujemy nim)
        {
            if (humanCaptured)
            {
                Point pointerPosition = e.GetPosition(null);
                Point relativePosition = grid.TransformToVisual(playArea).Transform(pointerPosition);
                if ((Math.Abs(relativePosition.X - Canvas.GetLeft(human)) > human.ActualWidth * 3)
                    || (Math.Abs(relativePosition.Y - Canvas.GetTop(human)) > human.ActualHeight * 3))
                {
                    humanCaptured = false;
                    human.IsHitTestVisible = true;
                }
                else
                {
                    Canvas.SetLeft(human, relativePosition.X - human.ActualWidth / 2);
                    Canvas.SetTop(human, relativePosition.Y - human.ActualHeight / 2);
                }
            }
        }
        private void playArea_MouseLeave(object sender, MouseEventArgs e) // metoda, która kończy grę po wyjściu za obszar gry
        {
            if (humanCaptured)
                EndTheGame();
        }
        private void Score_TextChanged(object sender, TextChangedEventArgs e)
        {
            Score.Text = Convert.ToString(licznik);
        }
        private void NextLevel() // metoda, która zatrzymuje grę i wyświetla nowy poziom
        {
            if (!playArea.Children.Contains(Level))
            {
                playArea.Children.Add(Level);
                targetTimer.Stop();
                enemyTimer.Stop();
            }
        }
        private void PrzeszedlesGre() // metoda, która po zdobyciu odpowiedniej ilości punktów zatrzymuje grę, wyświetla wynik oraz powoduje wyświetlenie przycisków Quit i Start Buttton
        {
            if (!playArea.Children.Contains(Przeszedles))
            {
                enemyTimer.Stop();
                targetTimer.Stop();
                humanCaptured = false;
                startButton.Visibility = Visibility.Visible;
                quitButton.Visibility = Visibility.Visible;
                playArea.Children.Add(Przeszedles);
            }
        }
        private void BestScore_TextChanged(object sender, TextChangedEventArgs e) // przypisanie najlepszego wyniku
        {
            BestScore.Text = Convert.ToString(NajlepszyWynik);
        }

        private void quitButton_Click(object sender, RoutedEventArgs e) // po wciśnieciu Quit kończy grę
        {
            Application.Current.Shutdown();
        }
    }
}

