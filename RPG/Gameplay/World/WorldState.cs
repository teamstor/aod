using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;
using Microsoft.Xna.Framework.Graphics;

using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;
using Microsoft.Xna.Framework.Input;
using TeamStor.Engine.Coroutine;
using Game = TeamStor.Engine.Game;
using TeamStor.Engine.Tween;

namespace TeamStor.RPG.Gameplay.World
{
    /// <summary>
    /// Gameplay state the game is in while the player is walking around in the world.
    /// </summary>
    public class WorldState : GameState
    {
        private bool _debug;
        private bool _hasDrawnName = false;
        private float _drawNameAlpha = 0;
        
        /// <summary>
        /// The player.
        /// </summary>
        public Player Player
        {
            get; private set;
        }

        /// <summary>
        /// The camera following the player.
        /// </summary>
        public Camera Camera
        {
            get; private set;
        }

        private List<NPC> _npcs = new List<NPC>();

        /// <summary>
        /// NPCs in the world.
        /// </summary>
        public IReadOnlyList<NPC> NPCs
        {
            get
            {
                return _npcs;
            }
        }
        
        public class DrawEventArgs : EventArgs
        {
            public SpriteBatch Batch;
            public Vector2 ScreenSize;
        }

        public event EventHandler<Game.UpdateEventArgs> UpdateHook;
        public event EventHandler<DrawEventArgs> DrawHook;

        // Used when spawning the player.
        public struct SpawnArgs
        {
            public SpawnArgs(Point position, Direction direction)
            {
                Position = position;
                Direction = direction;
            }

            /// <summary>
            /// Position the player will spawn at. (-1, -1) = default
            /// </summary>
            public Point Position;

            /// <summary>
            /// Direction the player will spawn in.
            /// </summary>
            public Direction Direction;
        }

        private SpawnArgs _spawnArgs;

        /// <summary>
        /// Spawns a new NPC in the world.
        /// </summary>
        /// <param name="template">The template to create the NPC from.</param>
        /// <param name="position">The position to spawn the NPC at.</param>
        /// <returns>The newly spawned NPC.</returns>
        public NPC SpawnNPC(NPCTemplate template, Point position)
        {
            NPC npc = new NPC(this, template);
            npc.MoveInstantly(position);
            return npc;
        }

        /// <summary>
        /// Map of the world.
        /// </summary>
        public Map Map
        {
            get; private set;
        }

        /// <summary>
        /// If the world should be paused.
        /// </summary>
        public bool Paused { get; set; }

        private TweenedDouble _transitionCover;
        private bool _useTransiton = false;

        public WorldState(Map map, SpawnArgs spawnArgs, bool transition = false)
        {
            Map = map;
            _spawnArgs = spawnArgs;
            _useTransiton = transition;
        }

        public WorldState(Map map, bool transition = false) : this(map, new SpawnArgs(new Point(-1, -1), Direction.Down), transition) { }

        public override void OnEnter(GameState previousState)
        {
            Player = new Player(this);

            if(_spawnArgs.Position != new Point(-1, -1))
                Player.MoveInstantly(_spawnArgs.Position);

            Player.Heading = _spawnArgs.Direction;

            if(_useTransiton)
            {
                _transitionCover = new TweenedDouble(Game, 1);
                _transitionCover.TweenTo(0, TweenEaseType.Linear, 0.4);
                Paused = true;
            }
            else
            {
                _transitionCover = new TweenedDouble(Game, 0);
                Coroutine.Start(ShowMapName);
            }

            Camera = new Camera(this);

            Game.IsMouseVisible = false;

            if(Map.TransitionCache != null)
                Map.TransitionCache.Clear();
        }

        public override void OnLeave(GameState nextState)
        {
        }

        public override void Update(double deltaTime, double totalTime, long count)
        {
            if(Input.Key(Keys.LeftShift) && Input.KeyPressed(Keys.F5))
                _debug = !_debug;

            if(_useTransiton && _transitionCover.IsComplete && Paused)
            {
                _useTransiton = Paused = false;
                Coroutine.Start(ShowMapName);
            }

            if(!Paused)
            {
                Point lastPlayerPos = Player.Position;
                Player.Update(deltaTime, totalTime, count);
                Camera.Update(deltaTime);

                foreach(Tile.MapLayer layer in Tile.CachedAllMapLayers)
                {
                    TileEventBase oldEvents = Map[layer, lastPlayerPos.X, lastPlayerPos.Y].Events;
                    TileEventBase newEvents = Map[layer, Player.Position.X, Player.Position.Y].Events;

                    if(Player.Position != lastPlayerPos)
                    {
                        oldEvents?.OnWalkLeave(Map.GetMetadata(layer, lastPlayerPos.X, lastPlayerPos.Y), this, lastPlayerPos);
                        newEvents?.OnWalkEnter(Map.GetMetadata(layer, Player.Position.X, Player.Position.Y), this, Player.Position);
                    }
                }
            }
            
            if(UpdateHook != null)
                UpdateHook(this, new Game.UpdateEventArgs(deltaTime, totalTime, count));
        }

