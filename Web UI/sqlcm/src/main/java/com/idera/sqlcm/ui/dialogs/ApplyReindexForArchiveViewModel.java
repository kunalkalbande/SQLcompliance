package com.idera.sqlcm.ui.dialogs;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.common.grid.EventsGridViewModel;
import com.idera.sqlcm.entities.CMApplyReindexForArchiveRequest;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebConstants;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.converter.ShortTimeConverter;

import org.apache.commons.lang.time.DurationFormatUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Window;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Arrays;
import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class ApplyReindexForArchiveViewModel {

    public static final String ZUL_URL = "~./sqlcm/dialogs/attachArchive/apply-reindex-for-archive-dialog.zul";

    private String archiveName;

    private ListModelList<Date> startTimeListModelList;

    private ListModelList<String> durationListModelList;

    private static final String DEFAULT_START_TIME = "Thu Jan 01 01:30:00 GMT 1970";

    private static final String START_TIME_FORMAT = "00:00 AM";

    private static final String DEFAULT_DURATION = "00:30";

    private static final String DISABLED_DURATION = "00:00";

    private String defaultStartTime = DEFAULT_START_TIME;

    private String defaultDuration = DEFAULT_DURATION;

    ArchivePropertiesViewModel.DialogListener listener;

    private ShortTimeConverter timeConverter;

    private Window window;

    private String helpURL = "http://wiki.idera.com/display/SQLCM/Attach+Archive+Database+window";

    @Wire
    Button okButton;

    @Wire
    Listbox startTimeListBox;

    @Wire
    Listbox durationListBox;

    @Wire
    Checkbox disableScheduleCheckBox;

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        HashMap<String, Object> args = (HashMap<String, Object>) Executions.getCurrent().getArg();
        archiveName = (String) args.get("archiveName");
        listener = (ArchivePropertiesViewModel.DialogListener) args.get("listener");
        timeConverter = new ShortTimeConverter();
        initTimeRanges();
    }

    public void initTimeRanges() {
        startTimeListModelList = new ListModelList<>();
        durationListModelList = new ListModelList<>();
        Calendar instance = Calendar.getInstance();
        try {
            instance.setTime(new SimpleDateFormat(WebConstants.SHORT_TIME_FORMAT).parse(START_TIME_FORMAT));
            int i = 1;
            while (i++ != 49) {
                startTimeListModelList.add(instance.getTime());
                durationListModelList.add(DurationFormatUtils.formatDuration(instance.getTimeInMillis(), WebConstants.DURATION_FORMAT));
                instance.add(Calendar.MINUTE, 30);
            }
            durationListModelList.add(DurationFormatUtils.formatDuration(instance.getTimeInMillis(), WebConstants.DURATION_FORMAT));
        } catch (ParseException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_PREPARE_TIME);
        }

    }

    public ListModelList<Date> getStartTimeListModelList() {
        return startTimeListModelList;
    }

    public void setStartTimeListModelList(ListModelList<Date> startTimeListModelList) {
        this.startTimeListModelList = startTimeListModelList;
    }

    public ListModelList<String> getDurationListModelList() {
        return durationListModelList;
    }

    public void setDurationListModelList(ListModelList<String> durationListModelList) {
        this.durationListModelList = durationListModelList;
    }

    public String getDefaultStartTime() {
        return defaultStartTime;
    }

    public String getDefaultDuration() {
        return defaultDuration;
    }

    public ShortTimeConverter getTimeConverter() {
        return timeConverter;
    }

    public void setTimeConverter(ShortTimeConverter timeConverter) {
        this.timeConverter = timeConverter;
    }

    @Command("okAction")
    public void okAction(@BindingParam("comp") Window x) {
        try {
            CMApplyReindexForArchiveRequest archive = prepareArchiveReindexData();
            DatabasesFacade.applyReindexForAttach(archive);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_APPLY_REINDEX_FOR_ARCHIVE);
        }
        refreshListener();
        x.detach();
    }

    private CMApplyReindexForArchiveRequest prepareArchiveReindexData() {
        CMApplyReindexForArchiveRequest archive = new CMApplyReindexForArchiveRequest();
        archive.setArchive(archiveName);
        archive.setIndexStartTime((Date) startTimeListBox.getSelectedItem().getValue());
        List<String> duration = Arrays.asList(((String) durationListBox.getSelectedItem().getValue()).split(":"));
        archive.setIndexDurationHours(Integer.parseInt(duration.get(0)));
        archive.setIndexDurationMinutes(Integer.parseInt(duration.get(1)));
        return archive;
    }

    private void refreshListener() {
        if (listener != null) {
            listener.refreshDatabaseArchives();
        }
    }

    @Command("disableSchedule")
    public void disableSchedule() {
        updateOkButton();
        if (disableScheduleCheckBox.isChecked()) {
            durationListBox.setSelectedItem(durationListBox.getItemAtIndex(0));
        }
    }

    @Command("onSelectDuration")
    public void onSelectDuration() {
        updateOkButton();
    }

    private void updateOkButton() {
        if (((String) durationListBox.getSelectedItem().getValue()).equals(DISABLED_DURATION) || disableScheduleCheckBox.isChecked()) {
            okButton.setDisabled(true);
        } else {
            okButton.setDisabled(false);
        }
    }

    public String getHelpURL() {
        return helpURL;
    }

    public static void showApplyReindexForArchiveWindow(String archiveName, EventsGridViewModel listener) {
        Map args = new HashMap();
        args.put("archiveName", archiveName);
        args.put("listener", listener);
        Window window = (Window) Executions.createComponents(ApplyReindexForArchiveViewModel.ZUL_URL, null, args);
        window.doHighlighted();
    }
}

















