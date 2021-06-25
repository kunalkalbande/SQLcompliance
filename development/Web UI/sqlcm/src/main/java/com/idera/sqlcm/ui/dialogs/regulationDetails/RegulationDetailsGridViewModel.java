package com.idera.sqlcm.ui.dialogs.regulationDetails;

import com.idera.sqlcm.entities.RegulationSectionValue;
import org.apache.log4j.Logger;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.ExecutionArgParam;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zul.ListModelList;

public class RegulationDetailsGridViewModel {

    private static final String GRID_INFO_ZUL_URL = "~./sqlcm/instancedetails/addserverwizard/regulationDetails/regulation-details-info-grid.zul";

    public static final String SECTION_VALUES_ARG = "sectionValues";

    private static final Logger logger = Logger.getLogger(RegulationDetailsGridViewModel.class);

    private ListModelList<RegulationSectionValue> regulationInfoList;

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view,
                             @ExecutionArgParam(SECTION_VALUES_ARG) ListModelList<RegulationSectionValue> regulationInfoList) {
        Selectors.wireComponents(view, this, false);
        this.regulationInfoList = regulationInfoList;

    }

    public ListModelList<RegulationSectionValue>  getRegulationInfoList() {
        return regulationInfoList;
    }

}

















