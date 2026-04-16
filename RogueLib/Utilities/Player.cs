using RogueLib.Dungeon;
using RogueLib.items;
using RogueLib.Utilities;
using System;
using System.Collections.Generic;

public class Player : IActor, IDrawable
{
    public string Name { get; set; }
    public Vector2 Pos;
    public char Glyph => '@';
    public ConsoleColor _color = ConsoleColor.White;

    // ================= BASE STATS =================
    protected int _hp = 20;
    protected int _maxHp = 20;

    // ⚔️ MAIN STAT (USED FOR DAMAGE)
    protected int _str = 5;

    // ================= DEFENSE SYSTEM =================
    protected int _shield = 0;

    protected int _gold = 0;

    // ================= INVENTORY =================
    public List<Item> Inventory { get; } = new();

    // ================= READ ONLY =================
    public int Strength => _str;
    public int HP => _hp;
    public int MaxHP { get => _maxHp; set => _maxHp = value; }

    // ✅ DEFENSE STAT (FIX YOU REQUESTED)
    public int Defense => _shield;

    public bool IsDead => _hp <= 0;

    // ================= ATTACK = STRENGTH =================
    public int GetAttack()
    {
        return _str;
    }

    // ================= CORE =================
    public Player()
    {
        Name = "Rogue";
        Pos = Vector2.Zero;
    }

    public virtual void Update() { }

    public void Draw(IRenderWindow disp)
        => disp.Draw(Glyph, Pos, _color);

    // ================= STATS =================

    // kill gains
    public void AddStrength(int value)
    {
        _str += value;
    }

    public void AddArmor(int value)
    {
        _shield += value;
    }

    public void Heal(int amount)
    {
        _hp = Math.Min(_maxHp, _hp + amount);
    }

    public void RestoreMaxHP()
    {
        _hp = _maxHp;
    }

    public void AddGold(int amount)
    {
        _gold += amount;
    }

    public void AddItem(Item item)
    {
        Inventory.Add(item);
        item.Apply(this);
    }

    // ================= DAMAGE SYSTEM =================
    public void TakeDamage(int dmg, string source = "Enemy")
    {
        int remaining = dmg;

        // shield absorbs first
        if (_shield > 0)
        {
            int absorbed = Math.Min(_shield, remaining);
            _shield -= absorbed;
            remaining -= absorbed;
        }

        if (remaining > 0)
            _hp -= remaining;

        if (_hp < 0)
            _hp = 0;
    }

    // ================= AUTO POTION =================
    public bool TryAutoPotion()
    {
        if (_hp <= _maxHp / 2)
        {
            var potion = Inventory
                .OfType<Potion>()
                .OrderBy(p => p.Type == PotionType.Healing ? 0 : 1)
                .FirstOrDefault();

            if (potion != null)
            {
                potion.Apply(this);
                Inventory.Remove(potion);
                return true;
            }
        }
        return false;
    }

    // ================= HUD =================
    public string HUD =>
        $"HP:{_hp}/{_maxHp} STR:{_str} ATK:{GetAttack()} DEF:{_shield} GOLD:{_gold}";
}