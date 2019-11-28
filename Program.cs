using System;

// 1. Поиск нечетного числа заданного дипазона, который выбирается случайно
// 2. Поиск простого числа, из нечетных чисел, полученных в пункте 1
// 3. Поиск мнимального e
// 4. Поиск закрытого ключа

namespace RSA
{
   class Program
    {
        const int PQ_BITS = 20;                    //Максимальное числоа 28 битное. 32 битное тупит
        const int PQ_START = (1 << (PQ_BITS/2));   //Середина нашего ключа
        const int PQ_RANGE = (2 << (PQ_BITS/3));   //Выбор дипозона, равного трети битов в ключе, умноженной на 2

        static void Main()
        {
            long inp = Convert.ToInt32(Console.ReadLine());

            long p = 0;          // p - параметр для формулы RSA
            long q = 0;          // q - параметр для формулы RSA
            long e, n, fn, d = 0, ds = 0;    // Параметры для формулы RSA
            int i = 200;    // Диапозон случайных чисел

            var rand = new Random();    // Объект для работы со случайными числами
            long ran = rand.Next(i);     // Случайное число заданного диапазона

            // Поиск нечетного числа по заданному диапозону
            // Поиск простого числа из нечетных
            pq_prime(ref p, ref q, ref ran);
           
            n = p * q;
            fn = (p - 1) * (q - 1);
            e = 3;

            // Найти минимальный e, тут мудрить не надо
            while (nod(fn,e) > 1)
            {
                e += 2;
            }

            // Алгоритм расширенного НОД для поиска закрытого ключа (e, ф(n))
            nodext(e, fn, ref d, ds);

            if (d < 0) d += fn;

            // Числа
            Console.WriteLine("(p,q) p = " + p + ", q = " + q);
            Console.WriteLine("(e,n) e = " + e + ", n = " + n);
            Console.WriteLine("(d,n) d = " + d + ", n = " + n);


          //  Console.WriteLine("ТЕСТ" + fastpow(400, 13, 1041541));
            // Шифруем цифру `inp`
            //    int inp = 1111;
            Console.WriteLine("INPUT:   " + inp);

            // Производим раунд шифрования
            long cd = fastpow(inp, e, n);
            Console.WriteLine("CYPHER:  " + cd);
      
            // Раунд дешифрования
            long dc = fastpow(cd, d, n);
            Console.WriteLine("DECRYPT: " + dc);

            Console.ReadLine();
   
        }

        // Метод поиска двух простых чисел
        static void pq_prime(ref long p, ref long q, ref long ran)
        {
            // Поиск случайного НЕЧЕТНОГО числа возле середины
            p = (PQ_START + ((ran % PQ_RANGE) - (PQ_RANGE >> 1))) | 1;
           
            // Поиск простого числа из нечетных чисел
            while (prime(p) == 0)
            {
                p += 2;
            }

            // Ищется МАКСИМАЛЬНОЕ НЕЧЕТНОЕ q, близкое к верхнему пределу 28-х битного ключа
            q = 1 | (((1 << PQ_BITS) - 1) / p);

            // Ищется ближайшее простое число
            while (prime(q) == 0)
            {
                q -= 2;
            }

        }

        // Метод поиска простого числа
        static long prime(long n)
        {
            for (long i = 2; i <= Math.Sqrt(n); i++)
            {
                if ((n % i) == 0) {
                    return 0;
                }
               
            }
            return 1;
        }
        
        // Метод поиска НОД
        static long nod(long a, long b)
        {
            return (b == 0) ? a : nod(b, a % b);  
        }

        // Алгоритм расширенного НОД
        static long nodext(long a, long b, ref long x, long y)
        {
            long q, r;
            long x2 = 1, y2 = 0, // x(i-2)=1, y(i-2)=0
            x1 = 0, y1 = 1; // x(i-1)=0, y(i-1)=1

            do
            {
                // Алгоритм Евклида
                q = a / b;
                r = a % b;
                a = b;
                b = r;

                // Расширенный алгоритм Евклида
                x = x2 - x1 * q; // x(i) = x(i-2) - x(i-1)*q
                y = y2 - y1 * q; // y(i) = y(i-2) - y(i-1)*q

                // "Сдвиг истории" назад
                x2 = x1; y2 = y1; // Старый x(i-1), y(i-1) перемещается в x(i-2), y(i-2)
                x1 = x; y1 = y;  // Новый  x(i), y(i) становится x(i-1), y(i-1)

            } while (r > 0); // Перебирать, пока не станет r = 0

            // И записать разультат, где x = x(i-1), y = y(i-1)
            // т.к. ранее мы "сдвинули" в (x2, y2) старые (x1, y1)
            x = x2;
            y = y2;

            // Вернуть значение НОД
            return a;
        }

        // a^b mod m
        static long fastpow(long a, long b, long m)
        {
            int id;
            long r = 1;

            for (id = 20; id >= 0; id--)
            {
                
                if ((b & (1 << id)) != 0) r = (r * a) % m;
                if (id != 0) r = (r * r) % m;
            }

            return r;
        }
    }
}
