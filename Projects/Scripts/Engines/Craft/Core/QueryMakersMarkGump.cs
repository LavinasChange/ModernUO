using System;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Engines.Craft
{
  public class QueryMakersMarkGump : Gump
  {
    private CraftItem m_CraftItem;
    private CraftSystem m_CraftSystem;
    private Mobile m_From;
    private int m_Quality;
    private BaseTool m_Tool;
    private Type m_TypeRes;

    public QueryMakersMarkGump(int quality, Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes,
      BaseTool tool) : base(100, 200)
    {
      from.CloseGump<QueryMakersMarkGump>();

      m_Quality = quality;
      m_From = from;
      m_CraftItem = craftItem;
      m_CraftSystem = craftSystem;
      m_TypeRes = typeRes;
      m_Tool = tool;

      AddPage(0);

      AddBackground(0, 0, 220, 170, 5054);
      AddBackground(10, 10, 200, 150, 3000);

      AddHtmlLocalized(20, 20, 180, 80, 1018317); // Do you wish to place your maker's mark on this item?

      AddHtmlLocalized(55, 100, 140, 25, 1011011); // CONTINUE
      AddButton(20, 100, 4005, 4007, 1);

      AddHtmlLocalized(55, 125, 140, 25, 1011012); // CANCEL
      AddButton(20, 125, 4005, 4007, 0);
    }

    public override void OnResponse(NetState sender, RelayInfo info)
    {
      bool makersMark = info.ButtonID == 1;

      if (makersMark)
        m_From.SendLocalizedMessage(501808); // You mark the item.
      else
        m_From.SendLocalizedMessage(501809); // Cancelled mark.

      m_CraftItem.CompleteCraft(m_Quality, makersMark, m_From, m_CraftSystem, m_TypeRes, m_Tool, null);
    }
  }
}
