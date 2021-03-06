using System;

namespace Server.Items
{
  public class AgilityPotion : BaseAgilityPotion
  {
    [Constructible]
    public AgilityPotion() : base(PotionEffect.Agility)
    {
    }

    public AgilityPotion(Serial serial) : base(serial)
    {
    }

    public override int DexOffset => 10;
    public override TimeSpan Duration => TimeSpan.FromMinutes(2.0);

    public override void Serialize(IGenericWriter writer)
    {
      base.Serialize(writer);

      writer.Write(0); // version
    }

    public override void Deserialize(IGenericReader reader)
    {
      base.Deserialize(reader);

      int version = reader.ReadInt();
    }
  }
}