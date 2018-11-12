using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.RPG
{
    /// <summary>
    /// Makes sure all tiles are available at start.
    /// </summary>
    public static class EarlyTileInitalizer
    {
        public static void Initialize()
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(DefaultTiles).TypeHandle);
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(WaterTiles).TypeHandle);
        }
    }
}
