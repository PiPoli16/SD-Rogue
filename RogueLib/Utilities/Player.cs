using RogueLib.Dungeon;
using RogueLib.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class Player : IActor, IDrawable
{
    public string Name { get; set; }
    public Vector2 Pos;

    public char Glyph => '@';
    public ConsoleColor _color = ConsoleColor.White;

    // ---------------- STATS ----------------
    protected int _level = 0;
    protected int _hp = 12;
    protected int _maxHp = 12;
    protected int _str = 16;
    protected int _arm = 4;
    protected int _gold = 0;
    protected int _turn = 0;

    public int Strength => _str;
    public int Armor => _arm;
    public int HP => _hp;
    public bool IsDead => _hp <= 0;

    public int Turn => _turn;

    // ---------------- INVENTORY ----------------
    public List<Item> Inventory { get; } = new();

    // ---------------- HUD + LOG SYSTEM ----------------
    private string _hudMessage = "";
    private int _hudTimer = 0;

    private readonly List<string> _log = new();

    public string Message => _hudMessage;
    public IReadOnlyList<string> Log => _log;

    public Player()
    {
        Name = "Rogue";
        Pos = Vector2.Zero;
    }

    // ---------------- UPDATE ----------------
    public virtual void Update()
    {
        _turn++;

        if (_hudTimer > 0)
        {
            _hudTimer--;

            if (_hudTimer == 0)
                _hudMessage = "";
        }
    }

    // ---------------- DRAW ----------------
    public virtual void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, _color);
    }

    // ---------------- HUD ----------------
    public string HUD =>
        $"HP:{_hp}/{_maxHp} STR:{_str} ARM:{_arm} GOLD:{_gold} TURN:{_turn}";

    // ---------------- ITEMS ----------------
    public void AddItem(Item item)
    {
        Inventory.Add(item);
        item.Apply(this);
    }

    // ---------------- STATS ----------------
    public void AddStrength(int value) => _str += value;
    public void AddArmor(int value) => _arm += value;

    public void Heal(int amount)
    {
        _hp = Math.Min(_maxHp, _hp + amount);
    }

    public void AddGold(int amount)
    {
        _gold += amount;
    }

    // ---------------- MESSAGE SYSTEM (NEW) ----------------
    public void SetMessage(string msg)
    {
        // HUD (instant)
        _hudMessage = msg;
        _hudTimer = 40;

        // LOG (history)
        _log.Add(msg);

        // cap log size
        if (_log.Count > 100)
            _log.RemoveAt(0);
    }

    // ---------------- DAMAGE ----------------
    public void TakeDamage(int dmg, string source = "Enemy")
{
    int reduced = Math.Max(0, dmg - _arm);
    _hp -= reduced;

    SetMessage($"-{reduced} HP ({source})");

    if (_hp <= 0)
    {
        _hp = 0;
        SetMessage("💀 You died!");
    }
}
}