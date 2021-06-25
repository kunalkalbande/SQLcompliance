package com.idera.sqlcm.ui.dialogs.adddatabasewizard.steps;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;

import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Radiogroup;


public class AuditCollectionLevelStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/adddatabasewizard/steps/audit-collection-level-step.zul";

    @Wire
    private Radiogroup rgFlow;

    private ListModelList<AuditLevel> nextStepListModelList;

    public enum AuditLevel {

        DEFAULT(0, ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_AUDIT_COLLECTION_RADIO_DEFAULT_LABEL),
                ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_AUDIT_COLLECTION_RADIO_DEFAULT_DESC),
                PermissionsCheckStepViewModel.ZUL_PATH,
                new Link(
                        ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_AUDIT_COLLECTION_RADIO_DEFAULT_LINK_URL),
                        ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_AUDIT_COLLECTION_RADIO_DEFAULT_LINK_LABEL)
                )),
        CUSTOM(1, ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_AUDIT_COLLECTION_RADIO_CUSTOM_LABEL),
               ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_AUDIT_COLLECTION_RADIO_CUSTOM_DESC),
                DatabaseAuditSettingsStepViewModel.ZUL_PATH),
        REGULATION(2, ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_AUDIT_COLLECTION_RADIO_REGULATION_LABEL),
                   ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_AUDIT_COLLECTION_RADIO_REGULATION_DESC),
                   RegulationGuidelinesStepViewModel.ZUL_PATH);

        private static final String EMPTY_STRING = "";

        public static class Link {

            public static final Link EMPTY = new Link(EMPTY_STRING, EMPTY_STRING);

            private String label;
            private String url;

            public Link(String linkUrl, String linkLabel) {

                if (linkUrl == null) {
                    throw new RuntimeException(" linkUrl is null! ");
                }

                if (linkLabel == null) {
                    throw new RuntimeException(" linkLabel is null! ");
                }

                this.label = linkLabel;
                this.url   = linkUrl;
            }

            public String getLabel() {
                return (EMPTY_STRING.equals(label))? url : label;
            }

            public String getUrl() {
                return url;
            }

            public boolean isDefined() {
                return !EMPTY_STRING.equals(url);
            }
        }

        private int id;
        private String label;
        private String description;
        private Link link;

        private String nextStepZul;

        private AuditLevel(int id, String label, String desc, String nextStepZul) {
            this(id, label, desc, nextStepZul, null);
        }

        private AuditLevel(int id, String label, String desc, String nextStepZul, Link link) {
            this.id = id;
            this.label = label;
            this.description = desc;
            this.nextStepZul = nextStepZul;
            this.link = (link == null) ? Link.EMPTY : link;
        }

        public int getId() {
            return id;
        }

        public String getLabel() {
            return label;
        }

        public String getLabelAndDescription() {
            return label + description;
        }

        public String getNextStepZul() {
            return nextStepZul;
        }

        public String getName() {
            return this.name();
        }

        public Link getLink() {
            return link;
        }

        public static AuditLevel getById(long id) {
            for(AuditLevel e : values()) {
                if(e.id == id) {
                    return e;
                }
            }
            return null;
        }
    }

    public AuditCollectionLevelStepViewModel() {
        super();
        nextStepListModelList = new ListModelList();
        nextStepListModelList.add(AuditLevel.DEFAULT);
        nextStepListModelList.add(AuditLevel.CUSTOM);
        nextStepListModelList.add(AuditLevel.REGULATION);
    }

    @Override
    protected void doOnShow(AddDatabasesSaveEntity wizardSaveEntity) {
        getNextButton().setDisabled(false);
    }

    public ListModelList<AuditLevel> getNextStepListModelList() {
        return nextStepListModelList;
    }

    @Override
    public String getNextStepZul() {
        AuditLevel radioValue = getSelectedAuditLevel();
        return radioValue.getNextStepZul();
    }

    private AuditLevel getSelectedAuditLevel() {
        AuditLevel radioValue = rgFlow.getSelectedItem().getValue();
        if (radioValue == null) {
            throw new RuntimeException(" Not selected next step ");
        }
        return radioValue;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_AUDIT_COLLECTION_LEVEL_TIPS);
    }

    @Command("afterRenderGrid")
    public void afterRenderGrid() {
        // select first item of radio group
        if (rgFlow.getItemCount() > 0) {
            rgFlow.setSelectedIndex(0);
        }
    }

    @Override
    public void onBeforeNext(AddDatabasesSaveEntity wizardSaveEntity) {
        wizardSaveEntity.setCollectionLevel(getSelectedAuditLevel().getId());
    }

	@Override
	public String getHelpUrl() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public boolean onBeforeCancel(AddDatabasesSaveEntity wizardSaveEntity) {
		// TODO Auto-generated method stub
		return false;
	}
}
