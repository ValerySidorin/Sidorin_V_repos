using System;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Media;
using System.IO;
using System.Collections.Generic;

namespace Asteroids
{
    static class Game
    {
        static BufferedGraphicsContext context;
        static public BufferedGraphics buffer;
        static public Stream fs = new FileStream("hitlist.txt", FileMode.Create); //Поток для записи логов в файл
        static public int Width { get; set; } // Свойство ширины игрового окна
        static public int Height { get; set; } // Свойство высоты игрового окна
        static public BaseObject[] starsLittle, starsMedium, starsBig, med; // Массивы звезд и аптечек
        static public List<Asteroid> asteroidsLittle, asteroidsMedium, asteroidsBig; // Списки астероидов
        static public Image background = Image.FromFile("Images\\background.jpg"); // Картинка заднего фона
        static Point point0 = new Point(0, 0); // Нулевая точка
        static public List<Bullet> bullets; // Список пуль
        static public Ship ship; // Космический корабль
        public static System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer(); // Таймер для апдейта
        static public int score = 0; // Счетчик подбитых астероидов

        // Силы астероидов
        static public int powLittle = 5;
        static public int powMedium = 10;
        static public int powBig = 100;

        static public Size sizeLittle = new Size(30, 30);
        static public Size sizeMedium = new Size(40, 40);
        static public Size sizeBig = new Size(80, 66);

        static Game()
        {
        }

