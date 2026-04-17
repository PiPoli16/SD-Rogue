using RogueLib.Dungeon;
using RogueLib.items;
using RogueLib.Utilities;
using System;
using System.Collections.Generic;
using System.Linq; // needed for OfType and OrderBy

// Player represents the main controllable character in the game.
// It handles stats, combat, inventory, and rendering.
public class Player : IActor, IDrawable
{
    // ================= BASIC INFO =================

    // Player name (can be customized)
    public string Name { get; set; }

    // Current position on the map (X, Y coordinates)
    public Vector2 Pos;

    // Character used to draw the player on screen
    public char Glyph => '@';

    // Player color when rendered
    public ConsoleColor _color = ConsoleColor.White;

    // ================= BASE STATS =================

    // Current health points
    protected int _hp = 20;

    // Maximum health points
    protected int _maxHp = 20;

    // Main attack stat (used for dealing damage)
    protected int _str = 5;

    // ================= DEFENSE SYSTEM =================

    // Shield/armor value (absorbs damage before HP)
    protected int _shield = 0;

    // Player's gold (currency)
    protected int _gold = 0;

    // ================= INVENTORY =================

    // Stores all collected items
    public List<Item> Inventory { get; } = new();

    // ================= READ-ONLY PROPERTIES =================

    // Exposes strength safely (read-only outside the class)
    public int Strength => _str;

    // Exposes current HP
    public int HP => _hp;

    // MaxHP can be read and modified (used for scaling)
    public int MaxHP { get => _maxHp; set => _maxHp = value; }

    // Defense value (shield)
    public int Defense => _shield;

    // Check if player is dead
    public bool IsDead => _hp <= 0;

    // ================= ATTACK =================

    // Returns player's attack damage (currently equal to strength)
    public int GetAttack()
    {
        return _str;
    }

    // ================= CONSTRUCTOR =================

    // Initializes default player values
    public Player()
    {
        Name = "Rogue";
        Pos = Vector2.Zero; // starting position (can be overridden by level)
    }

    // ================= UPDATE =================

    // Called every game loop (currently unused but extendable)
    public virtual void Update() { }

    // ================= DRAW =================

    // Draws the player on screen using the render system
    public void Draw(IRenderWindow disp)
        => disp.Draw(Glyph, Pos, _color);

    // ================= STAT MODIFIERS =================

    // Increase player strength (used by weapons or rewards)
    public void AddStrength(int value)
    {
        _str += value;
    }

    // Increase player armor/shield
    public void AddArmor(int value)
    {
        _shield += value;
    }

    // Heal player without exceeding max HP
    public void Heal(int amount)
    {
        _hp = Math.Min(_maxHp, _hp + amount);
    }

    // Fully restore HP to max
    public void RestoreMaxHP()
    {
        _hp = _maxHp;
    }

    // Add gold to player
    public void AddGold(int amount)
    {
        _gold += amount;
    }

    // Add item to inventory AND apply its effect immediately
    public void AddItem(Item item)
    {
        Inventory.Add(item);   // store item
        item.Apply(this);      // apply effect (polymorphism)
    }

    // ================= DAMAGE SYSTEM =================

    // Handles incoming damage
    public void TakeDamage(int dmg, string source = "Enemy")
    {
        int remaining = dmg;

        // Step 1: Shield absorbs damage first
        if (_shield > 0)
        {
            int absorbed = Math.Min(_shield, remaining);
            _shield -= absorbed;
            remaining -= absorbed;
        }

        // Step 2: Remaining damage reduces HP
        if (remaining > 0)
            _hp -= remaining;

        // Step 3: Prevent negative HP
        if (_hp < 0)
            _hp = 0;
    }

    // ================= AUTO POTION SYSTEM =================

    // Automatically uses a potion when HP is low
    public bool TryAutoPotion()
    {
        // Trigger only when HP is 50% or below
        if (_hp <= _maxHp / 2)
        {
            // Get potions only, prioritize Healing first
            var potion = Inventory
                .OfType<Potion>() // filter only Potion objects
                .OrderBy(p => p.Type == PotionType.Healing ? 0 : 1) // Healing first
                .FirstOrDefault();

            // If a potion is found, use it
            if (potion != null)
            {
                potion.Apply(this);     // apply effect
                Inventory.Remove(potion); // remove from inventory
                return true;            // indicate potion was used
            }
        }

        return false; // no potion used
    }

    // ================= HUD =================

    // Returns a formatted string showing player stats on screen
    public string HUD =>
        $"HP:{_hp}/{_maxHp} STR:{_str} ATK:{GetAttack()} DEF:{_shield} GOLD:{_gold}";
}