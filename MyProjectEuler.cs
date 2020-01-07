using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Project_Euler
{
    static class Program
    {
        static void FillArray(int[,] arr)
        {
            Random rand = new Random();
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    arr[i, j] = rand.Next(10, 100);
        }

        static void PrintArray(int[,] arr)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                    Console.Write($"{arr[i, j]} ");
                Console.Write("\n");
            }

        }

        //Задача 6.
        //Найдите разность между суммой квадратов и квадратом суммы первых ста натуральных чисел.
        static int Euler6()
        {
            int sum = 0;
            int sum2 = 0;

            for (int i = 1; i <= 100; i++)
                sum += i * i;

            for (int i = 1; i <= 100; i++)
                sum2 += i;

            return sum2 * sum2 - sum;

        }

        //Задача 7.
        //Выписав первые шесть простых чисел, получим 2, 3, 5, 7, 11 и 13. Очевидно, что 6-ое простое число - 13.
        //число является 10001-ым простым числом?
        static int Euler7(int n)
        {
            int p = 0;
            int counter = 4;

            for (int i = 8; i <= 1000000000; i++)
            {
                if (i % 2 != 0 && i % 3 != 0 && i % 5 != 0 && i % 7 != 0)
                {
                    p = i;
                    counter++;
                }

                if (counter == n)
                    break;
            }

            return p;
        }

        //Задача 8.
        //Наибольшее произведение четырех последовательных цифр в нижеприведенном 1000-значном числе равно 9 × 9 × 8 × 9 = 5832.
        //Найдите наибольшее произведение тринадцати последовательных цифр в данном числе.
        static int Euler8(string s)
        {
            char[] ch = s.ToCharArray();

            int[] arr = new int[100];
            int multi = 1;


            for (int i = 0; i < ch.Length; i++)
            {
                arr[i] = int.Parse(ch[i].ToString());
            }

            for (int i = 0; i < arr.Length - 13; i++)
            {
                int p = 1;
                for (int j = i; j < i + 13; j++)
                {
                    p *= arr[j];
                }
                if (p > multi)
                    multi = p;
            }

            return multi;
        }

        //Задача 9.
        //Существует только одна тройка Пифагора, для которой a + b + c = 1000.
        //Найдите произведение abc.
        static int Euler9()
        {
            int a = 0;
            int b = 0;
            int c = 0;

            for (int i = 1; i < 500; i++)
            {
                for (int j = 1; j < 500; j++)
                {
                    for (int k = 1; k < 500; k++)
                    {
                        if (Math.Pow(i, 2) + Math.Pow(j, 2) == Math.Pow(k, 2) && i + j + k == 1000)
                        {
                            a = i;
                            b = j;
                            c = k;
                        }
                    }
                }
            }
            return a * b * c;
        }

        //Задача 10.
        //Сумма простых чисел меньше 10 равна 2 + 3 + 5 + 7 = 17.
        //Найдите сумму всех простых чисел меньше двух миллионов.
        static int Euler10()
        {
            int sum = 17;

            for (int i = 8; i < 2000000; i++)
            {
                if (i % 2 != 0 && i % 3 != 0 && i % 5 != 0 && i % 7 != 0)
                {
                    sum += i;
                }
            }

            return sum;
        }

        //Задача 11.
        //Каково наибольшее произведение четырех подряд идущих чисел в таблице 20×20, 
        //расположенных в любом направлении (вверх, вниз, вправо, влево или по диагонали)?
        static int Euler11(int[,] arr)
        {
            int multi = 1;

            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1) - 3; j++)
                    if (arr[i, j] * arr[i, j + 1] * arr[i, j + 2] * arr[i, j + 3] > multi)
                        multi = arr[i, j] * arr[i, j + 1] * arr[i, j + 2] * arr[i, j + 3];

            for (int i = 0; i < arr.GetLength(0) - 3; i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    if (arr[i, j] * arr[i + 1, j] * arr[i + 2, j] * arr[i + 3, j] > multi)
                        multi = arr[i, j] * arr[i + 1, j] * arr[i + 2, j] * arr[i + 3, j];

            for (int i = 0; i < arr.GetLength(0) - 3; i++)
                for (int j = 0; j < arr.GetLength(1) - 3; j++)
                    if (arr[i, j] * arr[i + 1, j + 1] * arr[i + 2, j + 2] * arr[i + 3, j + 3] > multi)
                        multi = arr[i, j] * arr[i + 1, j + 1] * arr[i + 2, j + 2] * arr[i + 3, j + 3];

            for (int i = 0; i < arr.GetLength(0) - 3; i++)
                for (int j = 3; j < arr.GetLength(1); j++)
                    if (arr[i, j] * arr[i + 1, j - 1] * arr[i + 2, j - 2] * arr[i + 3, j - 3] > multi)
                        multi = arr[i, j] * arr[i + 1, j - 1] * arr[i + 2, j - 2] * arr[i + 3, j - 3];

            return multi;
        }

        //Задача 12.
        //Каково первое треугольное число, у которого более пятисот делителей?
        static void Euler12()
        {
            int number = 0;
            for (int i = 1; i <= 13000; i++)
            {
                int counter = 0;
                number += i;
                for (int j = 1; j * j < number; j++)
                {
                    if (number % j == 0)
                    {
                        counter += 2;
                        if (counter == 500)
                        {
                            Console.WriteLine($"{i} - {number}");
                        }
                    }
                }
            }
        }

        static void Euler12ver2()
        {
            bool flag = true;
            long number = 1;
            long nextNum = 2;
            int maxDiv = 500;

            while (flag)
            {
                if (CountDiv(number) > maxDiv)
                {
                    Console.WriteLine(number);
                    flag = false;
                }
                else
                {
                    number += nextNum;
                    nextNum++;
                }
            }
        }

        static long CountDiv(long number)
        {
            long div = 0;
            for (int i = 1; i * i < number; i++)
                if (number % i == 0)
                    div += 2;
            return div;
        }

        //Задача 13.
        //Найдите первые десять цифр суммы следующих ста 50-значных чисел...
        static void Euler13(long[] arr, int n)
        {
            long sum = 0;
            for (int i = 0; i < n; i++)
            {
                sum /= 10;
                for (int j = 0; j < arr.Length; j++)
                {
                    sum += arr[j] % 10;
                }
                Console.WriteLine(sum % 10);
            }
        }

        //Задача 17.
        //Если записать числа от 1 до 5 английскими словами(one, two, three, four, five), 
        //то используется всего 3 + 3 + 5 + 4 + 4 = 19 букв.
        //Сколько букв понадобится для записи всех чисел от 1 до 1000 (one thousand) включительно?
        static string CreateWord(int n)
        {
            int temp = n;
            StringBuilder result = new StringBuilder();
            string[] units = new string[] { "", "one ", "two ", "three ", "four ", "five ", "six ", "seven ", "eight ", "nine " };
            string[] dozens = new string[] { "", "ten ", "twenty ", "thirty ", "fourty ", "fifty ", "sixty ", "seventy ", "eighty ", "ninety " };
            string[] exceptions = new string[] { "", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            string[] hundreds = new string[] { "", "one hundred ", "two hundred ", "three hundred ", "four hundred ", "five hundred ", "six hundred ", "seven hundred ", "eight hundred ", "nine hundred " };
            string[] thousands = new string[] { "", "one thousand ", "two thousand ", "three thousand ", "four thousand ", "five thousand ", "six thousand ", "seven thousand ", "eight thousand ", "nine thousand " };

            if (n > 9999)
                throw new IndexOutOfRangeException();

            if (n <= 9999 && n > 999)
            {
                int j = n / 1000;
                result.Append(thousands[j]);
                if ((temp / 100) % 10 == 0)
                    result.Append("and ");
                n = n % 1000;
            }

            if (n <= 999 && n > 99)
            {
                int j = n / 100;
                result.Append(hundreds[j]);
                if ((temp / 100) % 10 != 0)
                    result.Append("and ");
                n = n % 100;
            }

            if (n <= 99 && n > 9)
            {
                if (n > 10 && n < 20)
                {
                    int i = n % 10;
                    result.Append(exceptions[i]);
                    return result.ToString();
                }
                int j = n / 10;
                result.Append(dozens[j]);
                n = n % 10;
            }

            if (n <= 9)
            {
                result.Append(units[n]);
            }

            return result.ToString();
        }

        static int CountChars(int from, int to)
        {
            int sum = 0;

            for (int i = from; i <= to; i++)
            {
                string s = CreateWord(i);
                string[] temp = s.Split(' ');

                foreach (string str in temp)
                {
                    char[] c = str.ToCharArray();
                    sum += c.Length;
                }
            }
            return sum;
        }

        //Задача 18.
        //Найдите максимальную сумму пути от вершины до основания следующего треугольника:
        static void Euler18BruteForce(int[][] triangle)
        {
            int tempsum;
            int sum = 0;
            int possibleSolutions = (int)Math.Pow(2, triangle.Length - 1);
            int index;

            for (int i = 0; i < possibleSolutions; i++)
            {
                tempsum = triangle[0][0];
                index = 0;
                for (int j = 0; j < triangle.Length - 1; j++)
                {
                    index = index + (i >> j & 1);
                    tempsum += triangle[j + 1][index];
                }

                if (tempsum > sum)
                    sum = tempsum;
            }

            Console.WriteLine(sum);
        }

        static void Euler18Optimal(int[][] triangle)
        {
            int lines = triangle.Length - 1;

            for (int i = lines - 1; i >= 0; i--)
            {
                for (int j = 0; j <= triangle[i].Length - 1; j++)
                {
                    triangle[i][j] += Math.Max(triangle[i + 1][j], triangle[i + 1][j + 1]);
                }
            }

            Console.WriteLine(triangle[0][0]);
        }

        //Задача 19.
        //Сколько воскресений выпадает на первое число месяца в двадцатом веке (с 1 января 1901 года до 31 декабря 2000 года)?
        static int Euler19(int from, int to)
        {
            int[] months = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            int counter = 0;
            int dayOfWeek = 2;

            for (int i = from; i <= to; i++)
            {
                if (i % 4 == 0)
                    months[1]++;

                for (int j = 0; j < months.Length; j++)
                {
                    dayOfWeek += months[j] % 7;

                    if (dayOfWeek > 7)
                        dayOfWeek -= 7;

                    if (dayOfWeek == 7)
                        counter++;
                }

                if (months[2] == 29)
                    months[2]--;

            }
            return counter;
        }

        //Задача 21.
        //Подсчитайте сумму всех дружественных чисел меньше 10000.
        static int Euler21(int n)
        {
            int sum = 0;

            for (int i = 1; i < n; i++)
            {
                int div = d(i);
                if (d(div) == i && i != div)
                    sum += i + div;
            }
            return sum / 2;
        }

        static int d(int n)
        {
            int sum = 0;
            for (int i = 1; i <= n / 2; i++)
            {
                if (n % i == 0)
                    sum += i;
            }
            return sum;
        }
        static void Main(string[] args)
        {
        }
    }
}
