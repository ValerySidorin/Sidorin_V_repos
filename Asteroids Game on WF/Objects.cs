using System;
using System.Collections.Generic;
using System.Drawing;


namespace Asteroids
{

    interface ICollision
    {
        bool Collision(ICollision obj);
        Rectangle rect { get; }
    }
    abstract class BaseObject : ICollision
    {
        protected Point pos;
        protected Point dir;
        protected Size size;
        public Rectangle rect => new Rectangle(pos, size);

        //public delegate void Message();
        //public delegate void Hit();


        public BaseObject(Point pos, Point dir, Size size)
        {
            this.pos = pos;
            this.dir = dir;
            this.size = size;
        }



        public abstract void Draw(Image image);

        public abstract void Update();

        public virtual void Refresh(Size Size)
        {

        }

        public bool Collision(ICollision o) => o.rect.IntersectsWith(this.rect);
    }

    
    class Star : BaseObject
    {
        public static Image Image1 = Image.FromFile("Images\\Star1.png");
        public static Image Image2 = Image.FromFile("Images\\Star_2.png");
        public static Image Image3 = Image.FromFile("Images\\Star_3.png");

        public Star() : base(new Point(0, 0), new Point(0, 0), new Size(0, 0))
        {

        }


        public Star(Point pos, Point dir, Size size) : base(pos, dir, size)
        {

        }

        public override void Update()
        {
            pos.X = pos.X + dir.X;
            pos.Y = pos.Y + dir.Y;
            if (pos.X < -100)
            {
                pos.X = Game.Width;
                pos.Y = new Random().Next(0, Game.Height + 20);
            }
        }

        public override void Draw(Image image)
        {
            Game.buffer.Graphics.DrawImage(image, pos);
        }
    }

    class Asteroid : BaseObject
    {
        public static event Action AsteroidKilled;

        public static Image Image1 = Image.FromFile("Images\\asteroid1.png");
        public static Image Image2 = Image.FromFile("Images\\asteroid2.png");
        public static Image Image3 = Image.FromFile("Images\\asteroid3.png");

        public int Power { get; set; }


        public Asteroid() : base(new Point(0, 0), new Point(0, 0), new Size(0, 0))
        {
            Power = 1;
        }


        public Asteroid(Point pos, Point dir, Size size, int power) : base(pos, dir, size)
        {
            Power = power;
        }

        public override void Update()
        {
            pos.X = pos.X + dir.X;
            pos.Y = pos.Y + dir.Y;
            if (pos.X < -60)
            {
                Refresh(size);
            }
            if (pos.Y < -60) 
            {
                Refresh(size);
            }
            if (pos.Y > Game.Height) 
            {
                Refresh(size);
            }
        }
        public bool Intersects(Asteroid asteroid) => pos.X == asteroid.pos.X && pos.Y == asteroid.pos.Y;


        public override void Draw(Image image)
        {
            Game.buffer.Graphics.DrawImage(image, pos);
        }

        public override void Refresh(Size Size)
        {
            pos = new Point(new Random().Next(800, 1300), new Random().Next(1, 500));
            dir = new Point(new Random().Next(-7, -5), new Random().Next(-3, 3));
            size = Size;
        }

        public void Dead()
        {
            AsteroidKilled?.Invoke();
        }
    }

    class Bullet : BaseObject
    {
        public static Image Image = Image.FromFile("Images\\lazershot.png");
        public Bullet() : base(new Point(0, 0), new Point(0, 0), new Size(0, 0))
        {

        }


        public Bullet(Point pos, Point dir, Size size) : base(pos, dir, size)
        {

        }

        public override void Draw(Image image)
        {
            Game.buffer.Graphics.DrawImage(image, pos);
        }

        public override void Update()
        {
            pos.X = pos.X + dir.X;
            pos.Y = pos.Y + dir.Y;
        }

        public override void Refresh(Size Size)
        {
            pos = new Point(-100, -100);
            dir = new Point(0, 0);
            size = Size;
        }

        public bool OutOfRange()
        {
            return pos.X > Game.Width;
        }

        public void Shoot(Point position, Point direction)
        {
            pos = position;
            dir = direction;
        }
    }

    class Ship : BaseObject
    {
        public int Energy { get; set; }
        public static Image Image = Image.FromFile("Images\\spaceship(resized).png");

        public static event Action MessageDie;
        public static event Action HitMessage;
        public static event Action HealMessage;

        public Ship(Point pos, Point dir, Size size) :base(pos, dir, size)
        {
            Energy = 100;
        }

        public override void Draw(Image image)
        {
            Game.buffer.Graphics.DrawImage(image, pos);
        }

        public override void Update()
        {
            if (pos.Y > 0 && pos.Y < Game.Height - 75)
                pos.Y += dir.Y;
            else
            {
                pos.Y = pos.Y <= 0 ? 1 : Game.Height - 76;
                dir.Y = 0;
            }
        }

        public void EnergyLow(Asteroid asteroid)
        {
            Energy -= asteroid.Power;
            if (Energy < 0)
                Energy = 0;

            HitMessage?.Invoke();
        }

        public void Heal(MedPack med)
        {
            Energy += med.Value;
            if (Energy > 100)
                Energy = 100;

            HealMessage?.Invoke();
        }

        public void Up()
        {
            if (dir.Y != 0) dir.Y = 0;
            else
                dir.Y = -3;
        }

        public void Down()
        {
            if (dir.Y != 0) dir.Y = 0;
            else
                dir.Y = 3;
        }

        public void Die()
        {
            MessageDie?.Invoke();
        }
    }

    class MedPack : BaseObject
    {
        public int Value { get; set; }
        public static Image Image = Image.FromFile("Images\\med.png");

        public MedPack() : base(new Point(0, 0), new Point(0, 0), new Size(0, 0))
        {
            Value = 10;
        }


        public MedPack(Point pos, Point dir, Size size) : base(pos, dir, size)
        {
            Value = 10;
        }

        public override void Update()
        {
            pos.X = pos.X + dir.X;
            pos.Y = pos.Y + dir.Y;
            if (pos.X < -60)
            {
                Refresh(size);
            }
            if (pos.Y < -60)
            {
                Refresh(size);
            }
            if (pos.Y > Game.Height)
            {
                Refresh(size);
            }
        }

        public override void Draw(Image image)
        {
            Game.buffer.Graphics.DrawImage(image, pos);
        }

        public override void Refresh(Size Size)
        {
            pos = new Point(860, new Random().Next(0, 500));
            dir = new Point(new Random().Next(-7, -5), new Random().Next(-5, 5));
            size = Size;
        }
    }
}
