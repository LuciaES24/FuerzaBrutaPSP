using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Diagnostics;

internal class Program
{
    static bool done;
    static readonly object locker = new object();
    private static void Main(string[] args)
    {
        try
        {
            done = false;
            //Leemos el archivo con las contraseñas
            StreamReader sr = new StreamReader("C:\\Users\\lucia\\Desktop\\2151220-passwords.txt");
            var Line = sr.ReadLine();

            //Guardamos todas las líneas del fichero
            List<string> Lines = new List<string>();

            while (Line != null)
            {
                Lines.Add(Line);
                //Read the next line
                Line = sr.ReadLine();
            }

            //Cerramos el archivo
            sr.Close();
            Console.ReadLine();

            //Generamos una contraseña aleatoria
            var Random = new Random();
            int Index = Random.Next(Lines.Count);

            var Password = Lines[Index];
            Console.WriteLine("Contraseña elegida: " + Password);

            //Dividimos las líneas del archivo para asignárselas a cada uno de los hilos
            int TotalLines = Lines.Count;
            int DivLines = TotalLines / 8;

            int Div1 = DivLines;
            int Div2 = DivLines * 2;
            int Div3 = DivLines * 3;
            int Div4 = DivLines * 4;
            int Div5 = DivLines * 5;
            int Div6 = DivLines * 6;
            int Div7 = DivLines * 7;
            int Div8 = DivLines * 8;


            //Creamos los hilos
            Thread hilo1 = new Thread(() => FuerzaBruta(0, Div1, Password, Lines));
            hilo1.Start();

            Thread hilo2 = new Thread(() => FuerzaBruta(Div1, Div2, Password, Lines));
            hilo2.Start();

            Thread hilo3 = new Thread(() => FuerzaBruta(Div2, Div3, Password, Lines));
            hilo3.Start();

            Thread hilo4 = new Thread(() => FuerzaBruta(Div3, Div4, Password, Lines));
            hilo4.Start();

            Thread hilo5 = new Thread(() => FuerzaBruta(Div4, Div5, Password, Lines));
            hilo5.Start();

            Thread hilo6 = new Thread(() => FuerzaBruta(Div5, Div6, Password, Lines));
            hilo6.Start();

            Thread hilo7 = new Thread(() => FuerzaBruta(Div6, Div7, Password, Lines));
            hilo7.Start();

            Thread hilo8 = new Thread(() => FuerzaBruta(Div7, Div8, Password, Lines));
            hilo8.Start();

            if (done == true)
            {
                lock (locker)
                {
                    hilo1.Abort();
                    hilo2.Abort();
                    hilo3.Abort();
                    hilo4.Abort();
                    hilo5.Abort();
                    hilo6.Abort();
                    hilo7.Abort();
                    hilo8.Abort();
                }
            }
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine("No se ha encontrado el archivo");
        }
        
    }

    //Encripta una cadena en SHA256 y la devuelve
    private static string GetSHA256(string str)
    {
        SHA256 sha256 = SHA256.Create();
        ASCIIEncoding encoding = new ASCIIEncoding();
        byte[] stream = null;
        StringBuilder sb = new StringBuilder();
        stream = sha256.ComputeHash(encoding.GetBytes(str));
        for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
        return sb.ToString();
    }

    //Cada hilo comparará las contraseñas encriptándolas con la elegida para descubrir si se encuentra en el archivo 
    //Para ello utilizamos una fila de inicio, otra de fin, la contraseña que se ha elegido y la lista con todas las del archivo
    private static void FuerzaBruta(int start, int finish, string password, List<String> lines)
    {
        //Calculamos el tiempo que tardan los hilos en descubrir la contraseña
        Stopwatch timeMeasure = new Stopwatch();
        timeMeasure.Start();

        for (int contaLines = start;  contaLines < finish; contaLines++)
        {
            if (GetSHA256(lines[contaLines]) == GetSHA256(password) && done==false)
            {
                var ResultPassword = lines[contaLines];
                Console.WriteLine("Contraseña encontrada: " + ResultPassword);
                Console.WriteLine("Contraseña encriptada: " + GetSHA256(password));
                timeMeasure.Stop();
                Console.WriteLine($"Tiempo: {timeMeasure.Elapsed.TotalSeconds} s");
                done = true;
                break;
            }
        }
    }
}