        public override void FixedUpdate(long count)
        {
            if(!Paused)
            {
                foreach(Tile.MapLayer layer in Tile.CachedAllMapLayers)
                {
                    TileEventBase events = Map[layer, Player.Position.X, Player.Position.Y].Events;
                    events?.OnStandingOn(Map.GetMetadata(layer, Player.Position.X, Player.Position.Y), this, Player.Position, count);
                }
            }
        }

        private string DebugTileString(Point position)
        {
            if(position.X < 0 || position.Y < 0 || position.X >= Map.Width || position.Y >= Map.Height)
                return "(nothing)";

            string str = "";

            foreach(Tile.MapLayer layer in Tile.CachedAllMapLayers)
            {
                SortedDictionary<string, string> metadata = Map.GetMetadata(layer, position.X, position.Y);
                str += layer + " - " + 
                       "\"" + Map[layer, position.X, position.Y].Name(metadata, Map.Info.Environment) + "\" " + 
                       "(id " + Map[layer, position.X, position.Y] + ") " +
                       " metadata: ";

                if(metadata == null || metadata.Count == 0)
                    str += "(none)";
                else
                {
                    foreach(KeyValuePair<string, string> pair in metadata)
                        str += "\n    " + pair.Key + ": " + pair.Value;
                }

                str += "\n";
            }

            return str;
        }

