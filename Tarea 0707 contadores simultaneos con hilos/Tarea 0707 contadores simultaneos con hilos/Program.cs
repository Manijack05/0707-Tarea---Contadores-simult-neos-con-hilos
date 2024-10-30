using System;
using System.Collections.Generic;
using System.Threading;

namespace ContadoresSimultaneos
{
    class Program
    {
        static bool ejecutando = true;
        static Dictionary<int, (Thread, bool, int, int)> contadores = new Dictionary<int, (Thread, bool, int, int)>(); 
        static int contadorId = 1;

        static void Main()
        {
            while (ejecutando)
            {
                Console.Clear();
                Console.WriteLine("===== Menú de Contadores Simultáneos =====");
                Console.WriteLine("1. Crear un nuevo contador");
                Console.WriteLine("2. Iniciar un contador");
                Console.WriteLine("3. Detener un contador");
                Console.WriteLine("4. Mostrar contadores activos en tiempo real");
                Console.WriteLine("5. Salir");
                Console.Write("Seleccione una opción: ");
                var opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        CrearNuevoContador();
                        break;
                    case "2":
                        ListarContadores();
                        IniciarContador();
                        break;
                    case "3":
                        ListarContadores();
                        DetenerContador();
                        break;
                    case "4":
                        MostrarContadoresActivos();
                        break;
                    case "5":
                        Salir();
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Presione Enter para continuar.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        static void CrearNuevoContador()
        {
            Console.Write("Ingrese el intervalo de tiempo en milisegundos para el nuevo contador: ");
            if (int.TryParse(Console.ReadLine(), out int intervalo))
            {
                int id = contadorId++;
                bool activo = false;

                Thread contadorThread = new Thread(() => Contador(id, intervalo));
                contadores[id] = (contadorThread, activo, intervalo, intervalo);

                Console.WriteLine($"Contador {id} creado con intervalo de {intervalo} ms. Presione Enter para continuar.");
            }
            else
            {
                Console.WriteLine("Intervalo no válido. Presione Enter para continuar.");
            }
            Console.ReadLine();
        }

        static void ListarContadores()
        {
            Console.WriteLine("=== Lista de Contadores ===");
            foreach (var contador in contadores)
            {
                int id = contador.Key;
                var (_, activo, intervalo, tiempoRestante) = contador.Value;
                string estado = activo ? "Activo" : "Detenido";
                Console.WriteLine($"ID: {id} - Intervalo: {intervalo} ms - Estado: {estado} - Tiempo Restante: {tiempoRestante} ms");
            }
            Console.WriteLine();
        }

        static void IniciarContador()
        {
            Console.Write("Ingrese el ID del contador a iniciar: ");
            if (int.TryParse(Console.ReadLine(), out int id) && contadores.ContainsKey(id))
            {
                var (contadorThread, activo, intervalo, tiempoRestante) = contadores[id];

                if (!activo)
                {
                    activo = true;
                    contadorThread = new Thread(() => Contador(id, intervalo));
                    contadores[id] = (contadorThread, activo, intervalo, intervalo);
                    contadorThread.Start();

                    Console.WriteLine($"Contador {id} iniciado en segundo plano. Presione Enter para continuar.");
                }
                else
                {
                    Console.WriteLine("El contador ya está en ejecución. Presione Enter para continuar.");
                }
            }
            else
            {
                Console.WriteLine("ID de contador no encontrado. Presione Enter para continuar.");
            }
            Console.ReadLine();
        }

        static void DetenerContador()
        {
            Console.Write("Ingrese el ID del contador a detener: ");
            if (int.TryParse(Console.ReadLine(), out int id) && contadores.ContainsKey(id))
            {
                var (contadorThread, activo, intervalo, tiempoRestante) = contadores[id];

                if (activo)
                {
                    activo = false;
                    contadores[id] = (contadorThread, activo, intervalo, tiempoRestante);

                    Console.WriteLine($"Contador {id} detenido. Presione Enter para continuar.");
                }
                else
                {
                    Console.WriteLine("El contador ya está detenido. Presione Enter para continuar.");
                }
            }
            else
            {
                Console.WriteLine("ID de contador no encontrado. Presione Enter para continuar.");
            }
            Console.ReadLine();
        }

        static void MostrarContadoresActivos()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Estado de Contadores Activos ===");

                foreach (var contador in contadores)
                {
                    int id = contador.Key;
                    var (_, activo, intervalo, tiempoRestante) = contador.Value;

                    if (activo)
                    {
                        Console.WriteLine($"Contador {id}: Tiempo Restante: {tiempoRestante} ms");
                    }
                }

                Console.WriteLine("Presione Q para regresar al menú.");
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    break;
                }

                Thread.Sleep(500); 
            }
        }

        static void Contador(int id, int intervalo)
        {
            int tiempoRestante = intervalo;

            while (contadores[id].Item2 && tiempoRestante > 0)
            {
                tiempoRestante -= 100; 
                contadores[id] = (Thread.CurrentThread, true, intervalo, tiempoRestante);

                Thread.Sleep(100);
            }

            if (tiempoRestante <= 0)
            {
                contadores[id] = (Thread.CurrentThread, false, intervalo, 0);
                Console.WriteLine($"\nContador {id} ha llegado a 0 y se ha detenido automáticamente.");
            }
        }

        static void Salir()
        {
            ejecutando = false;
            foreach (var key in contadores.Keys)
            {
                var (contadorThread, activo, intervalo, tiempoRestante) = contadores[key];
                activo = false;
                contadores[key] = (contadorThread, activo, intervalo, tiempoRestante);
            }

            Console.WriteLine("Saliendo del programa... Todos los contadores han sido detenidos.");
            Console.ReadLine();
        }
    }
}
