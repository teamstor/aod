using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using TeamStor.Engine;
using TeamStor.Engine.Coroutine;
using TeamStor.Engine.Graphics;
using TeamStor.Engine.Tween;
using Game = TeamStor.Engine.Game;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD.Gameplay.World
{
    /// <summary>
    /// Gameplay state the game is in while the player is walking around in the world.
    /// </summary>
    public class WorldState : GameState
    {
        private bool _debug;
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
            public SpawnArgs(Point position, Direction direction, Player oldPlayer)
            {
                Position = position;
                Direction = direction;
                OldPlayer = oldPlayer;
            }

            /// <summary>
            /// Position the player will spawn at. (-1, -1) = default
            /// </summary>
            public Point Position;

            /// <summary>
            /// Direction the player will spawn in.
            /// </summary>
            public Direction Direction;
            
            /// <summary>
            /// Player to copy over.
            /// </summary>
            public Player OldPlayer;
        }

        private SpawnArgs _spawnArgs;

        private Queue<NPC> _npcDespawnQueue = new Queue<NPC>();

        /// <summary>
        /// Spawns a new NPC in the world.
        /// </summary>
        /// <param name="template">The template to create the NPC from.</param>
        /// <param name="position">The position to spawn the NPC at.</param>
        /// <returns>The newly spawned NPC.</returns>
        public NPC SpawnNPC(NPCTemplate template, Point position)
        {
            NPC npc = new NPC(this, position, template);
            _npcs.Add(npc);

            npc.MoveInstantly(position);
            npc.Template.Behavior.OnSpawned(npc);
            return npc;
        }

        /// <summary>
        /// Despawns the following NPC at the end of the world tick.
        /// </summary>
        /// <param name="npc">The NPC to despawn.</param>
        public void DespawnNPC(NPC npc)
        {
            if(!_npcDespawnQueue.Contains(npc))
            {
                npc.Template.Behavior.OnDespawned(npc, false);
                _npcDespawnQueue.Enqueue(npc);
            }
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

        private static RenderTarget2D _transitionRenderTarget;

        private TweenedDouble _transition;
        private bool _useTransiton = false;

        private enum TransitionType
        {
            Normal,
            CombatIn,
            CombatOut
        }

        private TransitionType _transitionType = TransitionType.Normal;

        private Point _lastPlayerPos;

        public WorldState(Map map, SpawnArgs spawnArgs, bool transition = false)
        {
            Map = map;
            _spawnArgs = spawnArgs;
            _useTransiton = transition;
        }

        public WorldState(Map map, bool transition = false) : this(map, new SpawnArgs(new Point(-1, -1), Direction.Down, null), transition) { }

        public override void OnEnter(GameState previousState)
        {
            if(_transitionRenderTarget == null)
                _transitionRenderTarget = new RenderTarget2D(Game.GraphicsDevice, 480, 270, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            Player = new Player(this);

            if(_spawnArgs.Position != new Point(-1, -1))
                Player.MoveInstantly(_spawnArgs.Position);

            _lastPlayerPos = Player.Position;

            Player.Heading = _spawnArgs.Direction;

            for(int x = 0; x < Map.Width; x++)
            {
                for(int y = 0; y < Map.Height; y++)
                {
                    if(Map[Tile.MapLayer.NPC, x, y] != DefaultTiles.EmptyNPC)
                        SpawnNPC(NPCTemplate.FromTile(Map[Tile.MapLayer.NPC, x, y]), new Point(x, y));
                }
            }

            if(_useTransiton)
            {
                _transition = new TweenedDouble(Game, 1);
                _transition.TweenTo(0, TweenEaseType.Linear, 0.4);
                Paused = true;
            }
            else
            {
                _transition = new TweenedDouble(Game, 0);
                Coroutine.Start(ShowMapName);
            }

            Camera = new Camera(this);

            Game.IsMouseVisible = false;

            if(Map.TransitionCache != null)
                Map.TransitionCache.Clear();
            if(Map.Atlas != null)
                Map.Atlas.Clear();
        }

        public override void OnLeave(GameState nextState)
        {
            foreach(NPC npc in _npcs)
                npc.Template.Behavior.OnDespawned(npc, true);

            _npcs.Clear();
        }

        public override void Update(double deltaTime, double totalTime, long count)
        {
            if(Input.Key(Keys.LeftShift) && Input.KeyPressed(Keys.F5))
                _debug = !_debug;

            if(_useTransiton && _transition.IsComplete && Paused)
            {
                _useTransiton = Paused = false;
                Coroutine.Start(ShowMapName);
            }

            if(!Paused)
            {
                Player.Update(deltaTime, totalTime, count);

                foreach(NPC npc in NPCs)
                    npc.Update(deltaTime, totalTime, count);

                foreach(Tile.MapLayer layer in Tile.CachedAllMapLayers)
                {
                    TileEventBase oldEvents = Map[layer, _lastPlayerPos.X, _lastPlayerPos.Y].Events;
                    TileEventBase newEvents = Map[layer, Player.Position.X, Player.Position.Y].Events;

                    if(Player.Position != _lastPlayerPos)
                    {
                        if(Map.Info.CombatArea)
                            RandomEncounter.TryRandomEncounter(this);

                        oldEvents?.OnWalkLeave(Map.GetMetadata(layer, _lastPlayerPos.X, _lastPlayerPos.Y), this, _lastPlayerPos);
                        newEvents?.OnWalkEnter(Map.GetMetadata(layer, Player.Position.X, Player.Position.Y), this, Player.Position);
                    }
                }

                _lastPlayerPos = Player.Position;
            }

            Camera.Update(deltaTime);

            while(_npcDespawnQueue.Count > 0)
            {
                NPC npc = _npcDespawnQueue.Dequeue();
                _npcs.Remove(npc);
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
                TileMetadata metadata = Map.GetMetadata(layer, position.X, position.Y);
                str += layer + " - " + 
                       "\"" + Map[layer, position.X, position.Y].Name(metadata, Map.Info.Environment) + "\" " + 
                       "(id '" + Map[layer, position.X, position.Y].ID + "') " +
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

        public bool IsPointBlocked(Point point)
        {
            foreach(NPC npc in NPCs)
            {
                if(npc.Position == point || npc.NextPosition == point)
                    return true;
            }

            return Map.IsPointBlocked(point) || Player.Position == point || Player.NextPosition == point;
        }

        private void DrawToScreenOrRenderTarget(SpriteBatch batch, Vector2 screenSize, RenderTarget2D target)
        {
            if(target != null)
            {
                batch.SamplerState = SamplerState.PointClamp;
                batch.RenderTarget = target;
            }
            else
                screenSize = Program.ScaleBatch(batch);

            batch.Rectangle(new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.Black);

            batch.SamplerState = SamplerState.PointWrap;
            Matrix oldTransform = batch.Transform;
            batch.Transform = Matrix.CreateTranslation(batch.SmartRound(Camera.Offset).X, batch.SmartRound(Camera.Offset).Y, 0) * batch.Transform;

            Map.Draw(Tile.MapLayer.Terrain, Game, new Rectangle((int)-Camera.Offset.X, (int)-Camera.Offset.Y, (int)screenSize.X, (int)screenSize.Y));

            foreach(NPC npc in NPCs)
                npc.Draw(batch);

            // we start by drawing the lower half on the body between the terrain layer and the decoration layer
            if(!Player.IsWalking)
                Player.Draw(batch, false);

            Map.Draw(Tile.MapLayer.Decoration, Game, new Rectangle((int)-Camera.Offset.X, (int)-Camera.Offset.Y, (int)screenSize.X, (int)screenSize.Y));

            // if the player is walking we draw everything over the decoration layer so he won't be covered by tall grass, etc
            if(Player.IsWalking)
                Player.Draw(batch, false);

            // the upper half is drawn over the decoration layer since the head should cover trees and other tiles on that layer
            Player.Draw(batch, true);

            if(_debug)
            {
                batch.Outline(new Rectangle(Player.Position.X * 16, Player.Position.Y * 16, 16, 16), Color.Red);
                batch.Outline(new Rectangle(Player.NextPosition.X * 16, Player.NextPosition.Y * 16, 16, 16), Color.Green);
                batch.Outline(new Rectangle(Player.NextPosition.X * 16 + Player.Heading.ToPoint().X * 16, Player.NextPosition.Y * 16 + Player.Heading.ToPoint().Y * 16, 16, 16), Color.Blue);
            }

            batch.Transform = oldTransform;

            Font alkhemikal = Assets.Get<Font>("fonts/Alkhemikal.ttf");
            Font poco = Assets.Get<Font>("fonts/Poco.ttf");

            Color gold = new Color(255, 194, 0);

            Rectangle transitionRectangle = Rectangle.Empty;
            if(Player.Heading == Direction.Up)
                transitionRectangle = new Rectangle(0, 0, 480, (int)(270 * _transition));
            if(Player.Heading == Direction.Down)
                transitionRectangle = new Rectangle(0, (int)(270 * (1.0 - _transition)), 480, 270);

            Vector2 measure = alkhemikal.Measure(16, Map.Info.Name);

            batch.Text(alkhemikal, 16, Map.Info.Name,
                new Vector2(screenSize.X / 2 - measure.X / 2, 40),
                gold * _drawNameAlpha);

            batch.Line(new Vector2(screenSize.X / 2 - measure.X / 2 - 4, 40 + measure.Y + 2),
                new Vector2(screenSize.X / 2 + measure.X / 2 + 4, 40 + measure.Y + 2),
                gold * _drawNameAlpha);

            int maxWidth = 80;
            foreach(Quest q in Player.Quests)
            {
                measure = poco.Measure(10, q.Name);
                if(measure.X > maxWidth)
                    maxWidth = (int)measure.X;
                
                measure = poco.Measure(10, q.Objective, 0.8f);
                if(measure.X > maxWidth)
                    maxWidth = (int)measure.X;
            }

            int y = 60;
            foreach(Quest q in Player.Quests)
            {
                measure = poco.Measure(10, q.Name);
                string texture = q.Fulfilled ? "quest/complete.png" : "quest/active.png";
                
                batch.Texture(new Vector2(screenSize.X - 8 - maxWidth - 14, y + 1), Assets.Get<Texture2D>(texture), Color.White);
                batch.Text(poco, 10, q.Name, new Vector2(screenSize.X - 8 - maxWidth, y - 8), gold);
                batch.Line(new Vector2(screenSize.X - 8 - maxWidth, y - 8 + measure.Y + 2),
                    new Vector2(screenSize.X - 8, y - 8 + measure.Y + 2),
                    gold);

                y += (int)measure.Y + 4;
                
                measure = poco.Measure(10, q.Objective, 0.8f);
                batch.Text(poco, 10, q.Objective, new Vector2(screenSize.X - 8 - maxWidth, y - 8), gold, 0.8f);
                y += (int)measure.Y + 8;
            }

            //batch.Rectangle(transitionRectangle, Color.Black);
            if(target == null)
                batch.Rectangle(new Rectangle(0, 0, 480, 270), Color.Black * _transition);

            if(DrawHook != null)
                DrawHook(this, new DrawEventArgs { Batch = batch, ScreenSize = screenSize });

            if(target != null)
                batch.RenderTarget = null;
            else
                Program.BlackBorders(batch);

            if(_debug && target == null)
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
                    rectangle.Height += (int)Game.DefaultFonts.MonoBold.Measure(12, DebugTileString(Player.NextPosition)).Y;
                    rectangle.Height += 8;
                }

                rectangle.Height += (int)Game.DefaultFonts.MonoBold.Measure(12, DebugTileString(Player.NextPosition + Player.Heading.ToPoint())).Y;

                rectangle.Width += 8;
                rectangle.Height += 8;

                batch.Rectangle(rectangle, Color.Black * 0.7f);

                y = 8;

                if(Player.NextPosition == Player.Position)
                {
                    batch.Text(SpriteBatch.FontStyle.MonoBold, 12, DebugTileString(Player.Position), new Vector2(8, y), Color.Green);
                    y += (int)Game.DefaultFonts.MonoBold.Measure(12, DebugTileString(Player.Position)).Y + 8;
                }
                else
                {
                    batch.Text(SpriteBatch.FontStyle.MonoBold, 12, DebugTileString(Player.Position), new Vector2(8, y), Color.Red);
                    y += (int)Game.DefaultFonts.MonoBold.Measure(12, DebugTileString(Player.Position)).Y + 8;
                    batch.Text(SpriteBatch.FontStyle.MonoBold, 12, DebugTileString(Player.NextPosition), new Vector2(8, y), Color.Green);
                    y += (int)Game.DefaultFonts.MonoBold.Measure(12, DebugTileString(Player.NextPosition)).Y + 8;
                }

                batch.Text(SpriteBatch.FontStyle.MonoBold, 12, DebugTileString(Player.NextPosition + Player.Heading.ToPoint()), new Vector2(8, y), Color.Blue);

                batch.Text(SpriteBatch.FontStyle.MonoBold, 16, "Atlases: " + Map.Atlas.Count + " (" + Map.Atlas.TileCount + ") " + Map.Atlas.TotalGenerationTime + " ms", new Vector2(rectangle.X + 8, rectangle.Bottom + 10), Color.White);

                int x = rectangle.X + 8;

                foreach(Texture2D texture in Map.Atlas.Textures)
                {
                    batch.Outline(new Rectangle(x, rectangle.Bottom + 40, 512, 512), Color.White, 2, false);
                    batch.Texture(new Rectangle(x, rectangle.Bottom + 40, 512, 512), texture, Color.White);

                    x += 512 + 8;
                }
            }
        }

        public override void Draw(SpriteBatch batch, Vector2 screenSize)
        {
            if(_transitionType == TransitionType.Normal)
                DrawToScreenOrRenderTarget(batch, screenSize, null);
            else
            {
                screenSize = Program.ScaleBatch(batch);

                batch.Rectangle(new Rectangle(0, 0, 480, 270), Color.Black);

                Rectangle rectangle = new Rectangle(0, 0, 480, 270);
                float extraScale = _transition * 0.2f;
                if(_transitionType == TransitionType.CombatOut)
                    extraScale *= -1;

                rectangle.Width += (int)(480 * extraScale);
                rectangle.Height += (int)(270 * extraScale);

                rectangle.X = 480 / 2 - rectangle.Width / 2;
                rectangle.Y = 270 / 2 - rectangle.Height / 2;

                batch.Texture(rectangle, _transitionRenderTarget, Color.White * (1.0f - _transition));

                Program.BlackBorders(batch);
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
        }

        private IEnumerator<ICoroutineOperation> WaitForTransition(GameState state)
        {
            yield return Wait.Seconds(Game, 0.8);
            Game.CurrentState = state;
        }

        private void LoadMapAndTransition(string newMapFile, bool transition, SpawnArgs? spawnArgs = null)
        {
            Map newMap = Map.Load(AppDomain.CurrentDomain.BaseDirectory + newMapFile);

            WorldState newState =
                spawnArgs.HasValue ? new WorldState(newMap, spawnArgs.Value, transition) :
                new WorldState(newMap, new SpawnArgs(new Point(-1, -1), Direction.Down, Player), transition);

            if(transition)
            {
                _transition.TweenTo(1, TweenEaseType.Linear, 0.4);
                Coroutine.AddExisting(WaitForTransition(newState));
            }
            else
                Game.CurrentState = newState;
        }

        /// <summary>
        /// Transitions to a new map with this existing player.
        /// </summary>
        /// <param name="newMap">The new map to start in.</param>
        /// <param name="transition">If a transition should be used when going between the maps.</param>
        /// <param name="spawnArgs">Used for determining where the player will spawn.</param>
        public void TransitionToMap(string newMap, bool transition, SpawnArgs? spawnArgs = null)
        {
            Paused = true;

            if(Map.IsPreloaded(AppDomain.CurrentDomain.BaseDirectory + newMap))
                LoadMapAndTransition(newMap, transition, spawnArgs);
            else
                Task.Run(() => LoadMapAndTransition(newMap, transition, spawnArgs));
        }

        private IEnumerator<ICoroutineOperation> RunCombatCoroutine(object enemy)
        {
            DrawToScreenOrRenderTarget(Game.Batch, new Vector2(480, 270), _transitionRenderTarget);
            _transitionType = TransitionType.CombatIn;
            _transition.TweenTo(1, TweenEaseType.EaseInCubic, 0.5);

            yield return Wait.Seconds(Game, 0.8);

            CombatState state = new CombatState(Player, enemy as LivingEntity);
            state.Game = Game;

            // do it manually so we don't call OnLeave in the other state
            typeof(Game).GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Game, state);
            state.OnEnter(this);

            while(Game.CurrentState != this)
                yield return null;

            DrawToScreenOrRenderTarget(Game.Batch, new Vector2(480, 270), _transitionRenderTarget);
            _transitionType = TransitionType.CombatOut;
            _transition.TweenTo(0, TweenEaseType.EaseOutCubic, 0.5);

            yield return Wait.Seconds(Game, 0.6);

            // combat complete at this point
            Paused = false;
            _transitionType = TransitionType.Normal;

            if((enemy as LivingEntity).Health <= 0)
                Player.XP += (enemy as LivingEntity).XP;
        }

        /// <summary>
        /// Starts combat against the player with the specified enemy.
        /// </summary>
        /// <param name="enemy">The enemy the player should fight.</param>
        public void StartCombat(LivingEntity enemy)
        {
            Paused = true;
            Coroutine.Start(RunCombatCoroutine, enemy);
        }
    }
}
