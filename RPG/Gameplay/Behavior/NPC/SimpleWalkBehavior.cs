using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.RPG.Gameplay.World;

namespace TeamStor.RPG.Gameplay.Behavior
{
    /// <summary>
    /// Simple NPC walk behavior that walks around randomly inside a rectangle.
    /// </summary>
    public class SimpleWalkBehavior : INPCBehavior
    {
        private Dictionary<NPC, double> _lastSecond = new Dictionary<NPC, double>();
        private Random _random = new Random();

        /// <summary>
        /// Radius that the NPC can walk around in.
        /// -1 = infinite
        /// </summary>
        public int Radius
        {
            get; private set;
        }

        /// <summary>
        /// Radius in which the NPC will look at the player instead of walking.
        /// -1 = never look at player
        /// </summary>
        public int LookAtPlayerRadius
        {
            get; private set;
        }

        /// <summary>
        /// Chance the NPC will move on a given second.
        /// </summary>
        public float WalkChance
        {
            get; private set;
        }

        public SimpleWalkBehavior(int radius, int lookAtPlayerRadius, float walkChance)
        {
            Radius = radius;
            LookAtPlayerRadius = lookAtPlayerRadius;
            WalkChance = walkChance;
        }

        public virtual void OnSpawned(NPC npc)
        {
        }

        public virtual void OnDespawned(NPC npc, bool isExitingWorld)
        {
        }

        public virtual void OnTick(NPC npc, double deltaTime, double totalTime, long count)
        {
            double lastSecond = totalTime;
            _lastSecond.TryGetValue(npc, out lastSecond);

            if(!npc.IsWalking &&
                LookAtPlayerRadius != -1 &&
                Vector2.Distance(npc.Position.ToVector2(), npc.World.Player.Position.ToVector2()) <= LookAtPlayerRadius)
            {
                if(npc.World.Player.Position.X < npc.Position.X)
                    npc.Heading = Direction.Left;
                if(npc.World.Player.Position.X > npc.Position.X)
                    npc.Heading = Direction.Right;

                if(npc.World.Player.Position.Y < npc.Position.Y)
                    npc.Heading = Direction.Up;
                if(npc.World.Player.Position.Y > npc.Position.Y)
                    npc.Heading = Direction.Down;
            }
            else if(Math.Floor(lastSecond) != Math.Floor(totalTime) && !npc.IsWalking)
            {
                if(_random.NextDouble() <= WalkChance)
                {
                    Point position = npc.Position;
                    int tries = 0;

                    while(npc.World.IsPointBlocked(position) ||
                        (Radius != -1 && Vector2.Distance(npc.OriginTile.ToVector2(), position.ToVector2()) > Radius))
                    {
                        bool walkVertical = _random.Next() % 2 == 1;
                        int walkDownOrRight = _random.Next() % 2 == 1 ? 1 : -1;

                        position = npc.Position;
                        if(walkVertical)
                            position.Y += walkDownOrRight;
                        else
                            position.X += walkDownOrRight;

                        tries++;
                        if(tries > 50)
                            break;
                    }

                    if(tries <= 50)
                        npc.MoveTo(position);
                }
            }

            if(!_lastSecond.ContainsKey(npc))
                _lastSecond.Add(npc, totalTime);

            _lastSecond[npc] = totalTime;
        }


        public virtual void OnInteract(NPC npc, Player player, bool isFacingPlayer)
        {
        }
    }
}
