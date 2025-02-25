using System;
using System.Collections.Generic;
using System.Net.Http;//Permite hacer peticiones HTTP a APIs en internet.
using System.Threading.Tasks;//Permite usar operaciones asíncronas (async/await).
using System.Text.Json;//Permite trabajar con JSON (para leer la respuesta de la API).
using System.Diagnostics;//Se usa para abrir el navegador con ChatGPT.

class Program
{
    static Dictionary<string, string> dictionary = new Dictionary<string, string>
    {
        {"hola", "hello"}, {"adiós", "goodbye"}, {"gracias", "thank you"}, {"por favor", "please"},
        {"sí", "yes"}, {"no", "no"}, {"bien", "fine"}, {"mal", "bad"}, {"feliz", "happy"},
        {"triste", "sad"}, {"rápido", "fast"}, {"lento", "slow"}
    };//diccionario en c# que almacena pares clave-valor

    static async Task Main()
    {
        while (true)
        {
            Console.Write("\n🔹 Ingresa una palabra en español (0 para salir): ");
            string palabra = Console.ReadLine().ToLower();

            if (palabra == "0")
            {
                Console.WriteLine("👋 Saliendo...");
                break;
            }

            string traduccion;
            if (dictionary.ContainsKey(palabra))
            {
                traduccion = dictionary[palabra];
                Console.WriteLine($"✅ Traducción: {traduccion}");
            }
            else
            {
                Console.WriteLine("⚠️ Palabra no encontrada en el diccionario. Buscando en la API...");
                traduccion = await Traducir(palabra, "en");

                if (!string.IsNullOrEmpty(traduccion))
                {
                    Console.WriteLine($"🌍 Traducción automática: {traduccion}");
                }
                else
                {
                    Console.WriteLine("❌ No se encontró traducción.");
                    continue;
                }
            }

            // Preguntar si quiere abrir ChatGPT
            Console.Write("\n💬 ¿Quieres abrir ChatGPT con la traducción? (sí/no): ");
            string respuesta = Console.ReadLine().ToLower();

            if (respuesta == "sí" || respuesta == "si")
            {
                AbrirChatGPT(traduccion);
            }
        }
    }

    static async Task<string> Traducir(string palabra, string idioma)
    {
        string url = $"https://api.mymemory.translated.net/get?q={palabra}&langpair=es|{idioma}";
        using (HttpClient client = new HttpClient())
        {
            string response = await client.GetStringAsync(url);

            // Extraer solo la traducción usando JSON
            using (JsonDocument json = JsonDocument.Parse(response))
            {
                var root = json.RootElement;
                return root.GetProperty("responseData").GetProperty("translatedText").GetString().Trim();
            }
        }
    }

    static void AbrirChatGPT(string traduccion)
    {
        string url = $"https://chat.openai.com/?q={Uri.EscapeDataString(traduccion)}";
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true //habilitar el uso en windows.
            });
            Console.WriteLine("🌐 Abriendo ChatGPT en tu navegador...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al abrir el navegador: {ex.Message}");
        }
    }
}




