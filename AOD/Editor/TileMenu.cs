using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using TeamStor.Engine.Graphics;
using TeamStor.Engine.Tween;
using TeamStor.AOD.Gameplay;
using Game = TeamStor.Engine.Game;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD.Editor
{
    /// <summary>
    /// Menu to select a tile from the global tile list.
    /// </summary>
    public class TileMenu
    {
        private bool _isScrolling = false;

        private bool _isSearching = false;
        private string _searchTerm = "";

        private bool _quitQueued = false;
        private Map.Environment _environment;
        private TweenedDouble _screenFade;
        private float _scrollTarget = -1;

        public Game Game;

        public bool CanInteractWith
        {
            get
            {
                return !_isSearching;
            }
        }

        public bool IsHovered
        {
            get
            {
                return !Disabled && (Rectangle.Value.Contains(Game.Input.MousePosition) || _isScrolling || _isSearching);
            }
        }

        /// <summary>
        /// If this menu is disabled.
        /// </summary>
        public bool Disabled = false;

        /// <summary>
        /// Special separator object for categories.
        /// </summary>
        public const string Separator = "separator";

        /// <summary>
        /// Top-level categories.
        /// </summary>
        public List<Category> Categories
        {
            get;
            private set;
        } = new List<Category>();

        /// <summary>
        /// Maximum height of this menu.
        /// </summary>
        public int MaxHeight { get; set; }

        /// <summary>
        /// Tile menu category. Can contain either tiles or sub-categories.
        /// If this is a sub-category (category in category) then the title will be smaller and less opaque.
        /// </summary>
        public class Category
        {
            public Category(string title, string icon)
            {
                Title = title;
                Icon = icon;
            }

            /// <summary>
            /// Header title shown.
            /// </summary>
            public string Title;

            /// <summary>
            /// Icon shown, this will be visible unless scrolled past.
            /// </summary>
            public string Icon;

            /// <summary>
            /// Sub-objects, either Tile, Category or TileMenu.Separator.
            /// </summary>
            public List<object> Objects = new List<object>();
        }

        /// <summary>
        /// Position of this menu.
        /// </summary>
        public TweenedRectangle Rectangle;

        /// <summary>
        /// Total area this tile menu could cover.
        /// </summary>
        public Vector2 TotalArea;

        /// <summary>
        /// Amount the user can scroll the menu.
        /// </summary>
        public int ScrollableAmount
        {
            get
            {
                return Math.Max(0, (int)TotalArea.Y - MaxHeight);
            }
        }

        /// <summary>
        /// Amount the user has scrolled.
        /// This can go over or under the minimum/maximum.
        /// </summary>
        public float Scroll;

        /// <summary>
        /// The currently selected tile.
        /// </summary>
        public Tile SelectedTile
        {
            get; private set;
        }

        public delegate void OnSelectionChanged(TileMenu menu, Tile newSelected);
        public OnSelectionChanged SelectionChanged;

        public TileMenu(Map.Environment environment, Engine.Game game)
        {
            _environment = environment;
            Game = game;

            Rectangle = new TweenedRectangle(game, new Rectangle(0, 0, (int)TotalArea.X, 28));
            _screenFade = new TweenedDouble(game, 0);

            Recreate();
        }

        private void Recreate()
        {
            Categories.Clear();
            Categories.Add(new Category(_environment == Map.Environment.Inside ? "General" : "Water",
                 _environment == Map.Environment.Inside ? "editor/tilemenu/general.png" : "editor/tilemenu/water.png"));
            Categories.Last().Objects.Add(WaterTiles.DeepWaterOrVoid);
            Categories.Last().Objects.Add(WaterTiles.ShallowWater);

            Categories.Add(new Category("Ground", "editor/tilemenu/ground.png"));
            Categories.Last().Objects.Add(GroundTiles.Grass);
            Categories.Last().Objects.Add(GroundTiles.Dirt);
            Categories.Last().Objects.Add(GroundTiles.Stone);
            Categories.Last().Objects.Add(GroundTiles.StoneDarker);
            Categories.Last().Objects.Add(GroundTiles.Gravel);

            Categories.Add(new Category("Foliage", "editor/tilemenu/foliage.png"));
            Categories.Last().Objects.Add(FoliageTiles.Tree);
            Categories.Last().Objects.Add(FoliageTiles.Birch);
            Categories.Last().Objects.Add(FoliageTiles.Bush);
            Categories.Last().Objects.Add(FoliageTiles.TallGrass);
            Categories.Last().Objects.Add(FoliageTiles.Rock);

            Categories.Add(new Category("City", "editor/tilemenu/city.png"));

            Category walls = new Category("Wall", "");
            walls.Objects.Add(CityTiles.Wall);
            walls.Objects.Add(CityTiles.Bricks);
            walls.Objects.Add(CityTiles.StoneBricks);
            walls.Objects.Add(Separator);
            walls.Objects.Add(CityTiles.WallBeam);
            Categories.Last().Objects.Add(walls);

            Category roofs = new Category("Roof", "");
            roofs.Objects.Add(CityTiles.RoofBlue);
            roofs.Objects.Add(CityTiles.RoofOrange);
            roofs.Objects.Add(CityTiles.RoofRed);
            roofs.Objects.Add(CityTiles.RoofPurple);
            roofs.Objects.Add(Separator);
            roofs.Objects.Add(CityTiles.RoofPurpleTip);
            roofs.Objects.Add(CityTiles.RoofBeam);
            Categories.Last().Objects.Add(roofs);

            Category windows = new Category("Window", "");
            windows.Objects.Add(CityTiles.WindowBlue);
            windows.Objects.Add(CityTiles.WindowOrange);
            windows.Objects.Add(CityTiles.WindowRed);
            windows.Objects.Add(CityTiles.WindowPurple);
            windows.Objects.Add(Separator);
            windows.Objects.Add(CityTiles.WindowStone);
            Categories.Last().Objects.Add(windows);

            Categories.Last().Objects.Add(CityTiles.HouseStairs);
            Categories.Last().Objects.Add(CityTiles.Door);
            Categories.Last().Objects.Add(CityTiles.DoorStone);

            Categories.Last().Objects.Add(Separator);

            Categories.Last().Objects.Add(CityTiles.Road);
            Categories.Last().Objects.Add(CityTiles.Sign);
            Categories.Last().Objects.Add(CityTiles.Waterwheel);

            Categories.Add(new Category("Inside", "editor/tilemenu/inside.png"));
            Categories.Last().Objects.Add(InsideTiles.Floor);
            Categories.Last().Objects.Add(InsideTiles.Wall);
            Categories.Last().Objects.Add(InsideTiles.Panel);

            Categories.Last().Objects.Add(Separator);

            Categories.Last().Objects.Add(InsideTiles.Door);
            Categories.Last().Objects.Add(InsideTiles.Doormat);

            Categories.Last().Objects.Add(Separator);
            Categories.Last().Objects.Add(InsideTiles.Bookshelf);
            Categories.Last().Objects.Add(InsideTiles.Chair);
            Categories.Last().Objects.Add(InsideTiles.Table);

            Categories.Add(new Category("Control", "editor/tilemenu/control.png"));
            Categories.Last().Objects.Add(ControlTiles.Spawnpoint);
            Categories.Last().Objects.Add(ControlTiles.MapPortal);
            Categories.Last().Objects.Add(Separator);
            Categories.Last().Objects.Add(ControlTiles.TextBox);
            Categories.Last().Objects.Add(Separator);
            Categories.Last().Objects.Add(ControlTiles.Barrier);
            Categories.Last().Objects.Add(ControlTiles.InvertedBarrier);

            Categories.Add(new Category("NPCs", "editor/tilemenu/npcs.png"));

            Category animals = new Category("Animals", "");
            animals.Objects.Add(NPCs.Pig.TileTemplate);
            animals.Objects.Add(NPCs.GreenPig.TileTemplate);

            Categories.Last().Objects.Add(animals);

            SelectedTile = WaterTiles.DeepWaterOrVoid;

            TotalArea = new Vector2(0, 0);

            // icons
            TotalArea.X += 18 + 10;

            foreach(Category category in Categories)
            {
                Point measure = CalculateCategorySize(category, true);
                TotalArea.X = Math.Max(18 + 10 + measure.X + 40, TotalArea.X);
                TotalArea.Y += measure.Y;
            }

            TotalArea.X += 16;

            Rectangle.TweenTo(new Rectangle(Rectangle.TargetValue.X, Rectangle.TargetValue.Y, (int)TotalArea.X, Rectangle.TargetValue.Height), TweenEaseType.Linear, 0);
        }

        private void OnSearchInput(char c)
        {
            if(c == (char) 22)
                _searchTerm += SDL.SDL_GetClipboardText();
            if(c == '\b' && _searchTerm.Length > 0)
                _searchTerm = _searchTerm.Substring(0, _searchTerm.Length - 1);
            else if(Char.IsLetterOrDigit(c) || Char.IsPunctuation(c) || c == ' ')
                _searchTerm += c;

            _searchTerm = _searchTerm.TrimStart();
        }

        private void RemoveSearchInput(object sender, Game.ChangeStateEventArgs e)
        {
            TextInputEXT.TextInput -= OnSearchInput;
            TextInputEXT.StopTextInput();
            Game.OnStateChange -= RemoveSearchInput;
        }

        private Point CalculateCategorySize(Category category, bool first)
        {
            Point size = new Point(0, (first ? 16 : 14) + 10);

            for(int i = 0; i < category.Objects.Count; i++)
            {
                if(i != 0)
                    size.Y += 4;

                if(category.Objects[i] is Category)
                {
                    Point s = CalculateCategorySize(category.Objects[i] as Category, false);
                    size.X = Math.Max(size.X, s.X);
                    size.Y += s.Y;
                }
                else if((category.Objects[i] as string) == Separator)
                    size.Y += 8;
                else
                {
                    if(Map.Atlas == null)
                        Map.Atlas = new TileAtlas(Game);

                    string layer = (category.Objects[i] as Tile).Layer.ToString();
                    Vector2 measure = Game.DefaultFonts.Bold.Measure(16, (category.Objects[i] as Tile).Name(null, _environment) + "\n" + layer);
                    size.X = Math.Max(size.X, Map.Atlas[(category.Objects[i] as Tile).TextureName(null, _environment)].Rectangle.Width * 2 + 4 + (int)measure.X);
                    size.Y += Math.Max((int)measure.Y, Map.Atlas[(category.Objects[i] as Tile).TextureName(null, _environment)].Rectangle.Height * 2);
                }
            }

            size.Y += 10;
            return size;
        }

        private Tile Search(string s, Category c)
        {
            Tile tile = null;

            foreach(object o in c.Objects)
            {
                if(o is Tile)
                {
                    if((o as Tile).Name(null, _environment).ToLowerInvariant().Trim().Replace(" ", "") == s)
                        return (o as Tile);

                    if((o as Tile).Name(null, _environment).ToLowerInvariant().Trim().Replace(" ", "").StartsWith(s))
                        tile = (o as Tile);
                }

                if(o is Category)
                {
                    Tile searched = Search(s, (o as Category));
                    if(searched != null)
                        tile = searched;
                }
            }

            return tile;
        }

        public void Update(Game game)
        {
            if(!Disabled && ((Rectangle.Value.Contains(game.Input.MousePosition) && !Rectangle.Value.Contains(game.Input.PreviousMousePosition)) || (Game.Input.KeyPressed(Keys.S) && !_isSearching)))
            {
                Rectangle.TweenTo(new Rectangle(Rectangle.TargetValue.X, Rectangle.TargetValue.Y, (int)TotalArea.X, Math.Min((int)TotalArea.Y, MaxHeight)), TweenEaseType.EaseOutQuad, 0.1f);
                _screenFade.TweenTo(1, TweenEaseType.EaseOutQuad, 0.1f);

                if(Game.Input.KeyPressed(Keys.S))
                {
                    _isSearching = true;
                    _quitQueued = true;

                    TextInputEXT.TextInput += OnSearchInput;
                    TextInputEXT.StartTextInput();
                    game.OnStateChange += RemoveSearchInput;
                }
            }
            else if(_quitQueued || (!Rectangle.Value.Contains(game.Input.MousePosition) && Rectangle.Value.Contains(game.Input.PreviousMousePosition)))
            {
                if(_isScrolling || _isSearching)
                    _quitQueued = true;
                else
                {
                    if(!Rectangle.Value.Contains(game.Input.MousePosition))
                    {
                        Rectangle.TweenTo(new Rectangle(Rectangle.TargetValue.X, Rectangle.TargetValue.Y, (int)TotalArea.X, 28), TweenEaseType.EaseOutQuad, 0.1f);
                        _screenFade.TweenTo(0, TweenEaseType.EaseOutQuad, 0.1f);
                    }

                    _quitQueued = false;
                }
            }

            if(_isSearching && game.Input.KeyPressed(Keys.Tab))
            {
                string s = _searchTerm.ToLowerInvariant().Trim().Replace(" ", "");

                foreach(Category category in Categories)
                {
                    Tile searched = Search(s, category);

                    if(searched != null)
                    {
                        _searchTerm = searched.Name(null, _environment);
                        break;
                    }
                }
            }

            if(_isSearching && game.Input.KeyPressed(Keys.Enter))
            {
                _searchTerm = _searchTerm.ToLowerInvariant().Trim().Replace(" ", "");

                foreach(Category category in Categories)
                {
                    Tile searched = Search(_searchTerm, category);
                    if(searched != null)
                    {
                        SelectedTile = searched;
                        if(SelectionChanged != null)
                            SelectionChanged(this, SelectedTile);
                    }
                }

                _isSearching = false;
                _searchTerm = "";

                TextInputEXT.TextInput -= OnSearchInput;
                TextInputEXT.StopTextInput();
                game.OnStateChange -= RemoveSearchInput;
            }
            else if(_isSearching && game.Input.KeyPressed(Keys.Escape))
            {
                _isSearching = false;
                _searchTerm = "";
                TextInputEXT.TextInput -= OnSearchInput;
                TextInputEXT.StopTextInput();
                game.OnStateChange -= RemoveSearchInput;
            }

            if(IsHovered && CanInteractWith)
            {
                float changed = _scrollTarget;
                
                if(game.Input.KeyPressed(Keys.Home))
                    _scrollTarget = 0;

                if(game.Input.KeyPressed(Keys.PageUp))
                    _scrollTarget = Scroll - 180;
                if(game.Input.KeyPressed(Keys.PageDown))
                    _scrollTarget = Scroll + 180;

                if(game.Input.KeyPressed(Keys.End))
                    _scrollTarget = ScrollableAmount;

                if(changed != _scrollTarget)
                {
                    if(_scrollTarget < 0)
                        _scrollTarget = 0;
                    if(_scrollTarget > ScrollableAmount)
                        _scrollTarget = ScrollableAmount;
                }
            }

            if(CanInteractWith)
            {
                if(_scrollTarget != -1)
                {
                    Scroll = MathHelper.Lerp(Scroll, _scrollTarget, (float)Game.DeltaTime * 14f);
                    if(Math.Abs(Scroll - _scrollTarget) <= 0.75f)
                        _scrollTarget = -1;
                }
                else if(Rectangle.Value.Contains(game.Input.MousePosition))
                {
                    if((game.Input.MouseScroll < 0 && Scroll < ScrollableAmount) ||
                        (game.Input.MouseScroll > 0 && Scroll > 0))
                        Scroll -= game.Input.MouseScroll / 4f;

                    if(game.Input.Key(Keys.Up) && Scroll > 0)
                        Scroll = Math.Max(0, Scroll - ((float)game.DeltaTime * 140f));
                    if(game.Input.Key(Keys.Down) && Scroll < ScrollableAmount)
                        Scroll = Math.Min(ScrollableAmount, Scroll + ((float)game.DeltaTime * 140f));
                }

                if(_isScrolling)
                {
                    float at = game.Input.MousePosition.Y - (Rectangle.Value.Top + 8);
                    float percentage = at / (Rectangle.Value.Height - 16);
                    Scroll = percentage * ScrollableAmount;

                    if(game.Input.MouseReleased(Engine.MouseButton.Left))
                        _isScrolling = false;
                }

                Scroll = MathHelper.Clamp(Scroll, 0, ScrollableAmount);
            }
        }

        private void DrawCategory(Engine.Game game, Category category, bool first, ref int y)
        {
            game.Batch.Text(SpriteBatch.FontStyle.Bold, (first ? (uint)16 : 14), category.Title, new Vector2(0, y), Color.White * _screenFade);
            y += (first ? 16 : 14) + 10;

            for(int i = 0; i < category.Objects.Count; i++)
            {
                if(i != 0)
                    y += 4;

                if(category.Objects[i] is Category)
                    DrawCategory(game, category.Objects[i] as Category, false, ref y);
                else if((category.Objects[i] as string) == Separator)
                    y += 8;
                else
                {
                    string layer = (category.Objects[i] as Tile).Layer.ToString();
                    Vector2 measure = Game.DefaultFonts.Bold.Measure(16, (category.Objects[i] as Tile).Name(null, _environment) + "\n" + layer);
                    TileAtlas.Region region = Map.Atlas[(category.Objects[i] as Tile).TextureName(null, _environment)];

                    bool isTextBigger = measure.Y > region.Rectangle.Height * 2;
                    int textureY = isTextBigger ? y + (int)(measure.Y / 2 - region.Rectangle.Height) : y;
                    int textY = isTextBigger ? y : y + (int)(region.Rectangle.Height - measure.Y / 2);

                    Rectangle fullRectangle = new Rectangle(
                        Rectangle.Value.X + 8 + 28,
                        (int)-Scroll + Rectangle.Value.Y + 6 + y,
                        Rectangle.Value.Width - 8 - 48,
                        Math.Max((int)measure.Y, region.Rectangle.Height * 2));

                    if(fullRectangle.Contains(game.Input.MousePosition) ||
                        SelectedTile == (category.Objects[i] as Tile))
                        game.Batch.Rectangle(
                            new Rectangle(-4, y - 2, fullRectangle.Width + 4, fullRectangle.Height + 4), 
                            Color.White * (SelectedTile == (category.Objects[i] as Tile) ? 0.15f : 0.1f));

                    if(fullRectangle.Contains(game.Input.MousePosition) &&
                        SelectedTile != (category.Objects[i] as Tile) &&
                        game.Input.MousePressed(Engine.MouseButton.Left) &&
                        CanInteractWith)
                    {
                        SelectedTile = (category.Objects[i] as Tile);

                        if(SelectionChanged != null)
                            SelectionChanged(this, SelectedTile);
                    }

                    game.Batch.Texture(
                        new Rectangle(0, textureY, region.Rectangle.Width * 2, region.Rectangle.Height * 2),
                        region.Texture,
                        Color.White * _screenFade, region.Rectangle);

                    game.Batch.Text(SpriteBatch.FontStyle.Bold, 14, (category.Objects[i] as Tile).Name(null, _environment), new Vector2(
                        region.Rectangle.Width * 2 + 12, textY), Color.White * 0.6f * _screenFade);
                    game.Batch.Text(SpriteBatch.FontStyle.Bold, 14, layer, new Vector2(
                        region.Rectangle.Width * 2 + 12, textY + 16), Color.White * 0.3f * _screenFade);

                    y += Math.Max((int)measure.Y, region.Rectangle.Height * 2);
                }
            }

            y += 10;
        }

        public void Draw(Engine.Game game)
        {
            game.Batch.Scissor = Rectangle;

            if(!_isSearching)
                game.Batch.Rectangle(Rectangle, Color.Black * 0.85f);

            if(_screenFade != 1.0f)
            {
                game.Batch.Text(SpriteBatch.FontStyle.Bold, 15, SelectedTile.Name(null, _environment), new Vector2(Rectangle.Value.X + 8, Rectangle.Value.Y + 4),
                    Color.White * (1.0f - _screenFade) * (Disabled ? 0.3f : 0.6f));
                
                Rectangle tileRect = new Rectangle(Rectangle.Value.X + Rectangle.Value.Width - 4 - 16 - 2, Rectangle.Value.Y + 6, 16, 16);
                if(game.Assets.Get<Texture2D>(SelectedTile.TextureName(null, _environment)).Height > game.Assets.Get<Texture2D>(SelectedTile.TextureName(null, _environment)).Width)
                {
                    tileRect.Width /= 2;
                    tileRect.X += 8;
                }
                
                game.Batch.Outline(tileRect, Color.White * (1.0f - _screenFade) * 0.6f, 1, false);
                game.Batch.Texture(tileRect, game.Assets.Get<Texture2D>(SelectedTile.TextureName(null, _environment)), Color.White * (1.0f - _screenFade));
            }
            if(_screenFade != 0.0f)
            {
                Matrix oldTransform = game.Batch.Transform;
                game.Batch.Transform = Matrix.CreateTranslation(
                    Rectangle.Value.X + 8 + 18 + 10, 
                    (int)-Scroll + Rectangle.Value.Y + 6, 
                    0);

                int y = 0;

                foreach(Category category in Categories)
                    DrawCategory(game, category, true, ref y);

                game.Batch.Transform = oldTransform;

                int cY = 8;
                int categoryTop = 8 - (int)Scroll;
                foreach(Category category in Categories)
                {
                    Rectangle rectC = new Rectangle(
                        Rectangle.Value.X + 8, 
                        Rectangle.Value.Y + cY /* - (categoryTop < cY ? cY - categoryTop : 0) */, 
                        18, 
                        18);
                    bool hoverC = rectC.Contains(game.Input.MousePosition);

                    game.Batch.Texture(rectC, game.Assets.Get<Texture2D>(category.Icon), Color.White * _screenFade * (hoverC ? 1.0f : 0.6f));

                    if(game.Input.MousePressed(Engine.MouseButton.Left) && hoverC && CanInteractWith)
                        _scrollTarget = Math.Min(ScrollableAmount, categoryTop + (int)Scroll - 8);

                    /*if(categoryTop < cY)
                        cY -= Math.Min(cY - categoryTop, 24); */

                    cY += 24;
                    categoryTop += CalculateCategorySize(category, true).Y;
                }

                if(ScrollableAmount > 0)
                {
                    float scrollBarHeight = 1000 / ScrollableAmount;
                    if(scrollBarHeight > Rectangle.Value.Height - 16)
                        scrollBarHeight = Rectangle.Value.Height - 16;
                    if(scrollBarHeight < 20)
                        scrollBarHeight = 20;

                    Rectangle fullScrollRectangle = new Rectangle(Rectangle.TargetValue.Right - 18, Rectangle.TargetValue.Y + 8, 20, Rectangle.TargetValue.Height - 16);

                    if((fullScrollRectangle.Contains(game.Input.MousePosition) || _isScrolling) && CanInteractWith)
                    {
                        fullScrollRectangle.X = Rectangle.TargetValue.Right - 10;
                        fullScrollRectangle.Width = 2;

                        game.Batch.Rectangle(fullScrollRectangle, Color.White * 0.4f * _screenFade);

                        if(game.Input.MousePressed(Engine.MouseButton.Left) && !_isScrolling && CanInteractWith)
                            _isScrolling = true;
                    }

                    game.Batch.Rectangle(new Rectangle(
                        Rectangle.TargetValue.Right - 10,
                        (int)(MathHelper.Lerp(Rectangle.TargetValue.Y + 8, Rectangle.TargetValue.Bottom - 8 - scrollBarHeight, Scroll / ScrollableAmount)),
                        2,
                        (int)scrollBarHeight),
                        Color.White * 0.5f * _screenFade);
                }

                Rectangle rect = new Rectangle(
                    Rectangle.Value.X + 8,
                    Rectangle.TargetValue.Bottom - 8 - 18,
                    18,
                    18);
                bool hover = rect.Contains(game.Input.MousePosition);

                game.Batch.Texture(rect, game.Assets.Get<Texture2D>("editor/tilemenu/search.png"), Color.White * _screenFade * (hover ? 1.0f : 0.6f));

                if(game.Input.MousePressed(Engine.MouseButton.Left) && hover && CanInteractWith)
                {
                    _isSearching = true;
                    _quitQueued = true;

                    TextInputEXT.TextInput += OnSearchInput;
                    TextInputEXT.StartTextInput();
                    game.OnStateChange += RemoveSearchInput;
                }
            }

            if(_isSearching)
            {
                game.Batch.Rectangle(Rectangle, Color.Black * 0.85f);

                string text = _searchTerm;
                Vector2 measure = game.DefaultFonts.MonoBold.Measure(14, text + "|");
                text += ((int)((game.Time * 4) % 2) == 0 ? "|" : "");

                game.Batch.Text(SpriteBatch.FontStyle.MonoBold, 14, text, Rectangle.Value.Center.ToVector2() - measure / 2, Color.White);
            }

            game.Batch.Scissor = null;
        }
    }
}