using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Engines.Quests.Naturalist
{
  public class StudyNestsObjective : QuestObjective
  {
    private NestArea m_CurrentNest;

    private List<NestArea> m_StudiedNests = new List<NestArea>();
    private DateTime m_StudyBegin;
    private StudyState m_StudyState;

    public override object Message => 1054044;

    public override int MaxProgress => NestArea.NonSpecialCount;

    public bool StudiedSpecialNest{ get; private set; }

    public override bool GetTimerEvent() => true;

    public override void CheckProgress()
    {
      PlayerMobile from = System.From;

      if (m_CurrentNest != null)
      {
        NestArea nest = m_CurrentNest;

        if ((from.Map == Map.Trammel || from.Map == Map.Felucca) && nest.Contains(from))
        {
          if (m_StudyState != StudyState.Inactive)
          {
            TimeSpan time = DateTime.UtcNow - m_StudyBegin;

            if (time > TimeSpan.FromSeconds(30.0))
            {
              m_StudiedNests.Add(nest);
              m_StudyState = StudyState.Inactive;

              if (m_CurrentNest.Special)
              {
                from.SendLocalizedMessage(
                  1054057); // You complete your examination of this bizarre Egg Nest. The Naturalist will undoubtedly be quite interested in these notes!
                StudiedSpecialNest = true;
              }
              else
              {
                from.SendLocalizedMessage(
                  1054054); // You have completed your study of this Solen Egg Nest. You put your notes away.
                CurProgress++;
              }
            }
            else if (m_StudyState == StudyState.FirstStep && time > TimeSpan.FromSeconds(15.0))
            {
              if (!nest.Special)
                from.SendLocalizedMessage(
                  1054058); // You begin recording your completed notes on a bit of parchment.

              m_StudyState = StudyState.SecondStep;
            }
          }
        }
        else
        {
          if (m_StudyState != StudyState.Inactive)
            from.SendLocalizedMessage(
              1054046); // You abandon your study of the Solen Egg Nest without gathering the needed information.

          m_CurrentNest = null;
        }
      }
      else if (from.Map == Map.Trammel || from.Map == Map.Felucca)
      {
        NestArea nest = NestArea.Find(from);

        if (nest != null)
        {
          m_CurrentNest = nest;
          m_StudyBegin = DateTime.UtcNow;

          if (m_StudiedNests.Contains(nest))
          {
            m_StudyState = StudyState.Inactive;

            from.SendLocalizedMessage(
              1054047); // You glance at the Egg Nest, realizing you've already studied this one.
          }
          else
          {
            m_StudyState = StudyState.FirstStep;

            if (nest.Special)
              from.SendLocalizedMessage(
                1054056); // You notice something very odd about this Solen Egg Nest. You begin taking notes.
            else
              from.SendLocalizedMessage(
                1054045); // You begin studying the Solen Egg Nest to gather information.

            if (from.Female)
              from.PlaySound(0x30B);
            else
              from.PlaySound(0x419);
          }
        }
      }
    }

    public override void RenderProgress(BaseQuestGump gump)
    {
      if (!Completed)
      {
        gump.AddHtmlLocalized(70, 260, 270, 100, 1054055, BaseQuestGump.Blue); // Solen Nests Studied :
        gump.AddLabel(70, 280, 0x64, CurProgress.ToString());
        gump.AddLabel(100, 280, 0x64, "/");
        gump.AddLabel(130, 280, 0x64, MaxProgress.ToString());
      }
      else
      {
        base.RenderProgress(gump);
      }
    }

    public override void OnComplete()
    {
      System.AddObjective(new ReturnToNaturalistObjective());
    }

    public override void ChildDeserialize(IGenericReader reader)
    {
      int version = reader.ReadEncodedInt();

      int count = reader.ReadEncodedInt();
      for (int i = 0; i < count; i++)
      {
        NestArea nest = NestArea.GetByID(reader.ReadEncodedInt());
        m_StudiedNests.Add(nest);
      }

      StudiedSpecialNest = reader.ReadBool();
    }

    public override void ChildSerialize(IGenericWriter writer)
    {
      writer.WriteEncodedInt(0); // version

      writer.WriteEncodedInt(m_StudiedNests.Count);
      foreach (NestArea nest in m_StudiedNests)
        writer.WriteEncodedInt(nest.ID);

      writer.Write(StudiedSpecialNest);
    }

    private enum StudyState
    {
      Inactive,
      FirstStep,
      SecondStep
    }
  }

  public class ReturnToNaturalistObjective : QuestObjective
  {
    public override object Message => 1054048;

    public override void RenderProgress(BaseQuestGump gump)
    {
      string count = NestArea.NonSpecialCount.ToString();

      gump.AddHtmlLocalized(70, 260, 270, 100, 1054055, BaseQuestGump.Blue); // Solen Nests Studied :
      gump.AddLabel(70, 280, 0x64, count);
      gump.AddLabel(100, 280, 0x64, "/");
      gump.AddLabel(130, 280, 0x64, count);
    }
  }
}