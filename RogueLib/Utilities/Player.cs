using RogueLib.Dungeon;
using RogueLib.items;
using RogueLib.Utilities;
using System;
using System.Collections.Generic;

public class Player : IActor, IDrawable
{
    public string Name { get; set; } = "Rogue";
    public Vector2 Pos;
    public char Glyph => '@';
    public ConsoleColor _color = ConsoleColor.White;

    // Stats
    public int HP { get; private set; } = 20;
    public int MaxHP { get; private set; } = 20;
    public int Strength { get; private set; } = 10;
    private int _armorCount = 0; // number of armor items
    private const int ArmorShield = 2; // each armor reduces damage
    private int _gold = 0;

    // Notifications
    private string _msg = "";
    private int _msgTimer = 0;
    public string Message => _msg;

    // Inventory
    public List<Item> Inventory { get; } = new();

    // Update (virtual for override)
    public virtual void Update()
    {
        if (_msgTimer > 0)
        {
            _msgTimer--;
            if (_msgTimer == 0) _msg = "";
        }
    }

    public void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, _color);
    }

    public string HUD => $"HP:{HP}/{MaxHP} STR:{Strength} ARM:{_armorCount} GOLD:{_gold}";

    // Combat
    public void TakeDamage(int dmg, string source)
    {
        int effectiveDamage = dmg - (_armorCount * ArmorShield);
        if (effectiveDamage < 0) effectiveDamage = 0;

        if (effectiveDamage >= HP)
        {
            HP = 0;
            SetMessage("💀 You died!");
        }
        else
        {
            HP -= effectiveDamage;
            SetMessage($"-{effectiveDamage} HP from {source}");
        }
    }

    public void AddArmor()
    {
        _armorCount++;
        HP += 2; // armor increases HP
        SetMessage("🛡 Equipped armor (+2 HP)");
    }

    public void AddStrength(int value)
    {
        Strength += value;
        SetMessage($"⚔ +{value} STR");
    }

    public void Heal(int amount)
    {
        HP = Math.Min(MaxHP, HP + amount);
        SetMessage($"🧪 +{amount} HP");
    }

    public void RestoreMaxHP()
    {
        HP = MaxHP;
        SetMessage("🧪 Max HP restored!");
    }

    public void AddGold(int amt)
    {
        _gold += amt;
        SetMessage($"💰 +{amt} Gold");
    }

    public void SetMessage(string msg)
    {
        _msg = msg;
        _msgTimer = 40;
    }

    public void TryAutoPotion()
    {
        var pot = Inventory.Find(i => i is Potion || i is SpecialPotion);
        if (pot != null && HP <= MaxHP / 2)
        {
            pot.Apply(this);
            Inventory.Remove(pot);
        }
    }
}