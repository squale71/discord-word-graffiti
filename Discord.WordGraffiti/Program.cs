using Discord.WordGraffiti.App_Start;
using System;

namespace Discord.WordGraffiti
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.Initialize().GetAwaiter().GetResult();
        }
    }
}
