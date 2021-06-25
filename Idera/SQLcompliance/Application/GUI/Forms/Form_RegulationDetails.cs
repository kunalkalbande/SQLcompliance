using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLcompliance.Core.Templates.AuditTemplates;
using System.Collections;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_RegulationDetails : Form
    {
        private bool _pci = false;
        private bool _hipaa = false;
        private bool _disa = false;
        private bool _nerc = false;
        private bool _cis = false;
        private bool _sox = false;
        private bool _ferpa = false;
        private bool _gdpr = false;

        public Form_RegulationDetails(bool pci, bool hipaa, bool disa, bool nerc, bool cis, bool sox, bool ferpa, bool gdpr)
        {
            InitializeComponent();
            _pci = pci;
            _hipaa = hipaa;
            _disa = disa;
            _nerc = nerc;
            _cis = cis;
            _sox = sox;
            _ferpa = ferpa;
            _gdpr = gdpr;
            lableCustomMessage.Visible = !(pci || hipaa || disa || nerc || cis || sox || ferpa || gdpr);
            SetRegulationInfo();
            SetFirstPage();
        }

        private void SetFirstPage()
        {
            ultraTabControl1.Tabs["tabPCI"].Visible = _pci;
            ultraTabControl1.Tabs["tabHIPAA"].Visible = _hipaa;
            ultraTabControl1.Tabs["tabDISA"].Visible = _disa;
            ultraTabControl1.Tabs["tabNERC"].Visible = _nerc;
            ultraTabControl1.Tabs["tabCIS"].Visible = _cis;
            ultraTabControl1.Tabs["tabSOX"].Visible = _sox;
            ultraTabControl1.Tabs["tabFERPA"].Visible = _ferpa;
            ultraTabControl1.Tabs["tabGDPR"].Visible = _gdpr;
        }

        private void SetRegulationInfo()
        {
            Dictionary<int, List<RegulationSection>> regSections = RegulationDAL.LoadRegulationSections(Globals.Repository.Connection);
            List<RegulationSection> sections;

            //PCI
            if (regSections.TryGetValue((int)Regulation.RegulationType.PCI, out sections))
            {
                if (sections.Count > 0)
                {
                    regulationEntryPCI1.Section = sections[0].Name;
                    regulationEntryPCI1.ServerEvents = ConvertServerEvents(sections[0].ServerCategories);
                    regulationEntryPCI1.DatabaseEvents = ConvertDatabaseEvents(sections[0].DatabaseCategories);
                }
                if (sections.Count > 1)
                {
                    regulationEntryPCI2.Section = sections[1].Name;
                    regulationEntryPCI2.ServerEvents = ConvertServerEvents(sections[1].ServerCategories);
                    regulationEntryPCI2.DatabaseEvents = ConvertDatabaseEvents(sections[1].DatabaseCategories);
                }
                if (sections.Count > 2)
                {
                    regulationEntryPCI3.Section = sections[2].Name;
                    regulationEntryPCI3.ServerEvents = ConvertServerEvents(sections[2].ServerCategories);
                    regulationEntryPCI3.DatabaseEvents = ConvertDatabaseEvents(sections[2].DatabaseCategories);
                }
                if (sections.Count > 3)
                {
                    regulationEntryPCI4.Section = sections[3].Name;
                    regulationEntryPCI4.ServerEvents = ConvertServerEvents(sections[3].ServerCategories);
                    regulationEntryPCI4.DatabaseEvents = ConvertDatabaseEvents(sections[3].DatabaseCategories);
                }
                if (sections.Count > 4)
                {
                    regulationEntryPCI5.Section = sections[4].Name;
                    regulationEntryPCI5.ServerEvents = ConvertServerEvents(sections[4].ServerCategories);
                    regulationEntryPCI5.DatabaseEvents = ConvertDatabaseEvents(sections[4].DatabaseCategories);
                }
                if (sections.Count > 5)
                {
                    regulationEntryPCI6.Section = sections[5].Name;
                    regulationEntryPCI6.ServerEvents = ConvertServerEvents(sections[5].ServerCategories);
                    regulationEntryPCI6.DatabaseEvents = ConvertDatabaseEvents(sections[5].DatabaseCategories);
                }
            }

            //HIPAA
            if (regSections.TryGetValue((int)Regulation.RegulationType.HIPAA, out sections))
            {
                if (sections.Count > 0)
                {
                    regulationEntryHIPAA1.Section = sections[0].Name;
                    regulationEntryHIPAA1.ServerEvents = ConvertServerEvents(sections[0].ServerCategories);
                    regulationEntryHIPAA1.DatabaseEvents = ConvertDatabaseEvents(sections[0].DatabaseCategories);
                }

                if (sections.Count > 1)
                {
                    regulationEntryHIPAA2.Section = sections[1].Name;
                    regulationEntryHIPAA2.ServerEvents = ConvertServerEvents(sections[1].ServerCategories);
                    regulationEntryHIPAA2.DatabaseEvents = ConvertDatabaseEvents(sections[1].DatabaseCategories);
                }

                if (sections.Count > 2)
                {
                    regulationEntryHIPAA3.Section = sections[2].Name;
                    regulationEntryHIPAA3.ServerEvents = ConvertServerEvents(sections[2].ServerCategories);
                    regulationEntryHIPAA3.DatabaseEvents = ConvertDatabaseEvents(sections[2].DatabaseCategories);
                }

                if (sections.Count > 3)
                {
                    regulationEntryHIPAA4.Section = sections[3].Name;
                    regulationEntryHIPAA4.ServerEvents = ConvertServerEvents(sections[3].ServerCategories);
                    regulationEntryHIPAA4.DatabaseEvents = ConvertDatabaseEvents(sections[3].DatabaseCategories);
                }

                if (sections.Count > 4)
                {
                    regulationEntryHIPAA5.Section = sections[4].Name;
                    regulationEntryHIPAA5.ServerEvents = ConvertServerEvents(sections[4].ServerCategories);
                    regulationEntryHIPAA5.DatabaseEvents = ConvertDatabaseEvents(sections[4].DatabaseCategories);
                }

                if (sections.Count > 5)
                {
                    regulationEntryHIPAA6.Section = sections[5].Name;
                    regulationEntryHIPAA6.ServerEvents = ConvertServerEvents(sections[5].ServerCategories);
                    regulationEntryHIPAA6.DatabaseEvents = ConvertDatabaseEvents(sections[5].DatabaseCategories);
                }

                if (sections.Count > 6)
                {
                    regulationEntryHIPAA7.Section = sections[6].Name;
                    regulationEntryHIPAA7.ServerEvents = ConvertServerEvents(sections[6].ServerCategories);
                    regulationEntryHIPAA7.DatabaseEvents = ConvertDatabaseEvents(sections[6].DatabaseCategories);
                }

                if (sections.Count > 7)
                {
                    regulationEntryHIPAA8.Section = sections[7].Name;
                    regulationEntryHIPAA8.ServerEvents = ConvertServerEvents(sections[7].ServerCategories);
                    regulationEntryHIPAA8.DatabaseEvents = ConvertDatabaseEvents(sections[7].DatabaseCategories);
                }

                if (sections.Count > 8)
                {
                    regulationEntryHIPAA9.Section = sections[8].Name;
                    regulationEntryHIPAA9.ServerEvents = ConvertServerEvents(sections[8].ServerCategories);
                    regulationEntryHIPAA9.DatabaseEvents = ConvertDatabaseEvents(sections[8].DatabaseCategories);
                }

                if (sections.Count > 9)
                {
                    regulationEntryHIPAA10.Section = sections[9].Name;
                    regulationEntryHIPAA10.ServerEvents = ConvertServerEvents(sections[9].ServerCategories);
                    regulationEntryHIPAA10.DatabaseEvents = ConvertDatabaseEvents(sections[9].DatabaseCategories);
                }
            }

            //DISASTIG
            if (regSections.TryGetValue((int)Regulation.RegulationType.DISA, out sections))
            {
                if (sections.Count > 0)
                {
                    regulationEntryDISA1.Section = sections[0].Name;
                    regulationEntryDISA1.ServerEvents = ConvertServerEvents(sections[0].ServerCategories);
                    regulationEntryDISA1.DatabaseEvents = ConvertDatabaseEvents(sections[0].DatabaseCategories);
                }
                if (sections.Count > 1)
                {
                    regulationEntryDISA2.Section = sections[1].Name;
                    regulationEntryDISA2.ServerEvents = ConvertServerEvents(sections[1].ServerCategories);
                    regulationEntryDISA2.DatabaseEvents = ConvertDatabaseEvents(sections[1].DatabaseCategories);
                }
                if (sections.Count > 2)
                {
                    regulationEntryDISA3.Section = sections[2].Name;
                    regulationEntryDISA3.ServerEvents = ConvertServerEvents(sections[2].ServerCategories);
                    regulationEntryDISA3.DatabaseEvents = ConvertDatabaseEvents(sections[2].DatabaseCategories);
                }
                if (sections.Count > 3)
                {
                    regulationEntryDISA4.Section = sections[3].Name;
                    regulationEntryDISA4.ServerEvents = ConvertServerEvents(sections[3].ServerCategories);
                    regulationEntryDISA4.DatabaseEvents = ConvertDatabaseEvents(sections[3].DatabaseCategories);
                }
                if (sections.Count > 4)
                {
                    regulationEntryDISA5.Section = sections[4].Name;
                    regulationEntryDISA5.ServerEvents = ConvertServerEvents(sections[4].ServerCategories);
                    regulationEntryDISA5.DatabaseEvents = ConvertDatabaseEvents(sections[4].DatabaseCategories);
                }
                if (sections.Count > 5)
                {
                    regulationEntryDISA6.Section = sections[5].Name;
                    regulationEntryDISA6.ServerEvents = ConvertServerEvents(sections[5].ServerCategories);
                    regulationEntryDISA6.DatabaseEvents = ConvertDatabaseEvents(sections[5].DatabaseCategories);
                }
            }
            //NERC
            if (regSections.TryGetValue((int)Regulation.RegulationType.NERC, out sections))
            {
                if (sections.Count > 0)
                {
                    regulationEntryNERC1.Section = sections[0].Name;
                    regulationEntryNERC1.ServerEvents = ConvertServerEvents(sections[0].ServerCategories);
                    regulationEntryNERC1.DatabaseEvents = ConvertDatabaseEvents(sections[0].DatabaseCategories);
                }
            }
            //CIS
            if (regSections.TryGetValue((int)Regulation.RegulationType.CIS, out sections))
            {
                if (sections.Count > 0)
                {
                    regulationEntryCIS1.Section = sections[0].Name;
                    regulationEntryCIS1.ServerEvents = ConvertServerEvents(sections[0].ServerCategories);
                    regulationEntryCIS1.DatabaseEvents = ConvertDatabaseEvents(sections[0].DatabaseCategories);
                }
            }
            //SOX
            if (regSections.TryGetValue((int)Regulation.RegulationType.SOX, out sections))
            {
                if (sections.Count > 0)
                {
                    regulationEntrySOX1.Section = sections[0].Name;
                    regulationEntrySOX1.ServerEvents = ConvertServerEvents(sections[0].ServerCategories);
                    regulationEntrySOX1.DatabaseEvents = ConvertDatabaseEvents(sections[0].DatabaseCategories);
                }
                if (sections.Count > 1)
                {
                    regulationEntrySOX2.Section = sections[1].Name;
                    regulationEntrySOX2.ServerEvents = ConvertServerEvents(sections[1].ServerCategories);
                    regulationEntrySOX2.DatabaseEvents = ConvertDatabaseEvents(sections[1].DatabaseCategories);
                }
            }

            //FERPA
            if (regSections.TryGetValue((int)Regulation.RegulationType.FERPA, out sections))
            {
                if (sections.Count > 0)
                {
                    regulationEntryFERPA1.Section = sections[0].Name;
                    regulationEntryFERPA1.ServerEvents = ConvertServerEvents(sections[0].ServerCategories);
                    regulationEntryFERPA1.DatabaseEvents = ConvertDatabaseEvents(sections[0].DatabaseCategories);
                }
                if (sections.Count > 1)
                {
                    regulationEntryFERPA2.Section = sections[1].Name;
                    regulationEntryFERPA2.ServerEvents = ConvertServerEvents(sections[1].ServerCategories);
                    regulationEntryFERPA2.DatabaseEvents = ConvertDatabaseEvents(sections[1].DatabaseCategories);
                }
            }
            //GDPR
            if (regSections.TryGetValue((int)Regulation.RegulationType.GDPR, out sections))
            {
                if (sections.Count > 0)
                {
                    regulationEntryGDPR1.Section = sections[0].Name;
                    regulationEntryGDPR1.ServerEvents = ConvertServerEvents(sections[0].ServerCategories);
                    regulationEntryGDPR1.DatabaseEvents = ConvertDatabaseEvents(sections[0].DatabaseCategories);
                }
                if (sections.Count > 1)
                {
                    regulationEntryGDPR2.Section = sections[1].Name;
                    regulationEntryGDPR2.ServerEvents = ConvertServerEvents(sections[1].ServerCategories);
                    regulationEntryGDPR2.DatabaseEvents = ConvertDatabaseEvents(sections[1].DatabaseCategories);
                }
                if (sections.Count > 2)
                {
                    regulationEntryGDPR3.Section = sections[2].Name;
                    regulationEntryGDPR3.ServerEvents = ConvertServerEvents(sections[2].ServerCategories);
                    regulationEntryGDPR3.DatabaseEvents = ConvertDatabaseEvents(sections[2].DatabaseCategories);
                }
                if (sections.Count > 3)
                {
                    regulationEntryGDPR4.Section = sections[3].Name;
                    regulationEntryGDPR4.ServerEvents = ConvertServerEvents(sections[3].ServerCategories);
                    regulationEntryGDPR4.DatabaseEvents = ConvertDatabaseEvents(sections[3].DatabaseCategories);
                }
                if (sections.Count > 4)
                {
                    regulationEntryGDPR5.Section = sections[4].Name;
                    regulationEntryGDPR5.ServerEvents = ConvertServerEvents(sections[4].ServerCategories);
                    regulationEntryGDPR5.DatabaseEvents = ConvertDatabaseEvents(sections[4].DatabaseCategories);
                }
                if (sections.Count > 5)
                {
                    regulationEntryGDPR6.Section = sections[5].Name;
                    regulationEntryGDPR6.ServerEvents = ConvertServerEvents(sections[5].ServerCategories);
                    regulationEntryGDPR6.DatabaseEvents = ConvertDatabaseEvents(sections[5].DatabaseCategories);
                }
                if (sections.Count > 6)
                {
                    regulationEntryGDPR7.Section = sections[6].Name;
                    regulationEntryGDPR7.ServerEvents = ConvertServerEvents(sections[6].ServerCategories);
                    regulationEntryGDPR7.DatabaseEvents = ConvertDatabaseEvents(sections[6].DatabaseCategories);
                }
                if (sections.Count > 7)
                {
                    regulationEntryGDPR8.Section = sections[7].Name;
                    regulationEntryGDPR8.ServerEvents = ConvertServerEvents(sections[7].ServerCategories);
                    regulationEntryGDPR8.DatabaseEvents = ConvertDatabaseEvents(sections[7].DatabaseCategories);
                }
                if (sections.Count > 8)
                {
                    regulationEntryGDPR9.Section = sections[8].Name;
                    regulationEntryGDPR9.ServerEvents = ConvertServerEvents(sections[8].ServerCategories);
                    regulationEntryGDPR9.DatabaseEvents = ConvertDatabaseEvents(sections[8].DatabaseCategories);
                }

            }
        }

        private string ConvertServerEvents(int serverEvents)
        {
            ArrayList events = new ArrayList();

            if ((serverEvents & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins)
                events.Add("Logins");

            // SQLCM-5375-6.1.4.3-Capture Logout Events
            if ((serverEvents & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts)
                events.Add("Logouts");

            if ((serverEvents & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins)
                events.Add("Failed Logins");

            if ((serverEvents & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges)
                events.Add("Security Changes");

            if ((serverEvents & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition)
                events.Add("DDL");

            if ((serverEvents & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity)
                events.Add("Administrative Actions");

            if ((serverEvents & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined)
                events.Add("User Defined Events");

            if ((serverEvents & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers)
            {
                events.Add("Privileged Users");
                events.Add("Privileged Users Events");
            }

            if (events.Count > 0)
            {
                StringBuilder eventStr = new StringBuilder((string)events[0]);

                for (int i = 1; i < events.Count; i++)
                {
                    if (i % 2 == 0)
                        eventStr.AppendFormat(",\r\n{0}", events[i]);
                    else
                        eventStr.AppendFormat(", {0}", events[i]);
                }
                return eventStr.ToString();
            }
            return "None";
        }


        private string ConvertDatabaseEvents(int databaseEvents)
        {
            ArrayList events = new ArrayList();

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges)
                events.Add("Security");

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition)
                events.Add("DDL");

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity)
                events.Add("Administrative Actions");

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification)
                events.Add("DML");

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select)
                events.Add("Select");

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)
                events.Add("SQL Text");

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions)
                events.Add("Transactions");

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns)
                events.Add("Sensitive Columns");

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange)
                events.Add("Before After Data Change");

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers)
            {
                events.Add("Privileged Users");
                events.Add("Privileged Users Events");
            }

            if (events.Count > 0)
            {
                StringBuilder eventStr = new StringBuilder((string)events[0]);

                for (int i = 1; i < events.Count; i++)
                {
                    if (i % 2 == 0)
                        eventStr.AppendFormat(",\r\n{0}", events[i]);
                    else
                        eventStr.AppendFormat(", {0}", events[i]);
                }
                return eventStr.ToString();
            }
            return "None";
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}