        public override void Draw(SpriteBatch batch, Vector2 screenSize)
        {
            screenSize = Program.ScaleBatch(batch);

            batch.Rectangle(new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.Black);

            batch.SamplerState = SamplerState.PointWrap;
            Matrix oldTransform = batch.Transform;
            batch.Transform = Matrix.CreateTranslation(Camera.Offset.X, Camera.Offset.Y, 0) * batch.Transform;
            
            Map.Draw(Tile.MapLayer.Terrain, Game, new Rectangle((int)-Camera.Offset.X, (int)-Camera.Offset.Y, (int)screenSize.X, (int)screenSize.Y));

            foreach(NPC npc in NPCs)
                npc.Draw(batch);
            Player.Draw(batch);

            Map.Draw(Tile.MapLayer.Decoration, Game, new Rectangle((int)-Camera.Offset.X, (int)-Camera.Offset.Y, (int)screenSize.X, (int)screenSize.Y));
         
            if(_debug)
            {
                batch.Outline(new Rectangle(Player.Position.X * 16, Player.Position.Y * 16, 16, 16), Color.Red);
                batch.Outline(new Rectangle(Player.NextPosition.X * 16, Player.NextPosition.Y * 16, 16, 16), Color.Green);
                batch.Outline(new Rectangle(Player.NextPosition.X * 16 + Player.Heading.ToPoint().X * 16, Player.NextPosition.Y * 16 + Player.Heading.ToPoint().Y * 16, 16, 16), Color.Blue);
            }

            batch.Transform = oldTransform;

            Rectangle transitionRectangle = Rectangle.Empty;
            if(Player.Heading == Direction.Up)
                transitionRectangle = new Rectangle(0, 0, 480, (int)(270 * _transitionCover));
            if(Player.Heading == Direction.Down)
                transitionRectangle = new Rectangle(0, (int)(270 * (1.0 - _transitionCover)), 480, 270);

            Vector2 measure = Assets.Get<Font>("fonts/Alkhemikal.ttf").Measure(16, Map.Info.Name);

            batch.Text(Assets.Get<Font>("fonts/Alkhemikal.ttf"), 16, Map.Info.Name, 
                new Vector2(screenSize.X / 2 - measure.X / 2, 40), 
                Color.Goldenrod * _drawNameAlpha);

            batch.Line(new Vector2(screenSize.X / 2 - measure.X / 2 - 4, 40 + measure.Y + 2),
                new Vector2(screenSize.X / 2 + measure.X / 2 + 4, 40 + measure.Y + 2),
                Color.Goldenrod * _drawNameAlpha);

            //batch.Rectangle(transitionRectangle, Color.Black);
            batch.Rectangle(new Rectangle(0, 0, 480, 270), Color.Black * _transitionCover);

            if(DrawHook != null)
                DrawHook(this, new DrawEventArgs { Batch = batch, ScreenSize = screenSize });
                                
            Program.BlackBorders(batch);

            if(_debug)
            {
                Rectangle rectangle = new Rectangle(4, 4, 0, 0);
                
                rectangle.Width = Math.Max(rectangle.Width, (int)Game.DefaultFonts.MonoBold.Measure(12, DebugTileString(Player.Position)).X);
                if(Player.NextPosition != Player.Position)
                    rectangle.Width = Math.Max(rectangle.Width, (int)Game.DefaultFonts.MonoBold.Measure(12, DebugTileString(Player.NextPosition)).X);
                rectangle.Width = Math.Max(rectangle.Width, (int)Game.DefaultFonts.MonoBold.Measure(12, DebugTileString(Player.NextPosition + Player.Heading.ToPoint())).X);

                rectangle.Height += (int)Game.DefaultFonts.MonoBold.Measure(12, DebugTileString(Player.Position)).Y;
                rectangle.Height += 8;
                
                if(Player.NextPosition != Player.Position)
                {
                    rectangle.Height += (int) Game.DefaultFonts.MonoBold.Measure(12, DebugTileString(Player.NextPosition)).Y;
                    rectangle.Height += 8;
                }

                rectangle.Height += (int)Game.DefaultFonts.MonoBold.Measure(12, DebugTileString(Player.NextPosition + Player.Heading.ToPoint())).Y;

                rectangle.Width += 8;
                rectangle.Height += 8;
                
                batch.Rectangle(rectangle, Color.Black * 0.7f);

                int y = 8;

                if(Player.NextPosition == Player.Position)
                {
                    batch.Text(SpriteBatch.FontStyle.MonoBold, 12, DebugTileString(Player.Position), new Vector2(8, y), Color.Green);
                    y += (int) Game.DefaultFonts.MonoBold.Measure(12, DebugTileString(Player.Position)).Y + 8;
                }
                else
                {
                    batch.Text(SpriteBatch.FontStyle.MonoBold, 12, DebugTileString(Player.Position), new Vector2(8, y), Color.Red);
                    y += (int) Game.DefaultFonts.MonoBold.Measure(12, DebugTileString(Player.Position)).Y + 8;
                    batch.Text(SpriteBatch.FontStyle.MonoBold, 12, DebugTileString(Player.NextPosition), new Vector2(8, y), Color.Green);
                    y += (int) Game.DefaultFonts.MonoBold.Measure(12, DebugTileString(Player.NextPosition)).Y + 8;
                }
                
                batch.Text(SpriteBatch.FontStyle.MonoBold, 12, DebugTileString(Player.NextPosition + Player.Heading.ToPoint()), new Vector2(8, y), Color.Blue);
            }
        }

        private IEnumerator<ICoroutineOperation> ShowMapName()
        {
            yield return Wait.Seconds(Game, 0.1);

            while(_drawNameAlpha < 1.0f)
            {
                _drawNameAlpha += (float)Game.DeltaTime * 8;
                yield return null;
            }

            _drawNameAlpha = 1.0f;

            yield return Wait.Seconds(Game, 2);

            while(_drawNameAlpha > 0.0f)
            {
                _drawNameAlpha -= (float)Game.DeltaTime * 8;
                yield return null;
            }

            _drawNameAlpha = 0.0f;

            _hasDrawnName = true;
        }

        private IEnumerator<ICoroutineOperation> WaitForTransition(GameState state)
        {
            yield return Wait.Seconds(Game, 0.8);
            Game.CurrentState = state;
        }

        /// <summary>
        /// Transitions to a new map with this existing player.
        /// </summary>
        /// <param name="newMap">The new map to start in.</param>
        /// <param name="transition">If a transition should be used when going between the maps.</param>
        /// <param name="spawnArgs">Used for determining where the player will spawn.</param>
        public void TransitionToMap(Map newMap, bool transition, SpawnArgs? spawnArgs = null)
        {
            // TODO: transition
            WorldState newState = 
                spawnArgs.HasValue ? new WorldState(newMap, spawnArgs.Value, transition) : 
                new WorldState(newMap, transition);

            if(transition)
            {
                _transitionCover.TweenTo(1, TweenEaseType.Linear, 0.4);
                Paused = true;
                Coroutine.AddExisting(WaitForTransition(newState));
            }
            else
                Game.CurrentState = newState;
        }
    }
}
