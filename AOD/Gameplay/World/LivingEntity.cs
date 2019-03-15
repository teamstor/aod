using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.AOD.Gameplay.World
{
    /// <summary>
    /// Living entity with attributes that can die.
    /// </summary>
    public class LivingEntity : PositionedEntity
    {
        private int _healthValue;

        /// <summary>
        /// Name of this entity.
        /// </summary>
        public string Name
        {
            get; protected set;
        }

        /// <summary>
        /// Amount of health points the entity has.
        /// Is limited to the range 0-MaxHealth
        /// </summary>
        public int Health
        {
            get
            {
                return _healthValue;
            }
            set
            {
                _healthValue = value;
                if(_healthValue < 0)
                    _healthValue = 0;
                if(_healthValue > MaxHealth)
                    _healthValue = MaxHealth;
            }
        }

        /// <summary>
        /// Maximum amount of health this entity can have.
        /// Vitality * 10
        /// </summary>
        public int MaxHealth
        {
            get
            {
                return Vitality * 10;
            }
        }

        /// <summary>
        /// If this entity is dead (HP = 0).
        /// </summary>
        public bool IsDead
        {
            get
            {
                return Health == 0;
            }
        }

        private int _magickaValue;

        /// <summary>
        /// Magic points the entity has.
        /// Is limited to the range 0-MaxMagicka
        /// </summary>
        public int Magicka
        {
            get
            {
                return _magickaValue;
            }
            set
            {
                _magickaValue = value;
                if(_magickaValue < 0)
                    _magickaValue = 0;
                if(_magickaValue > MaxMagicka)
                    _magickaValue = MaxMagicka;
            }
        }

        /// <summary>
        /// Maximum amount of magicka this enitity can have.
        /// Spirit * 10
        /// </summary>
        public int MaxMagicka
        {
            get
            {
                return Spirit * 10;
            }
        }

        private int _levelValue;

        /// <summary>
        /// Level of this entity. This is only visual for enemies but allows players to raise attributes.
        /// 1-50
        /// </summary>
        public int Level
        {
            get
            {
                return _levelValue;
            }
            set
            {
                _levelValue = value;
                if(_levelValue < 1)
                    _levelValue = 1;
                if(_levelValue > 50)
                    _levelValue = 50;
            }
        }

        private int _xpValue;

        /// <summary>
        /// Amount of player experience. Used for leveling.
        /// </summary>
        public int XP
        {
            get
            {
                return _xpValue;
            }
            set
            {
                _xpValue = value;
                while(_xpValue >= NeededXP)
                {
                    _xpValue -= NeededXP;
                    Level++;

                    if(OnLevelUp != null)
                        OnLevelUp(this, Level, _xpValue);
                }
            }
        }

        /// <summary>
        /// Amount of XP needed to advance to the next level.
        /// </summary>
        public int NeededXP
        {
            get
            {
                // TODO: ändra ekvationen
                return (Level * 100) + (Level * 5);
            }
        }

        /// <param name="entity">The entity (wow)</param>
        /// <param name="newLevel">The new level of the player</param>
        /// <param name="xpLeft">The amount of XP left over after leveling</param>
        public delegate void LevelUpDelegate(LivingEntity entity, int newLevel, int xpLeft);

        /// <summary>
        /// Event called when the entity levels up.
        /// </summary>
        public LevelUpDelegate OnLevelUp;

        /// <summary>
        /// Affects the amount of damage this entity gives with melee weapons.
        /// </summary>
        public int Strength;

        /// <summary>
        /// Affects the amount of damage this entity gives with ranged weapons.
        /// </summary>
        public int Agility;

        /// <summary>
        /// Affects the amount of damage this entity gives with magic spells.
        /// </summary>
        public int Intellect;

        /// <summary>
        /// Affects the maximum amount of health this entity has.
        /// </summary>
        public int Vitality;

        /// <summary>
        /// Affects the maximum amount of magicka this entity has.
        /// </summary>
        public int Spirit;

        /// <summary>
        /// Base melee attack damage based on what weapon is equipped.
        /// </summary>
        public Tuple<int, int> MeleeAttackDamageRange
        {
            get
            {
                int min = !Inventory[InventoryEquipSlot.Weapon].IsEmptyReference && Inventory[InventoryEquipSlot.Weapon].Item is WeaponItem ?
                    (Inventory[InventoryEquipSlot.Weapon].Item as WeaponItem).DamageRange.Item1 : 1;
                int max = !Inventory[InventoryEquipSlot.Weapon].IsEmptyReference && Inventory[InventoryEquipSlot.Weapon].Item is WeaponItem ? 
                    (Inventory[InventoryEquipSlot.Weapon].Item as WeaponItem).DamageRange.Item2 : 1;

                return new Tuple<int, int>(min, max);
            }
        }

        /// <summary>
        /// Affects how much of a chance there is an enemy will hit your armor rather than you with a physical attack.
        /// 1-100, 100 is a guaranteed block.
        /// A random physical armor item should lose durability when hit.
        /// </summary>
        public int PhysicalArmor
        {
            get
            {
                int physicalArmor = 0;

                if(!Inventory[InventoryEquipSlot.Head].IsEmptyReference &&
                    (Inventory[InventoryEquipSlot.Head].Item is ArmorItem) &&
                    (Inventory[InventoryEquipSlot.Head].Item as ArmorItem).Protection == ArmorItem.ProtectionType.Physical)
                    physicalArmor += (Inventory[InventoryEquipSlot.Head].Item as ArmorItem).ArmorValue;

                if(!Inventory[InventoryEquipSlot.Chest].IsEmptyReference &&
                    (Inventory[InventoryEquipSlot.Chest].Item is ArmorItem) &&
                    (Inventory[InventoryEquipSlot.Chest].Item as ArmorItem).Protection == ArmorItem.ProtectionType.Physical)
                    physicalArmor += (Inventory[InventoryEquipSlot.Chest].Item as ArmorItem).ArmorValue;

                if(!Inventory[InventoryEquipSlot.Leggings].IsEmptyReference &&
                    (Inventory[InventoryEquipSlot.Leggings].Item is ArmorItem) &&
                    (Inventory[InventoryEquipSlot.Leggings].Item as ArmorItem).Protection == ArmorItem.ProtectionType.Physical)
                    physicalArmor += (Inventory[InventoryEquipSlot.Leggings].Item as ArmorItem).ArmorValue;

                if(!Inventory[InventoryEquipSlot.Boots].IsEmptyReference &&
                    (Inventory[InventoryEquipSlot.Boots].Item is ArmorItem) &&
                    (Inventory[InventoryEquipSlot.Boots].Item as ArmorItem).Protection == ArmorItem.ProtectionType.Physical)
                    physicalArmor += (Inventory[InventoryEquipSlot.Boots].Item as ArmorItem).ArmorValue;

                if(!Inventory[InventoryEquipSlot.Ring].IsEmptyReference &&
                    (Inventory[InventoryEquipSlot.Ring].Item is ArmorItem) &&
                    (Inventory[InventoryEquipSlot.Ring].Item as ArmorItem).Protection == ArmorItem.ProtectionType.Physical)
                    physicalArmor += (Inventory[InventoryEquipSlot.Ring].Item as ArmorItem).ArmorValue;

                return physicalArmor;
            }
        }

        /// <summary>
        /// Affects how much of a chance there is an enemy will hit your armor rather than you with a spell.
        /// 1-100, 100 is a guaranteed block.
        /// A random magic armor item should lose durability when hit.
        /// </summary>
        public int MagicArmor
        {
            get
            {
                int magicalArmor = 0;

                if(!Inventory[InventoryEquipSlot.Head].IsEmptyReference &&
                    (Inventory[InventoryEquipSlot.Head].Item is ArmorItem) &&
                    (Inventory[InventoryEquipSlot.Head].Item as ArmorItem).Protection == ArmorItem.ProtectionType.Magical)
                    magicalArmor += (Inventory[InventoryEquipSlot.Head].Item as ArmorItem).ArmorValue;

                if(!Inventory[InventoryEquipSlot.Chest].IsEmptyReference &&
                    (Inventory[InventoryEquipSlot.Chest].Item is ArmorItem) &&
                    (Inventory[InventoryEquipSlot.Chest].Item as ArmorItem).Protection == ArmorItem.ProtectionType.Magical)
                    magicalArmor += (Inventory[InventoryEquipSlot.Chest].Item as ArmorItem).ArmorValue;

                if(!Inventory[InventoryEquipSlot.Leggings].IsEmptyReference &&
                    (Inventory[InventoryEquipSlot.Leggings].Item is ArmorItem) &&
                    (Inventory[InventoryEquipSlot.Leggings].Item as ArmorItem).Protection == ArmorItem.ProtectionType.Magical)
                    magicalArmor += (Inventory[InventoryEquipSlot.Leggings].Item as ArmorItem).ArmorValue;

                if(!Inventory[InventoryEquipSlot.Boots].IsEmptyReference &&
                    (Inventory[InventoryEquipSlot.Boots].Item is ArmorItem) &&
                    (Inventory[InventoryEquipSlot.Boots].Item as ArmorItem).Protection == ArmorItem.ProtectionType.Magical)
                    magicalArmor += (Inventory[InventoryEquipSlot.Boots].Item as ArmorItem).ArmorValue;

                if(!Inventory[InventoryEquipSlot.Ring].IsEmptyReference &&
                    (Inventory[InventoryEquipSlot.Ring].Item is ArmorItem) &&
                    (Inventory[InventoryEquipSlot.Ring].Item as ArmorItem).Protection == ArmorItem.ProtectionType.Magical)
                    magicalArmor += (Inventory[InventoryEquipSlot.Ring].Item as ArmorItem).ArmorValue;

                return magicalArmor;
            }
        }

        /// <summary>
        /// Entity inventory.
        /// </summary>
        public Inventory Inventory
        {
            get; private set;
        }

        public LivingEntity(WorldState world, string name) : base(world)
        {
            Inventory = new Inventory(this);
            Name = name;

            Level = 1;

            Vitality = 10;
            Spirit = 10;

            Health = 100;
            Magicka = 100;
        }

        public string StatsInfoString
        {
            get
            {
                string s = Name + " (Level " + Level + ")\n";
                s += "HP: " + Health + "/" + MaxHealth + "\n";
                if(MaxMagicka == 0)
                    s += "MP: (none)\n";
                else
                    s += "MP: " + Magicka + "/" + MaxMagicka + "\n";

                s += "\n";

                s += "Strength: " + Strength + "\n";
                s += "Agility: " + Agility + "\n";
                s += "Intellect: " + Intellect + "\n";
                s += "Vitality: " + Vitality + "\n";
                s += "Spirit: " + Spirit + "\n";

                s += "\n";

                s += "Armor: Physical " + PhysicalArmor + ", Magical " + MagicArmor;

                return s;
            }
        }
    }
}
