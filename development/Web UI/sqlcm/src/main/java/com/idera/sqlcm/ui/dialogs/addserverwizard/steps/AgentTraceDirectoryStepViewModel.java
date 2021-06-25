package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;


import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Textbox;

import java.util.Arrays;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import java.util.regex.PatternSyntaxException;

public class AgentTraceDirectoryStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/agent-trace-directory-step.zul";

    public static final int MAX_TRACE_DIR_PATH_LENGTH = 180;
    public static final String WINDOW_DIR_PATH_REGEXP_PATTERN = "^[a-zA-Z]:\\\\(?:[^\\\\/:*?\"<>|\r\n]+\\\\)*[^\\\\/:*?\"<>|\r\n]*$"; // Valid example: C:\someFolder

    @Wire
    private Textbox tbTraceDir;

    private String traceDirPath = "";

    public String getTraceDirPath() {
        return traceDirPath;
    }

    public void setTraceDirPath(String traceDirPath) {
        this.traceDirPath = traceDirPath;
    }

    @Override
    public String getNextStepZul() {
        return AddServerInvisibleStepViewModel.ZUL_PATH;
    }

    @Override
    protected void onDoAfterWire() {
        enableTextBoxIfSpecifySelected();
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AGENT_TRACE_DIR_TIPS);
    }

    @Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+SQLcompliance+Agent+Trace+Directory+window";
    }

    private ListModelList<TraceDirOption> traceDirOptionListModelList;

    public ListModelList<TraceDirOption> getTraceDirOptionListModelList() {
        return traceDirOptionListModelList;
    }

    public void setTraceDirOptionListModelList(ListModelList<TraceDirOption> traceDirOptionListModelList) {
        this.traceDirOptionListModelList = traceDirOptionListModelList;
    }
    
    public enum TraceDirOption {

        TRACE_DIR_USE_DEFAULT(1, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_TRACE_DIR_USE_DEFAULT)),
        TRACE_DIR_SPECIFY(2, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_TRACE_DIR_SPECIFY));

        private int id;
        private String label;

        TraceDirOption(int id, String label) {
            this.id = id;
            this.label = label;
        }

        public int getId() {
            return id;
        }

        public String getLabel() {
            return label;
        }

        public String getName() {
            return this.name();
        }

        public static TraceDirOption getById(long id) {
            for(TraceDirOption e : values()) {
                if(e.id == id) {
                    return e;
                }
            }
            return null;
        }
    }

    @Command("onItemChecked")
    public void onItemChecked() {
        enableTextBoxIfSpecifySelected();
    }

    private void enableTextBoxIfSpecifySelected() {
        TraceDirOption traceDirOption = Utils.getSingleSelectedItem(traceDirOptionListModelList);
        if (TraceDirOption.TRACE_DIR_SPECIFY.equals(traceDirOption)) {
            tbTraceDir.setDisabled(false);
        } else {
            tbTraceDir.setDisabled(true);
        }
    }

    public AgentTraceDirectoryStepViewModel() {
        super();
        traceDirOptionListModelList = new ListModelList();
        traceDirOptionListModelList.addAll(Arrays.asList(TraceDirOption.values()));
        traceDirOptionListModelList.setSelection(Arrays.asList(TraceDirOption.TRACE_DIR_USE_DEFAULT));
    }

    @Override
    public boolean isValid() {
        if (tbTraceDir.isDisabled()) {
            return true;
        }
        return validateTraceDirPath();
    }

    private boolean validateTraceDirPath() {
        String trimmedTraceDirPath = traceDirPath.trim();
        if (!trimmedTraceDirPath.isEmpty() &&
            trimmedTraceDirPath.length() <= MAX_TRACE_DIR_PATH_LENGTH &&
            isTraceDirMatchPattern(traceDirPath)) {
            Clients.clearWrongValue(tbTraceDir);
            return true;
        }
        Clients.wrongValue(tbTraceDir, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_TRACE_DIR_INVALID_PATH));
        return false;
    }

    private boolean isTraceDirMatchPattern(String traceDirPath) {
        boolean match = false;
        try {
            Pattern regex = Pattern.compile(WINDOW_DIR_PATH_REGEXP_PATTERN);
            Matcher regexMatcher = regex.matcher(traceDirPath);
            match = regexMatcher.matches();
        } catch (PatternSyntaxException ex) {
            throw new RuntimeException(" Syntax error in the regular expression for validate login ");
        }
        return match;
    }

    @Override
    public void onBeforeNext(AddServerWizardEntity wizardEntity) {
        wizardEntity.getAddServerEntity().getAgentDeploymentProperties().setAgentTraceDirectory(traceDirPath.trim());
    }
}
