using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.RPG.Legacy
{
    public class OldMapLoader
    {
        /*public static Map Load(Stream stream)
        {
            Map.Information info = new Map.Information("???", Environment.Forest, Weather.Sunny);
            int width = 0, height = 0;

            using(BinaryReader reader = new BinaryReader(stream, Encoding.UTF8))
            {
                if(reader.ReadString() != "rpg:map")
                    throw new Exception("Not a valid map stream");

                int readVersion;
                if((readVersion = reader.ReadInt32()) != MAP_FORMAT_VERSION)
                    throw new Exception("Map loaded from " +
                                        (readVersion > MAP_FORMAT_VERSION ? "a newer" : "an older") +
                                        " version (" + MAP_FORMAT_VERSION + " != " + readVersion + ")");

                info.Name = reader.ReadString();
                info.Environment = (Environment)reader.ReadByte();
                info.Weather = (Weather)reader.ReadByte();

                width = reader.ReadInt32();
                height = reader.ReadInt32();

                Map map = new Map(width, height, info);
                for(int i = 0; i < width * height; i++)
                {
                    map._layerTerrain[i] = reader.ReadInt32();
                    if(!Tile.Exists((byte)(map._layerTerrain[i] & 0xff), Tile.MapLayer.Terrain))
                        map._layerTerrain[i] = 0;
                    map._layerDecoration[i] = reader.ReadInt32();
                    if(!Tile.Exists((byte)(map._layerDecoration[i] & 0xff), Tile.MapLayer.Decoration))
                        map._layerDecoration[i] = 0;
                    map._layerNPC[i] = reader.ReadInt32();
                    if(!Tile.Exists((byte)(map._layerNPC[i] & 0xff), Tile.MapLayer.NPC))
                        map._layerNPC[i] = 0;
                    map._layerControl[i] = reader.ReadInt32();
                    if(!Tile.Exists((byte)(map._layerControl[i] & 0xff), Tile.MapLayer.Control))
                        map._layerControl[i] = 0;
                }

                int count = reader.ReadInt32();
                for(int i = 0; i < count; i++)
                {
                    map._metadataTerrain.Add(new SortedDictionary<string, string>());
                    int pairs = reader.ReadInt32();

                    for(int i2 = 0; i2 < pairs; i2++)
                        map._metadataTerrain[map._metadataTerrain.Count - 1].Add(reader.ReadString(), reader.ReadString());
                }

                count = reader.ReadInt32();
                for(int i = 0; i < count; i++)
                {
                    map._metadataDecoration.Add(new SortedDictionary<string, string>());
                    int pairs = reader.ReadInt32();

                    for(int i2 = 0; i2 < pairs; i2++)
                        map._metadataDecoration[map._metadataDecoration.Count - 1].Add(reader.ReadString(), reader.ReadString());
                }

                count = reader.ReadInt32();
                for(int i = 0; i < count; i++)
                {
                    map._metadataNPC.Add(new SortedDictionary<string, string>());
                    int pairs = reader.ReadInt32();

                    for(int i2 = 0; i2 < pairs; i2++)
                        map._metadataNPC[map._metadataNPC.Count - 1].Add(reader.ReadString(), reader.ReadString());
                }

                count = reader.ReadInt32();
                for(int i = 0; i < count; i++)
                {
                    map._metadataControl.Add(new SortedDictionary<string, string>());
                    int pairs = reader.ReadInt32();

                    for(int i2 = 0; i2 < pairs; i2++)
                        map._metadataControl[map._metadataControl.Count - 1].Add(reader.ReadString(), reader.ReadString());
                }
                GC.Collect();

                return map;
            }
        }*/

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
                foreach(SortedDictionary<string, string> s in _metadataTerrain)
                {
                    writer.Write(s.Count);
                    foreach(KeyValuePair<string, string> pair in s)
                    {
                        writer.Write(pair.Key);
                        writer.Write(pair.Value);
                    }
                }

                writer.Write(_metadataDecoration.Count);
                foreach(SortedDictionary<string, string> s in _metadataDecoration)
                {
                    writer.Write(s.Count);
                    foreach(KeyValuePair<string, string> pair in s)
                    {
                        writer.Write(pair.Key);
                        writer.Write(pair.Value);
                    }
                }

                writer.Write(_metadataNPC.Count);
                foreach(SortedDictionary<string, string> s in _metadataNPC)
                {
                    writer.Write(s.Count);
                    foreach(KeyValuePair<string, string> pair in s)
                    {
                        writer.Write(pair.Key);
                        writer.Write(pair.Value);
                    }
                }

                writer.Write(_metadataControl.Count);
                foreach(SortedDictionary<string, string> s in _metadataControl)
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
