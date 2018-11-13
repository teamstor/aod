using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine.Tween;

using Game = TeamStor.Engine.Game;

namespace TeamStor.RPG.Editor
{
    /// <summary>
    /// Menu to select a tile from the global tile list.
    /// </summary>
    public class TileMenu
    {
        private Map.Environment _environment;
        public Game Game;

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
        /// The currently selected tile.
        /// </summary>
        public Tile SelectedTile
        {
            get; private set;
        }

        public TileMenu(Map.Environment environment, Engine.Game game)
        {
            _environment = environment;
            Game = game;

            Categories.Add(new Category(environment == Map.Environment.Inside ? "General" : "Water",
                environment == Map.Environment.Inside ? "editor/tilemenu/general.png" : "editor/tilemenu/water.png"));
            Categories.Last().Objects.Add(WaterTiles.DeepWaterOrVoid);
            Categories.Last().Objects.Add(WaterTiles.ShallowWater);

            Categories.Add(new Category("Ground", "editor/tilemenu/ground.png"));
            Categories.Last().Objects.Add(GroundTiles.Grass);
            Categories.Last().Objects.Add(GroundTiles.Dirt);
            Categories.Last().Objects.Add(GroundTiles.Stone);

            Categories.Add(new Category("Foliage", "editor/tilemenu/foliage.png"));
            Categories.Last().Objects.Add(FoliageTiles.Tree);
            Categories.Last().Objects.Add(FoliageTiles.Bush);

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
            windows.Objects.Add(InsideTiles.Floor);
            windows.Objects.Add(InsideTiles.Wall);
            windows.Objects.Add(InsideTiles.Panel);

            windows.Objects.Add(Separator);

            windows.Objects.Add(InsideTiles.Door);
            windows.Objects.Add(InsideTiles.Doormat);

            Categories.Add(new Category("Control", "editor/tilemenu/control.png"));
            Categories.Last().Objects.Add(ControlTiles.Spawnpoint);
            Categories.Last().Objects.Add(ControlTiles.MapPortal);
            Categories.Last().Objects.Add(Separator);
            Categories.Last().Objects.Add(ControlTiles.TextBox);
            Categories.Last().Objects.Add(Separator);
            Categories.Last().Objects.Add(ControlTiles.Barrier);
            Categories.Last().Objects.Add(ControlTiles.InvertedBarrier);

            SelectedTile = WaterTiles.DeepWaterOrVoid;

            // icons
            TotalArea.X += 18 + 10;

            foreach(Category category in Categories)
            {
                Point measure = CalculateCategorySize(category, true);
                TotalArea.X = Math.Max(18 + 10 + measure.X, TotalArea.X);
                TotalArea.Y += measure.Y;
            }

            TotalArea.X += 8;
            TotalArea.Y += 8;

            Rectangle = new TweenedRectangle(game, new Rectangle(0, 0, (int)TotalArea.X, 24));
        }

        private Point CalculateCategorySize(Category category, bool first)
        {
            Point size = new Point(0, (first ? 24 : 20) + 10);

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

                    Vector2 measure = Game.DefaultFonts.Bold.Measure(16, (category.Objects[i] as Tile).Name(null, _environment));
                    size.X = Math.Max(size.X, Map.Atlas[(category.Objects[i] as Tile).TextureName(null, _environment)].Rectangle.Width * 2 + 4 + (int)measure.X);
                    size.Y += Math.Max((int)measure.Y, Map.Atlas[(category.Objects[i] as Tile).TextureName(null, _environment)].Rectangle.Height * 2);
                }
            }

            return size;
        }

        public void Draw(Engine.Game game)
        {
            game.Batch.Scissor = Rectangle;

            game.Batch.Rectangle(Rectangle, Color.Black * 0.85f);

            game.Batch.Scissor = null;
        }
    }
}