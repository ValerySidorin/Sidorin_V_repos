using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Figures
{
    public class Figure
    {
        public List<double> values { get; set; }
        public Figure(List<double> list)
        {
            values = list;
        }

        public Figure()
        {

        }

        public virtual double CountSquare()
        {
            if (values.Count == 1)
            {
                Figure rnd = new Round(values);
                return rnd.CountSquare();
            }
            if (values.Count == 3)
            {
                Figure trgl = new Triangle(values);
                return trgl.CountSquare();
            }
            return -1;
        }
    }

    public class Round : Figure
    {

        public Round(List<double> list) : base(list)
        {

        }

        public Round(double radius)
        {
            values.Add(radius);
        }
        public override double CountSquare()
        {
            return Math.Pow(values[0], 2) * Math.PI;
        }
    }

    public class Triangle : Figure
    {
        public Triangle(List<double> list) : base(list)
        {

        }

        public Triangle(double firstside, double secondside, double thirdside)
        {
            values.Add(firstside);
            values.Add(secondside);
            values.Add(thirdside);
        }
        public int Check()
        {
            for (int i = 0; i < values.Count; i++)
            {
                for (int j = i + 1; j < values.Count; j++)
                {
                    if (values[i] == values[j])
                    {
                        values.Remove(values[j]);
                        return 1;
                    }
                }
            }

            double max = values.Max();
            values.Remove(max);
            if (Math.Pow(values[0], 2) * Math.Pow(values[1], 2) == max)
            {
                return 2;
            }
            else
            {
                values.Add(max);
                return 0;
            }
        }

        public double MultiSides(List<double> sides)
        {
            double multi = 1;

            foreach (double side in sides)
                multi *= side;

            return multi;
        }

        public double Perimeter(List<double> sides)
        {
            double sum = 0;

            foreach (double side in sides)
                sum += side;

            return sum;
        }

        public double CountHalfPerimeter() => Perimeter(values) / 2;
        public override double CountSquare()
        {
            switch (Check())
            {
                case 0:
                    return Math.Sqrt(CountHalfPerimeter() * (CountHalfPerimeter() - values[0]) * (CountHalfPerimeter() - values[1]) * (CountHalfPerimeter() - values[2]));
                case 1:
                    return MultiSides(values) / 2;
                case 2:
                    return MultiSides(values);
            }
            return -1;
        }
    }
}
