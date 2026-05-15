using System;
using System.Numerics;

class RSADecryptor
{
    static BigInteger NWD(BigInteger a, BigInteger b)
    {
        while (b != 0)
        {
            BigInteger temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    static (BigInteger gcd, BigInteger x, BigInteger y) RozszerzonyEuklides(BigInteger a, BigInteger b)
    {
        if (b == 0)
            return (a, 1, 0);

        var (gcd, x1, y1) = RozszerzonyEuklides(b, a % b);
        return (gcd, y1, x1 - (a / b) * y1);
    }

    static BigInteger OdwrotnoscModularna(BigInteger e, BigInteger phi)
    {
        var (gcd, x, _) = RozszerzonyEuklides(e, phi);
        if (gcd != 1)
            throw new Exception($"NWD(e, phi) = {gcd}, odwrotnosc nie istnieje");
        return ((x % phi) + phi) % phi;
    }

    static BigInteger ISqrt(BigInteger n)
    {
        BigInteger x = (BigInteger)Math.Sqrt((double)n);
        while (x * x > n) x--;
        while ((x + 1) * (x + 1) <= n) x++;
        return x;
    }

    static (BigInteger p, BigInteger q)? Faktoryzuj(BigInteger n)
    {
        if (n % 2 == 0)
            return (2, n / 2);

        BigInteger sqrtN = ISqrt(n);
        for (BigInteger i = 3; i <= sqrtN + 1; i += 2)
        {
            if (n % i == 0)
                return (i, n / i);
        }

        BigInteger a = ISqrt(n) + 1;
        for (int iter = 0; iter < 100000; iter++, a++)
        {
            BigInteger b2 = a * a - n;
            if (b2 < 0) continue;
            BigInteger b = ISqrt(b2);
            if (b * b == b2)
            {
                BigInteger p = a - b;
                BigInteger q = a + b;
                if (p > 1 && q > 1 && p * q == n)
                    return (p, q);
            }
        }

        return null;
    }

    static BigInteger PotegowanieModularne(BigInteger b, BigInteger exp, BigInteger mod)
    {
        BigInteger result = 1;
        b %= mod;
        while (exp > 0)
        {
            if (exp % 2 == 1)
                result = result * b % mod;
            exp >>= 1;
            b = b * b % mod;
        }
        return result;
    }

    static void Main()
    {
        Console.Write("N = ");
        BigInteger N = BigInteger.Parse(Console.ReadLine().Trim());

        Console.Write("E = ");
        BigInteger E = BigInteger.Parse(Console.ReadLine().Trim());

        Console.Write("C = ");
        BigInteger C = BigInteger.Parse(Console.ReadLine().Trim());

        var wynik = Faktoryzuj(N);
        if (wynik == null)
        {
            Console.WriteLine("Nie udalo sie sfaktoryzowac N.");
            return;
        }

        var (p, q) = wynik.Value;
        BigInteger phi = (p - 1) * (q - 1);
        BigInteger nwd = NWD(E, phi);

        Console.WriteLine($"p = {p}");
        Console.WriteLine($"q = {q}");
        Console.WriteLine($"phi(N) = {phi}");
        Console.WriteLine($"NWD(e, phi) = {nwd}");

        if (nwd != 1)
        {
            Console.WriteLine("Blad: NWD(e, phi) != 1");
            return;
        }

        BigInteger d = OdwrotnoscModularna(E, phi);
        BigInteger M = PotegowanieModularne(C, d, N);

        Console.WriteLine($"d = {d}");
        Console.WriteLine($"M = {M}");
    }
}