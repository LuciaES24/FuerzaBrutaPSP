using System.Security.Cryptography;
using System.Text;
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

            //Leemos todas las líneas del fichero
            var Lines = File.ReadAllLines("2151220-passwords.txt").ToList();

            //Generamos una contraseña aleatoria
            var Random = new Random();
            int Index = Random.Next(Lines.Count);

            var Password = Lines[Index];
            Console.WriteLine("Contraseña elegida: " + Password);

            //Dividimos las líneas del archivo para asignárselas a cada uno de los hilos
            int TotalLines = Lines.Count;
            int DivLines = TotalLines / 8;

            var numHilos = 8;
            var listaHilos = new List<Thread>();

            //Creamos todos los hilos y los iniciamos
            for (int i = 0; i <= numHilos; i++)
            {
                if (i == 8)
                {
                    break;
                }
                var hilo = new Thread(() => FuerzaBruta(DivLines*i, (DivLines*(i+1))-1, Password, Lines));
                hilo.Start();
                listaHilos.Add(hilo);
                Thread.Sleep(10);
            }

            //Si la tarea ha terminado, hacemos que terminen todos los hilos
            if (done == true)
            {
                lock (locker)
                {
                    foreach (Thread t in listaHilos)
                    {
                        t.Abort();
                    }
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

        for (int contaLines = start; contaLines < finish; contaLines++)
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
            }else if (done == true)
            {
                break;
            }
        }
    }
}