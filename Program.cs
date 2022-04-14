using System;
using System.Threading.Tasks;

namespace Tranzact_ProgrammingChallenge_JEACH
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No terms were specified for the Search Fight. Please execute again with the search terms.");
                return;
            }

            Console.WriteLine("Let's start a Search Fight....");
            foreach (string arg in args) { 
                Console.WriteLine(arg);
            }
        }
    }
}
