using System;
using System.IO;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using SharpFont;

namespace TeamStor.AOD
{
	/// <summary>
	/// Global game settings.
	/// </summary>
	public static class Settings
	{
		/// <summary>
		/// If the game should be displayed in fullscreen.
		/// </summary>
		public static bool Fullscreen = false;

        /// <summary>
        /// If the game should use v-sync.
        /// </summary>
        public static bool VSync = true;
		
		/// <summary>
		/// Volume of all audio in the game.
		/// </summary>
		public static int MasterVolume = 50;
		
		/// <summary>
		/// Volume of music relative to the master volume.
		/// </summary>
		public static int MusicVolume = 100;
		
		/// <summary>
		/// Windows = %appdata%/teamstor/aod
		/// Mac = ~/teamstor/aod
		/// Linux = $XDG_CONFIG_HOME/teamstor/aod
		/// 		~/.config/teamstor/aod
		/// 		~/teamstor/aod
		/// Default = ./teamstor/aod
		/// </summary>
		public static string SettingsDirectory
		{
			get
			{
				switch(Environment.OSVersion.Platform)
				{
					case PlatformID.Win32NT:
						return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\teamstor\\aod";
					
					case PlatformID.Unix:
					case PlatformID.MacOSX:
						if(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME") != null)
							return Environment.GetEnvironmentVariable("XDG_CONFIG_HOME") + "/teamstor/aod/";
						else if(Environment.GetEnvironmentVariable("HOME") != null)
						{
							if(Directory.Exists(Environment.GetEnvironmentVariable("HOME") + "/.config"))
								return Environment.GetEnvironmentVariable("HOME") + "/.config/teamstor/aod";
							else
								return Environment.GetEnvironmentVariable("HOME") + "/teamstor/aod";
						}
						break;
				}
				
				return "./teamstor/aod";
			}
		}
		
		/// <summary>
		/// Loads settings from settings.json if the file exists.
		/// </summary>
		public static void Load()
		{
			string settingsFile = SettingsDirectory + "/settings.json";

			if(File.Exists(settingsFile))
			{
				using(StreamReader sreader = new StreamReader(File.OpenRead(settingsFile), System.Text.Encoding.UTF8))
				{
					using(JsonReader reader = new JsonTextReader(sreader))
					{
						while(reader.Read())
						{
							if(reader.TokenType == JsonToken.PropertyName)
							{
								string token = (string)reader.Value;
								if(token == "fullscreen")
									Fullscreen = reader.ReadAsString() == "true";
                                if(token == "vsync")
                                    VSync = reader.ReadAsString() == "true";

                                if(token == "volume")
								{
									while(reader.Read())
									{
										if(reader.TokenType == JsonToken.PropertyName)
										{
											token = (string)reader.Value;

											if(token == "master")
												MasterVolume = reader.ReadAsInt32().Value;

											if(token == "music")
												MusicVolume = reader.ReadAsInt32().Value;
										}
										else if(reader.TokenType == JsonToken.EndObject)
											break;
									}
									break;
								}

                                if(token == "keyboard")
                                {
	                                while(reader.Read())
	                                {
		                                if(reader.TokenType == JsonToken.PropertyName)
		                                {
			                                token = (string)reader.Value;

			                                foreach(InputAction action in Enum.GetValues(typeof(InputAction)))
			                                {
				                                if(token == action.ToString().ToLowerInvariant())
					                                InputMap.UpdateMappingKey(action, (Keys)Enum.Parse(typeof(Keys), reader.ReadAsString(), true));
			                                }
		                                }
		                                else if(reader.TokenType == JsonToken.EndObject)
			                                break;
	                                }
	                                break;
                                }
                                
                                if(token == "gamepad")
                                {
	                                while(reader.Read())
	                                {
		                                if(reader.TokenType == JsonToken.PropertyName)
		                                {
			                                token = (string)reader.Value;

			                                foreach(InputAction action in Enum.GetValues(typeof(InputAction)))
			                                {
				                                if(token == action.ToString().ToLowerInvariant())
					                                InputMap.UpdateMappingGamepad(action, (Buttons)Enum.Parse(typeof(Buttons), reader.ReadAsString(), true));
			                                }
		                                }
		                                else if(reader.TokenType == JsonToken.EndObject)
			                                break;
	                                }
	                                break;
                                }
							}
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Saves settings to settings.json.
		/// </summary>
		public static void Save()
		{
			string settingsFile = SettingsDirectory + "/settings.json";

			if(File.Exists(settingsFile))
				File.Delete(settingsFile);

			if(!Directory.Exists(SettingsDirectory))
				// creates directories recursively
				Directory.CreateDirectory(SettingsDirectory);

			using(StreamWriter swriter = new StreamWriter(File.OpenWrite(settingsFile), System.Text.Encoding.UTF8))
			{
				using(JsonWriter writer = new JsonTextWriter(swriter))
				{
					writer.Formatting = Formatting.Indented;
					
					writer.WriteStartObject();
					writer.WritePropertyName("fullscreen");
					writer.WriteValue(Fullscreen ? "true" : "false");
                    writer.WritePropertyName("vsync");
                    writer.WriteValue(VSync ? "true" : "false");

                    writer.WritePropertyName("volume");
					writer.WriteStartObject();
					writer.WritePropertyName("master");
					writer.WriteValue(MasterVolume);
					writer.WritePropertyName("music");
					writer.WriteValue(MusicVolume);
					writer.WriteEndObject();
					
					writer.WritePropertyName("keyboard");
					writer.WriteStartObject();
					
					foreach(InputAction action in Enum.GetValues(typeof(InputAction)))
					{
						writer.WritePropertyName(action.ToString().ToLowerInvariant());
						writer.WriteValue(InputMap.FindMapping(action).Key.ToString().ToLowerInvariant());
					}
					
					writer.WriteEndObject();
					
					writer.WritePropertyName("gamepad");
					writer.WriteStartObject();
					
					foreach(InputAction action in Enum.GetValues(typeof(InputAction)))
					{
						writer.WritePropertyName(action.ToString().ToLowerInvariant());
						writer.WriteValue(InputMap.FindMapping(action).GamepadButton.ToString().ToLowerInvariant());
					}
					
					writer.WriteEndObject();
					
					writer.WriteEndObject();
				}
			}
		}
	}
}