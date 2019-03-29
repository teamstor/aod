using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.AOD.Gameplay.Quests;
using TeamStor.Engine.Graphics;
using TeamStor.AOD.Gameplay.World;
using TeamStor.AOD.Gameplay.World.UI;
using static TeamStor.Engine.Graphics.SpriteBatch;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD.Gameplay
{
    /// <summary>
    /// Player class for WorldState and CombatState.
    /// </summary>
    public class Player : LivingEntity
    {
        private double _landWhen = 0;
        private double _keyHeld;
        private Keys _keyQueued = Keys.None;
        
        private SortedDictionary<string, Quest> _activeQuests = new SortedDictionary<string, Quest>();

        /// <summary>
        /// The maximum number of quests the player can have active at one time.
        /// </summary>
        public const int MAX_QUESTS = 3;
        
        /// <summary>
        /// All active quests.
        /// </summary>
        public ICollection<Quest> Quests
        {
            get { return _activeQuests.Values; }
        }

        public Player(WorldState world, Player player) : this(world, player.Name)
        {
            _activeQuests = player._activeQuests;
        }
        
        public Player(WorldState world, string name) : base(world, name)
        {
            AddQuest(new TestQuest1());
            AddQuest(new TestQuest2());
            
            Speed = 3.5;
            MoveInstantly(new Point(0, 0));

            for(int x = 0; x < world.Map.Width; x++)
            {
                for(int y = 0; y < world.Map.Height; y++)
                {
                    if(world.Map[Tile.MapLayer.Control, x, y] == ControlTiles.Spawnpoint)
                        MoveInstantly(new Point(x, y));
                }
            }

            NextPosition = Position;
        }

        public void Update(double deltaTime, double totalTime, long count)
        {
            foreach(Quest quest in Quests)
                quest.OnQuestTick(this);
            
            if(!IsWalking)
            {
                if(InputMap.FindMapping(InputAction.Inventory).Pressed(World.Input))
                {
                    InventoryUI.Show(World, this);
                    return;
                }

                if(InputMap.FindMapping(InputAction.Spells).Pressed(World.Input))
                {
                    SpellsUI.Show(World);
                    return;
                }

                if(InputMap.FindMapping(InputAction.Player).Pressed(World.Input))
                {
                    PlayerUI.Show(World);
                    return;
                }

                if(InputMap.FindMapping(InputAction.Menu).Pressed(World.Input))
                {
                    PauseMenuUI.Show(World);
                    return;
                }


                Direction _lastHeading = Heading;

                if(InputMap.FindMapping(InputAction.Up).Held(World.Input))
                {
                    if(Heading != Direction.Up)
                        Heading = Direction.Up;
                    else if(_keyHeld == 0)
                        _keyHeld = 0.1;

                    _keyHeld += deltaTime;
                }
                else if(InputMap.FindMapping(InputAction.Down).Held(World.Input))
                {
                    if(Heading != Direction.Down)
                        Heading = Direction.Down;
                    else if(_keyHeld == 0)
                        _keyHeld = 0.1;

                    _keyHeld += deltaTime;
                }
                else if(InputMap.FindMapping(InputAction.Left).Held(World.Input))
                {
                    if(Heading != Direction.Left)
                        Heading = Direction.Left;
                    else if(_keyHeld == 0)
                        _keyHeld = 0.1;

                    _keyHeld += deltaTime;
                }
                else if(InputMap.FindMapping(InputAction.Right).Held(World.Input))
                {
                    if(Heading != Direction.Right)
                        Heading = Direction.Right;
                    else if(_keyHeld == 0)
                        _keyHeld = 0.1;

                    _keyHeld += deltaTime;
                }
                else
                    _keyHeld -= deltaTime * 2;

                _keyHeld = MathHelper.Clamp((float)_keyHeld, 0.0f, 0.1f);

                if(_keyQueued != Keys.None)
                {
                    if(_keyQueued == Keys.Up)
                        Heading = Direction.Up;
                    if(_keyQueued == Keys.Down)
                        Heading = Direction.Down;
                    if(_keyQueued == Keys.Left)
                        Heading = Direction.Left;
                    if(_keyQueued == Keys.Right)
                        Heading = Direction.Right;

                    _keyHeld = 0.1f;
                    _keyQueued = Keys.None;
                }

                if(_lastHeading != Heading && _keyHeld < 0.1)
                    _landWhen = World.Game.Time + 0.1;

                if(_keyHeld >= 0.1)
                {
                    if(!World.IsPointBlocked(Position + Heading.ToPoint()))
                        MoveTo(Position + Heading.ToPoint());
                }

                if(InputMap.FindMapping(InputAction.Action).Pressed(World.Input))
                {
                    Point interactionPoint = Position + Heading.ToPoint();
                    interactionPoint.X = (int)MathHelper.Clamp(interactionPoint.X, 0, World.Map.Width - 1);
                    interactionPoint.Y = (int)MathHelper.Clamp(interactionPoint.Y, 0, World.Map.Height - 1);

                    foreach(Tile.MapLayer layer in Tile.CachedAllMapLayers)
                    {
                        TileEventBase events = World.Map[layer, interactionPoint.X, interactionPoint.Y].Events;
                        events?.OnInteract(World.Map.GetMetadata(layer, interactionPoint.X, interactionPoint.Y), World, interactionPoint);
                    }

                    foreach(NPC npc in World.NPCs)
                    {
                        if(npc.Position == interactionPoint)
                        {
                            bool isFacingPlayer =
                                Heading == Direction.Left && npc.Heading == Direction.Right ||
                                Heading == Direction.Right && npc.Heading == Direction.Left ||
                                Heading == Direction.Up && npc.Heading == Direction.Down ||
                                Heading == Direction.Down && npc.Heading == Direction.Up;

                            npc.Template.Behavior.OnInteract(npc, this, isFacingPlayer);
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch batch, bool upperHalf)
        {
            string texture = "front";
            if(Heading == Direction.Left || Heading == Direction.Right)
                texture = "side";
            if(Heading == Direction.Up)
                texture = "back";

            int frame = 0;
            if(IsWalking)
                frame = ((int)World.Game.TotalFixedUpdates / 8) % 4;

            batch.Texture(WorldPosition + new Vector2(0, _landWhen > World.Game.Time ? -1 : 0) - new Vector2(0, upperHalf ? 16 : 0), 
                World.Game.Assets.Get<Texture2D>("player/" + texture + frame + ".png"), 
                Color.White,
                null,
                new Rectangle(0, upperHalf ? 0 : 16, 16, 16),
                0,
                null,
                Heading == Direction.Right ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
        }

        /// <summary>
        /// Adds a new quest to the players quest list.
        /// </summary>
        /// <param name="quest">The quest to add.</param>
        /// <returns>The added quest.</returns>
        public Quest AddQuest(Quest quest)
        {
            string id = quest.Priority + quest.ID;

            if(_activeQuests.ContainsKey(id))
                throw new Exception("A quest with the ID " + id + " is already active.");
            
            if(_activeQuests.Count >= MAX_QUESTS)
                throw new Exception("Max quest count exceeded.");
            
            _activeQuests.Add(id, quest);
            quest.OnQuestAdded(this);

            return quest;
        }
        
        /// <summary>
        /// Finds a quest in the players quest list.
        /// </summary>
        /// <param name="id">The ID of the quest.</param>
        /// <returns>The quest, or null.</returns>
        public Quest FindActiveQuestByID(string id)
        {
            if(_activeQuests.ContainsKey(QuestPriority.Main + id))
                return _activeQuests[QuestPriority.Main + id];
            if(_activeQuests.ContainsKey(QuestPriority.Side + id))
                return _activeQuests[QuestPriority.Side + id];
            if(_activeQuests.ContainsKey(id))
                return _activeQuests[id];

            return null;
        }

        /// <summary>
        /// Finishes an active quest.
        /// </summary>
        /// <param name="id">ID of the quest.</param>
        public void FinishQuest(string id)
        {
            Quest quest = FindActiveQuestByID(id);
            if(quest != null)
            {
                quest.OnQuestFinished(this);
                _activeQuests.Remove(quest.Priority + quest.ID);
            }
        }
    }
}
