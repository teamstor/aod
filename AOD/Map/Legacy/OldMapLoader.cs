using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.AOD.Legacy
{
    public class OldMapLoader
    {
        /// <summary>
        /// Old map version.
        /// </summary>
        public const int MAP_LEGACY_FORMAT_VERSION = 1;

        public static Map Load(Stream stream)
        {
            Map.Information info = new Map.Information("???", Map.Environment.Forest, Map.Weather.Sunny);
            int width = 0, height = 0;

            using(BinaryReader reader = new BinaryReader(stream, Encoding.UTF8))
            {
                if(reader.ReadString() != "rpg:map")
                    throw new Exception("Not a valid map stream");

                int readVersion;
                if((readVersion = reader.ReadInt32()) != MAP_LEGACY_FORMAT_VERSION)
                    throw new Exception("Map loaded from " +
                                        (readVersion > MAP_LEGACY_FORMAT_VERSION ? "a newer" : "an older") +
                                        " version (" + MAP_LEGACY_FORMAT_VERSION + " != " + readVersion + ")");

                info.Name = reader.ReadString();
                info.Environment = (Map.Environment)reader.ReadByte();
                info.Weather = (Map.Weather)reader.ReadByte();

                width = reader.ReadInt32();
                height = reader.ReadInt32();

                Map map = new Map(width, height, info);
                int x, y;
                for(y = 0; y < height; y++)
                {
                    for(x = 0; x < width; x++)
                    {
                        Tile terrainTile = WaterTiles.DeepWaterOrVoid;
                        if(MapConverterIDMap.TerrainMap.TryGetValue(reader.ReadInt32() & 0xff, out terrainTile))
                            map[Tile.MapLayer.Terrain, x, y] = terrainTile;

                        Tile decorationTile = DefaultTiles.EmptyDecoration;
                        if(MapConverterIDMap.DecorationMap.TryGetValue(reader.ReadInt32() & 0xff, out decorationTile))
                            map[Tile.MapLayer.Decoration, x, y] = decorationTile;

                        Tile npcTile = DefaultTiles.EmptyNPC;
                        if(MapConverterIDMap.NPCMap.TryGetValue(reader.ReadInt32() & 0xff, out npcTile))
                            map[Tile.MapLayer.NPC, x, y] = npcTile;

                        Tile controlTile = DefaultTiles.EmptyControl;
                        if(MapConverterIDMap.ControlMap.TryGetValue(reader.ReadInt32() & 0xff, out controlTile))
                            map[Tile.MapLayer.Control, x, y] = controlTile;
                    }
                }

                return map;
            }
        }

        /*public void Save(Stream stream)
        {
            using(BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                writer.Write("rpg:map");
                writer.Write(MAP_FORMAT_VERSION);

                writer.Write(Info.Name);
                writer.Write((byte)Info.Environment);
                writer.Write((byte)Info.Weather);

                writer.Write(Width);
                writer.Write(Height);

                for(int i = 0; i < Width * Height; i++)
                {
                    writer.Write(_layerTerrain[i]);
                    writer.Write(_layerDecoration[i]);
                    writer.Write(_layerNPC[i]);
                    writer.Write(_layerControl[i]);
                }

                writer.Write(_metadataTerrain.Count);
                foreach(TileMetadata s in _metadataTerrain)
                {
                    writer.Write(s.Count);
                    foreach(KeyValuePair<string, string> pair in s)
                    {
                        writer.Write(pair.Key);
                        writer.Write(pair.Value);
                    }
                }

                writer.Write(_metadataDecoration.Count);
                foreach(TileMetadata s in _metadataDecoration)
                {
                    writer.Write(s.Count);
                    foreach(KeyValuePair<string, string> pair in s)
                    {
                        writer.Write(pair.Key);
                        writer.Write(pair.Value);
                    }
                }

                writer.Write(_metadataNPC.Count);
                foreach(TileMetadata s in _metadataNPC)
                {
                    writer.Write(s.Count);
                    foreach(KeyValuePair<string, string> pair in s)
                    {
                        writer.Write(pair.Key);
                        writer.Write(pair.Value);
                    }
                }

                writer.Write(_metadataControl.Count);
                foreach(TileMetadata s in _metadataControl)
                {
                    writer.Write(s.Count);
                    foreach(KeyValuePair<string, string> pair in s)
                    {
                        writer.Write(pair.Key);
                        writer.Write(pair.Value);
                    }
                }
            }
        }*/
    }
}