        private static void GotHit() // Метод, сигнализирующий о попадании астероида в корабль
        {
            Console.WriteLine($"Ship got hit! HP left: {ship.Energy}");

            try
            {
                StreamWriter sr = new StreamWriter("hitlist.txt", true);
                sr.WriteLine($"Ship got hit! HP left: {ship.Energy}");
                sr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void Finish() // Метод, сигнализирующий о смерти корабля
        {
            timer.Stop();
            buffer.Graphics.DrawString("The End", SystemFonts.DefaultFont, Brushes.AntiqueWhite, 0, 0);
            buffer.Render();

            Console.WriteLine("Whoops! Ship is dead!");

            try
            {
                StreamWriter sr = new StreamWriter("hitlist.txt", true);
                sr.WriteLine("Whoops! Ship is dead!");
                sr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void GotHealed() // Метод, сигнализирующий  о получении аптечки кораблем
        {
            if (ship.Energy < 100)
            {
                Console.WriteLine($"Nice! Ship got some HP! Energy level: {ship.Energy}");

                try
                {
                    StreamWriter sr = new StreamWriter("hitlist.txt", true);
                    sr.WriteLine($"Nice! Ship got some HP! Energy level: {ship.Energy}");
                    sr.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void AsteroidDead() // Метод, сигнализирующий о сбитом астероиде
        {
            Console.WriteLine($"Ship got one asteroid! Score: {score}");

            try
            {
                StreamWriter sr = new StreamWriter("hitlist.txt", true);
                sr.WriteLine($"Ship got one asteroid! Score: {score}");
                sr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static public void CheckForCollision(List<Asteroid> asteroids, List<Bullet> bullets, Ship ship, Size size) //Метод для проверки столкновения объектов
        {
            Random rand = new Random();
            for (int i = 0; i < asteroids.Count; i++)
            {
                asteroids[i].Update();
                for (int j = 0; j < bullets.Count; j++)
                {
                    if (asteroids[i].Collision(bullets[j]))
                    {
                        score++;
                        asteroids[i].Dead();
                        asteroids[i].Refresh(size);
                        asteroids.Add(new Asteroid(new Point(rand.Next(800, 1300), rand.Next(1, 500)), new Point(rand.Next(-5, 5), rand.Next(-5, 5)), size, asteroids[i].Power));
                        bullets.RemoveAt(j);
                        j--;
                        asteroids[i].Update();
                    }
                }
                if (!ship.Collision(asteroids[i])) continue;
                ship?.EnergyLow(asteroids[i]);
                asteroids[i].Refresh(size);
                SystemSounds.Asterisk.Play();
                if (ship.Energy <= 0) ship?.Die();
            }
        }
        static public void Init(Form form)
        {       
            Graphics g;
            context = BufferedGraphicsManager.Current;
            g = form.CreateGraphics();
            fs.Close();
            Width = form.Width;
            Height = form.Height;

            if (Width > 800 || Height > 600) throw new Exception();

            buffer = context.Allocate(g, new Rectangle(0, 0, Width, Height));
            Load();
            timer.Interval = 20;
            timer.Tick += Timer_Tick;
            timer.Start();

            SoundPlayer sp = new SoundPlayer("Audio\\SoundTrack.wav");
            sp.PlayLooping();

            form.KeyDown += Form_KeyDown;

            //Делегаты типа Action, передающие информацию из класса в метод
            Ship.MessageDie += Finish;
            Ship.HitMessage += GotHit;
            Ship.HealMessage += GotHealed;
            Asteroid.AsteroidKilled += AsteroidDead;
        }

        private static void Form_KeyDown(object sender, KeyEventArgs e)
        {
            //При нажатии добавляем пулю в список
            if (e.KeyCode == Keys.Space)
            {
                bullets.Add(new Bullet(new Point(ship.rect.X + 10, ship.rect.Y), new Point(10, 0), new Size(35, 18)));
            }
            if (e.KeyCode == Keys.Up) ship.Up();
            if (e.KeyCode == Keys.Down) ship.Down();
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            Update();
            Draw();
        }

        static public void Load()
        {
            Random rand = new Random(); // Переменная для генерации стартовой позиции и вектора для объектов
            bullets = new List<Bullet>(); // Инициализируем список пуль
            ship = new Ship(new Point(10, 290), new Point(0, 0), new Size(40, 20)); // Инициализируем космический корабль


            // Инициализируем массивы звезд, задаем им позиции и векторы
            starsLittle = new BaseObject[35]; 
            for (int i = 0; i < starsLittle.Length; i++)
                starsLittle[i] = new Star(new Point(rand.Next(-100, 800), rand.Next(0, 500)), new Point(-1, 0), new Size(1, 1));

            starsMedium = new BaseObject[15];
            for (int i = 0; i < starsMedium.Length; i++)
                starsMedium[i] = new Star(new Point(rand.Next(-100, 800), rand.Next(0, 500)), new Point(-2, 0), new Size(5, 5));

            starsBig = new BaseObject[4];
            for (int i = 0; i < starsBig.Length; i++)
                starsBig[i] = new Star(new Point(rand.Next(-100, 800), rand.Next(0, 500)), new Point(-3, 0), new Size(10, 10));

            // Инициализируем списки астероидов, задаем им позиции и векторы
            asteroidsLittle = new List<Asteroid>();
            for (int i = 0; i < 3; i++)
                asteroidsLittle.Add(new Asteroid(new Point(rand.Next(800, 1300), rand.Next(1, 500)), new Point(rand.Next(-5, -5), rand.Next(-5, 5)), new Size(30, 30), powLittle));

            asteroidsMedium = new List<Asteroid>();
            for (int i = 0; i < 2; i++)
                asteroidsMedium.Add(new Asteroid(new Point(rand.Next(800, 1300), rand.Next(1, 500)), new Point(rand.Next(-5, -5), rand.Next(-5, 5)), new Size(40, 40), powMedium));

            asteroidsBig = new List<Asteroid>();
            for (int i = 0; i < 1; i++)
                asteroidsBig.Add(new Asteroid(new Point(rand.Next(800, 1300), rand.Next(1, 500)), new Point(rand.Next(-5, -5), rand.Next(-2, 2)), new Size(80, 66), powBig));

            // Инициализируем массив аптечек
            med = new BaseObject[2];
            for (int i = 0; i < med.Length; i++)
                med[i] = new MedPack(new Point(rand.Next(800, 1300), rand.Next(1, 500)), new Point(rand.Next(-7, -5), rand.Next(-5, 5)), new Size(30, 30));
        }


        static public void Draw() // Метод отрисовки всех объектов
        {
            buffer.Graphics.DrawImage(background, point0);
            foreach (Star obj in starsLittle)
                obj.Draw(Star.Image1);
            foreach (Star obj2 in starsMedium)
                obj2.Draw(Star.Image2);
            foreach (Star obj3 in starsBig)
                obj3.Draw(Star.Image3);
            foreach (Asteroid asteroid in asteroidsLittle)
                asteroid.Draw(Asteroid.Image1);
            foreach (Asteroid asteroid in asteroidsMedium)
                asteroid.Draw(Asteroid.Image2);
            foreach (Asteroid asteroid in asteroidsBig)
                asteroid.Draw(Asteroid.Image3);
            

            foreach (Bullet bul in bullets)
                bul.Draw(Bullet.Image);

            foreach (MedPack m in med)
                m.Draw(MedPack.Image);

            ship?.Draw(Ship.Image);
            if (ship != null)
            {
                buffer.Graphics.DrawString($"Energy: {ship.Energy}", SystemFonts.DefaultFont, Brushes.Red, 0, 0);
            }

            buffer.Graphics.DrawString($"Score: {score}", SystemFonts.DefaultFont, Brushes.DarkSeaGreen, 360, 0);

            buffer.Render();
        }

        static public void Update() // Метод, исполняющийся каждый тик таймера
        {
            foreach (BaseObject obj in starsLittle) obj.Update();

            foreach (BaseObject obj2 in starsMedium) obj2.Update();

            foreach (BaseObject obj3 in starsBig) obj3.Update();

            foreach (Bullet bul in bullets) bul.Update();

            ship?.Update();

            // Уничтожаем пули, вылетающие за пределы игрового поля
            for (int i = 0; i < bullets.Count; i++)
            {
                if (bullets[i].OutOfRange())
                {
                    bullets.RemoveAt(i);
                    i--;
                }
            }

            for ( int i = 0; i < med.Length; i++)
            {
                med[i].Update();
                if (!med[i].Collision(ship)) continue;

                ship?.Heal((MedPack)med[i]);
                med[i].Refresh(new Size(30, 30));
            }

            // Проверяем столкновения объектов
            CheckForCollision(asteroidsLittle, bullets, ship, sizeLittle);
            CheckForCollision(asteroidsMedium, bullets, ship, sizeMedium);

            for (int i = 0; i < asteroidsBig.Count; i++)
            {
                for (int j = 0; j < bullets.Count; j++)
                {
                    if (asteroidsBig[i].Collision(bullets[j]))
                    {
                        bullets.RemoveAt(j);
                        j--;
                    }
                }
                asteroidsBig[i].Update(); 

                if (!ship.Collision(asteroidsBig[i])) continue;
                ship?.EnergyLow(asteroidsBig[i]);
                System.Media.SystemSounds.Asterisk.Play();
                if (ship.Energy <= 0) ship?.Die();
            }
        }
    }

}
