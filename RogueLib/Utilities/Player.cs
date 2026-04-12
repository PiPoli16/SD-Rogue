using RogueLib.Dungeon;
using RogueLib.Utilities;
using System;
using System.Collections.Generic;

public abstract class Player : IActor, IDrawable
{
    public string Name { get; set; }
    public Vector2 Pos;
    public char Glyph => '@';
    public ConsoleColor _color = ConsoleColor.White;

    // ---------------- STATS ----------------
    protected int _hp = 12;
    protected int _maxHp = 12;
    protected int _str = 16;
    protected int _arm = 4;
    protected int _gold = 0;
    protected int _turn = 0;

    public int Strength => _str;
    public int Armor => _arm;
    public int HP => _hp;
    public int Turn => _turn;

    // ---------------- INVENTORY ----------------
    public List<Item> Inventory { get; } = new();

    // ---------------- HUD MESSAGE ONLY ----------------
    private string _hudMessage = "";

    public string Message => _hudMessage;

    public Player()
    {
        Name = "Rogue";
        Pos = Vector2.Zero;
    }

    // ---------------- UPDATE ----------------
    public virtual void Update()
    {
        _turn++;
    }

    // ---------------- DRAW ----------------
    public virtual void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, _color);
    }

    // ---------------- HUD ----------------
    public string HUD => $"HP:{_hp}/{_maxHp} STR:{_str} ARM:{_arm} GOLD:{_gold} TURN:{_turn}";

    // ---------------- STATS ----------------
    public void AddStrength(int value) => _str += value;
    public void AddArmor(int value) => _arm += value;
    public void Heal(int amount) => _hp = Math.Min(_maxHp, _hp + amount);
    public void AddGold(int amount) => _gold += amount;

    // ---------------- MESSAGE SYSTEM (ONLY ONE) ----------------
    public void SetMessage(string msg)
    {
        _hudMessage = msg;
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