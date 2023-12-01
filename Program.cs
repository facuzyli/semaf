using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Semaforo
{
    /// <summary>
    /// Esta clase sirve para...
    /// </summary>
    class Program
    {
        private static SemaphoreSlim SemaphoreUnoVerde = new SemaphoreSlim(0, 3);
        private static SemaphoreSlim SemaphoreUnoRojo = new SemaphoreSlim(0, 2);

        private static SemaphoreSlim SemaphoreDosVerde = new SemaphoreSlim(0, 1);
        private static SemaphoreSlim SemaphoreDosRojo = new SemaphoreSlim(0, 4);

        private static SemaphoreSlim SemaphoreTresVerde = new SemaphoreSlim(0, 1);
        private static SemaphoreSlim SemaphoreTresRojo = new SemaphoreSlim(0, 4);

        static void Main(string[] args)
        {
            Thread TC = new Thread(Controller);

            List<SemaphoreSlim> configS1 = new List<SemaphoreSlim>();
            configS1.Add(SemaphoreUnoVerde);
            configS1.Add(SemaphoreDosRojo);
            configS1.Add(SemaphoreTresRojo);


            //Thread T1 = new Thread(S1);
            Thread T2 = new Thread(S2);
            Thread T3 = new Thread(S3);
            Thread T4 = new Thread(S4);
            Thread T5 = new Thread(S5);

            Thread T1 = new Thread(SemaforoThread);
            T1.Name = "S1";
            T1.Start(configS1);

            //T1.Start();
            T2.Start();
            T3.Start();
            T4.Start();
            T5.Start();

            Thread.Sleep(1000);
            Console.WriteLine("Esperando que el controller empiece a secuenciar la programación");
            Thread.Sleep(2000);            

            TC.Start();
        }

        public static void S1()
        {
            while (true)
            {
                Console.WriteLine("S1 esperando verde");
                //if(!SemaphoreUnoVerde.Wait(5000)) //Evaluar política en caso de que el
                //controller no se comunique...
                SemaphoreUnoVerde.Wait();

                Console.WriteLine("S1 esperando rojo");
                SemaphoreDosRojo.Wait();

                Console.WriteLine("S1 esperando rojo");
                SemaphoreTresRojo.Wait();
            }
        }

        public static void S2()
        {
            while (true)
            {
                Console.WriteLine("S2 esperando verde");
                SemaphoreUnoVerde.Wait();

                Console.WriteLine("S2 esperando rojo");
                SemaphoreDosRojo.Wait();

                Console.WriteLine("S2 esperando rojo");
                SemaphoreTresRojo.Wait();
            }
        }

        public static void S3()
        {
            while (true)
            {
                Console.WriteLine("S3 esperando verde");
                SemaphoreUnoVerde.Wait();

                Console.WriteLine("S3 esperando rojo");
                SemaphoreDosRojo.Wait();

                Console.WriteLine("S3 esperando rojo");
                SemaphoreTresRojo.Wait();
            }
        }

        /// <summary>
        /// Este método...
        /// </summary>
        public static void S4()
        {
            while (true)
            {
                Console.WriteLine("S4 esperando rojo");
                SemaphoreUnoRojo.Wait();

                Console.WriteLine("S4 esperando verde");
                SemaphoreDosVerde.Wait();

                Console.WriteLine("S4 esperando rojo");
                SemaphoreTresRojo.Wait();
            }
        }

        public static void S5()
        {
            while (true)
            {
                Console.WriteLine("S5 esperando rojo");
                SemaphoreUnoRojo.Wait();

                Console.WriteLine("S5 esperando rojo");
                SemaphoreDosRojo.Wait();

                Console.WriteLine("S5 esperando verde");
                SemaphoreTresVerde.Wait();
            }
        }

        /// <summary>
        /// Tengo un solo método para aceptar cualquier CONFIG de un semáforo...
        /// </summary>
        /// <param name="semaforoConf">Enviar en un lista la configuración de las esperas...</param>
        public static void SemaforoThread(object semaforoConf)
        {
            List<SemaphoreSlim> semaforoConfig = semaforoConf as List<SemaphoreSlim>;

            while (true)
            {
                foreach (var item in semaforoConfig)
                {
                    Console.WriteLine("Soy semáforo " + Thread.CurrentThread.Name);
                    item.Wait();
                }
            }
        }

        public static void Controller()
        {
            while (true)
            {
                Console.WriteLine("Paso 1: Se liberan 3 verdes y 2 rojos.");

                SemaphoreUnoVerde.Release(3);
                SemaphoreUnoRojo.Release(2);

                Thread.Sleep(2000);

                Console.WriteLine("Paso 2: Se liberan 1 verde y 4 rojos.");
                SemaphoreDosVerde.Release(1);

                //if(SemaphoreDosVerde.CurrentCount > 0)? //Tomar alguna decisión de por qué hay hilos libres cuando no debería
                SemaphoreDosRojo.Release(4);

                Thread.Sleep(2000);

                Console.WriteLine("Paso 3: Se liberan 1 verde y 4 rojos.");
                SemaphoreTresVerde.Release(1);
                SemaphoreTresRojo.Release(4);

                Thread.Sleep(2000);
            }
        }
    }
}
