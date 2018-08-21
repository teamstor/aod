using System;
using TeamStor.Engine;

namespace TeamStor.RPG
{
    static class Program
    {
        static void Main()
        {
            using(Game game = Game.Run(new Menu.MenuState(), "data", false))
            {
                game.Run();
            }
        }
    }
}
