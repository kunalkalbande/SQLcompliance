package com.idera.sqlcm.ui.dialogs;

import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.instances.CMThreshold;
import com.idera.sqlcm.enumerations.Period;
import com.idera.sqlcm.enumerations.StatsCategory;

import org.zkoss.zk.ui.Sessions;
import org.zkoss.zul.ListModelList;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Comparator;
import java.util.List;

public class CMThresholdAdapter {
    private long serverId;
    private StatsCategory statisticCategory;
    private long warningThreshold;
    private long criticalThreshold;
    private ListModelList<Period> periodListModelList;
    private boolean enabled;

    public static ListModelList<CMThresholdAdapter> wrapThresholdsData(List<CMThreshold> cmThresholdList) {
        ListModelList<CMThresholdAdapter> adaptedList = new ListModelList<>();
        for (CMThreshold cmThreshold: cmThresholdList) {
            adaptedList.add(new CMThresholdAdapter(cmThreshold));
        }
        adaptedList.sort(statsCategoryComparator, true);
        return adaptedList;
    }

    private static Comparator<CMThresholdAdapter> statsCategoryComparator = new Comparator<CMThresholdAdapter>() {
        @Override
        public int compare(CMThresholdAdapter o1, CMThresholdAdapter o2) {
            if (o1 != null && o2 != null) {
                return o1.getStatisticCategory().getOrderNumber() - o2.statisticCategory.getOrderNumber();
            }
            return 0;
        }
    };

    public CMThresholdAdapter(CMThreshold cmThreshold) {
        this.serverId = cmThreshold.getServerId();
        this.statisticCategory = StatsCategory.getByIndex(cmThreshold.getStatisticCategory());
        this.warningThreshold = cmThreshold.getWarningThreshold();
        this.criticalThreshold = cmThreshold.getCriticalThreshold();
        this.periodListModelList = getPeriodList(cmThreshold.getPeriod());
        this.enabled = cmThreshold.isEnabled();
    }

    private ListModelList<Period> getPeriodList(int periodIndex) {
        ListModelList<Period> periodListModelList = new ListModelList<>();
        periodListModelList.addAll(Arrays.asList(Period.PER_DAY, Period.PER_HOUR));
        periodListModelList.setSelection(Arrays.asList(Period.getByIndex(periodIndex)));
        return periodListModelList;
    }

    public static List<CMThreshold> unwrapThresholdAdapterList(ListModelList<CMThresholdAdapter> cmThresholdAdapterListModelList) {
        List<CMThreshold> cmThresholdList = new ArrayList<>();
        for (CMThresholdAdapter thresholdAdapter: cmThresholdAdapterListModelList) {
            cmThresholdList.add(unwrapThreshold(thresholdAdapter));
        }
        return cmThresholdList;
    }

    public static CMThreshold unwrapThreshold(CMThresholdAdapter adapter) {
        CMThreshold cmThreshold = new CMThreshold();
        cmThreshold.setServerId(adapter.getServerId());
        cmThreshold.setStatisticCategory(adapter.getStatisticCategory().getIndex());
        cmThreshold.setWarningThreshold(adapter.getWarningThreshold());
        cmThreshold.setCriticalThreshold(adapter.getCriticalThreshold());
        cmThreshold.setPeriod(Utils.getSingleSelectedItem(adapter.getPeriodListModelList()).getIndex());
        cmThreshold.setEnabled(adapter.isEnabled());
        return cmThreshold;
    }

    public long getServerId() {
        return serverId;
    }

    public void setServerId(long serverId) {
        this.serverId = serverId;
    }

    public StatsCategory getStatisticCategory() {
        return statisticCategory;
    }

    public void setStatisticCategory(StatsCategory statisticCategory) {
        this.statisticCategory = statisticCategory;
    }

    public long getWarningThreshold() {
        return warningThreshold;
    }

    public void setWarningThreshold(long warningThreshold) {
        this.warningThreshold = warningThreshold;
    }

    public long getCriticalThreshold() {
        return criticalThreshold;
    }

    public void setCriticalThreshold(long criticalThreshold) {
        this.criticalThreshold = criticalThreshold;
    }

    public ListModelList<Period> getPeriodListModelList() {
        return periodListModelList;
    }

    public void setPeriodListModelList(ListModelList<Period> periodListModelList) {
        this.periodListModelList = periodListModelList;
    }

    public boolean isEnabled() {
    	Sessions.getCurrent().setAttribute("isEnable", enabled);
        return enabled;
    }

    public void setEnabled(boolean enabled) {
        this.enabled = enabled;
    }
}
