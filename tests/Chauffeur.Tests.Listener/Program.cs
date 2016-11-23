using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Chauffeur.Jenkins.Listeners;

namespace Chauffeur.Tests.Listener
{
    class Program
    {
        static void Main(string[] args)
        {
            ChaufferServiceListener listener = new ChaufferServiceListener("http://localhost:8080/test/");
            listener.Run((request) =>
            {
                Console.WriteLine(request.RawUrl);
            });

            Console.WriteLine("A simple webserver. Press a key to quit.");
            Console.ReadKey();

            listener.Stop();
        }
    }
}
