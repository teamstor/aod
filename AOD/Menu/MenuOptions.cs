using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.AOD.Menu.Elements;
using TeamStor.Engine;
using TeamStor.Engine.Coroutine;

namespace TeamStor.AOD.Menu
{
    /// <summary>
    /// Adds options to a MenuUI.
    /// </summary>
    public class MenuOptions
    {
        private MenuUI _ui;
        private MenuButton _fullscreenButton, _vsyncButton;

        private MenuButton _masterVolume, _musicVolume;

        public MenuOptions(MenuUI ui, string mainPage)
        {
            _ui = ui;

            MenuPage optionsPage = new MenuPage(150);
            _fullscreenButton = optionsPage.Add(new MenuButton(optionsPage, "Fullscreen: ?", "icons/fullscreen.png", "", "Displays the game over the\nwhole monitor")) as MenuButton;
            _fullscreenButton.RegisterEvent(MenuElement.EventType.Clicked, 
                (e, h) => { if(!h) ui.Parent.Game.Fullscreen = !ui.Parent.Game.Fullscreen; });

            _vsyncButton = optionsPage.Add(new MenuButton(optionsPage, "V-sync: ?", "icons/vsync.png", "", "Synchronizes the game to\nthe monitor")) as MenuButton;
            _vsyncButton.RegisterEvent(MenuElement.EventType.Clicked, 
                (e, h) => { if(!h) ui.Parent.Game.VSync = !ui.Parent.Game.VSync; });

            optionsPage.Add(new MenuSpacer(4));
            optionsPage.Add(new MenuButton(optionsPage, "Audio", "icons/audio.png", "", "", true)).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) ui.SwitchPage("__options/audio", false); });
            optionsPage.Add(new MenuButton(optionsPage, "Input", "icons/input.png", "", "", true)).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) ui.SwitchPage("__options/input", false); });
            optionsPage.Add(new MenuSpacer(4));
            optionsPage.Add(new MenuButton(optionsPage, "Back", "icons/arrow_left.png")).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) ui.SwitchPage(mainPage, true); });

            MenuPage audioPage = new MenuPage(150);
            _masterVolume = audioPage.Add(new MenuButton(audioPage, "Master Volume: Muted", "icons/audio.png")) as MenuButton;
            _musicVolume = audioPage.Add(new MenuButton(audioPage, "Music Volume: Muted", "icons/music.png")) as MenuButton;

            _masterVolume.RegisterEvent(MenuElement.EventType.Left, (e, h) =>
            {
                if(!h)
                    Settings.MasterVolume -= 10;
                if(Settings.MasterVolume < 0)
                    Settings.MasterVolume = 0;
            });
            _masterVolume.RegisterEvent(MenuElement.EventType.Right, (e, h) =>
            {
                if(!h)
                    Settings.MasterVolume += 10;
                if(Settings.MasterVolume > 100)
                    Settings.MasterVolume = 100;
            });

            _musicVolume.RegisterEvent(MenuElement.EventType.Left, (e, h) =>
            {
                if(!h)
                    Settings.MusicVolume -= 10;
                if(Settings.MusicVolume < 0)
                    Settings.MusicVolume = 0;
            });
            _musicVolume.RegisterEvent(MenuElement.EventType.Right, (e, h) =>
            {
                if(!h)
                    Settings.MusicVolume += 10;
                if(Settings.MusicVolume > 100)
                    Settings.MusicVolume = 100;
            });

            audioPage.Add(new MenuSpacer(4));
            audioPage.Add(new MenuButton(audioPage, "Back", "icons/arrow_left.png")).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) ui.SwitchPage("__options", true); });

            MenuPage inputPage = new MenuPage(150);

            inputPage.Add(new MenuSpacer(4));
            inputPage.Add(new MenuButton(inputPage, "Back", "icons/arrow_left.png")).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) ui.SwitchPage("__options", true); });

            ui.AddPage("__options", optionsPage);
            ui.AddPage("__options/audio", audioPage);
            ui.AddPage("__options/input", inputPage);
        }

        public IEnumerator<ICoroutineOperation> SwitchToOptionsPage()
        {
            return _ui.SwitchPage("__options");
        }

        public void UpdateLabels()
        {
            _fullscreenButton.Label = "Fullscreen: " + (_ui.Parent.Game.Fullscreen ? "On" : "Off");
            _vsyncButton.Label = "V-sync: " + (_ui.Parent.Game.VSync ? "On" : "Off");

            int masterVolumeCount = (int)Math.Floor(Settings.MasterVolume / 10.0);
            int musicVolumeCount = (int)Math.Floor(Settings.MusicVolume / 10.0);

            _masterVolume.Label = "Master Volume: ";
            for(int i = 0; i < masterVolumeCount; i++)
                _masterVolume.Label += "|";
            if(masterVolumeCount == 0)
                _masterVolume.Label += "Muted";

            _musicVolume.Label = "Music Volume: ";
            for(int i = 0; i < musicVolumeCount; i++)
                _musicVolume.Label += "|";
            if(musicVolumeCount == 0)
                _musicVolume.Label += "Muted";
        }
    }
}
