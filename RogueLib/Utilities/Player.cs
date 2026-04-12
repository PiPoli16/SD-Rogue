using RogueLib.Dungeon;
using RogueLib.items;
using RogueLib.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

public class Player : IActor, IDrawable
{
    public string Name { get; set; }
    public Vector2 Pos;
    public char Glyph => '@';
    public ConsoleColor _color = ConsoleColor.White;

    protected int _hp = 20;
    protected int _maxHp = 20;
    protected int _str = 10;

    // 🔥 SHIELD = consumable defense pool
    protected int _shield = 0;

    protected int _gold = 0;

    public int Strength => _str;
    public int HP => _hp;
    public int MaxHP { get => _maxHp; set => _maxHp = value; }

    // shows remaining shield
    public int Shield => _shield;

    public bool IsDead => _hp <= 0;

    public List<Item> Inventory { get; } = new();

    private Queue<string> _logs = new();
    private const int MAX_LOGS = 12;
    public IEnumerable<string> Logs => _logs;

    public Player()
    {
        Name = "Rogue";
        Pos = Vector2.Zero;
    }

    public virtual void Update() { }

    public virtual void Draw(IRenderWindow disp)
        => disp.Draw(Glyph, Pos, _color);

    // ---------------- LOGS ----------------
    public void AddLog(string message)
    {
        if (_logs.Count >= MAX_LOGS)
            _logs.Dequeue();

        _logs.Enqueue(message);
    }

    // ---------------- HUD ----------------
    public string HUD =>
        $"HP:{_hp}/{_maxHp} DEF:{_shield} STR:{_str} GOLD:{_gold}";

    // ---------------- STATS ----------------
    public void AddStrength(int value)
    {
        _str += value;
        AddLog($"+{value} STR gained");
    }

    // 🔥 ARMOR = adds consumable shield
    public void AddArmor(int value)
    {
        _shield += value;
        AddLog($"+{value} DEF (shield)");
    }

    public void Heal(int amount)
    {
        _hp = Math.Min(_maxHp, _hp + amount);
        AddLog($"+{amount} HP healed");
    }

    public void RestoreMaxHP()
    {
        _hp = _maxHp;
        AddLog("🧪 Full heal!");
    }

    public void AddGold(int amount)
    {
        _gold += amount;
        AddLog($"+{amount} gold");
    }

    public void AddItem(Item item)
    {
        Inventory.Add(item);
        item.Apply(this);
    }

    // ---------------- DAMAGE SYSTEM ----------------
    public void TakeDamage(int dmg, string source = "Enemy")
    {
        int remaining = dmg;

        // 🔥 SHIELD is CONSUMED when used
        if (_shield > 0)
        {
            int absorbed = Math.Min(_shield, remaining);

            _shield -= absorbed;   
            remaining -= absorbed;

            AddLog($"Shield blocked {absorbed} dmg");
        }

        // HP takes leftover damage
        if (remaining > 0)
        {
            _hp -= remaining;
            AddLog($"-{remaining} HP from {source}");
        }

        if (_hp <= 0)
            _hp = 0;
    }

    // ---------------- AUTO POTION ----------------
    public void TryAutoPotion()
    {
        if (_hp <= _maxHp / 2)
        {
            var potion = Inventory.Find(i => i is Potion);
            if (potion != null)
            {
                potion.Apply(this);
                Inventory.Remove(potion);
                AddLog("Auto-used potion");
            }
        }
    }
}