package com.idera.sqlcm.ui.dialogs.regulationDetails;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.RegulationSection;
import com.idera.sqlcm.facade.RegulationFacade;
import org.zkoss.bind.annotation.*;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Tab;
import org.zkoss.zul.Tabbox;
import org.zkoss.zul.Tabpanel;
import org.zkoss.zul.Tabpanels;
import org.zkoss.zul.Window;

import java.util.*;

public class RegulationDetailsViewModel {

    public static final String ZUL_URL = "~./sqlcm/dialogs/regulationDetails/regulation-details-dialog.zul";
    private static final String GRID_INFO_ZUL_URL = "~./sqlcm/dialogs/regulationDetails/regulation-details-info-grid.zul";
    private static final String REGULATION_TYPE_NAMES_TO_DISPLAY_ARG = "regulation_type_names_to_display_arg";

    @Wire
    private Window regulationDetailsWindow;

    @Wire
    private Tabbox tbSections;

    @Wire
    private Tabpanels tpSections;

    private List<RegulationSection> regulationSectionDictionary;

    private Collection<String> regulationTypeNamesToDisplay = new ArrayList<>(5);
    
    private boolean onlyCustomApplied = true;

	public boolean isOnlyCustomApplied() {
		return onlyCustomApplied;
	}

	@AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view,
                             @ExecutionArgParam(REGULATION_TYPE_NAMES_TO_DISPLAY_ARG) Collection<String> regulationTypeNamesToDisplay) {
        Selectors.wireComponents(view, this, false);

        this.regulationTypeNamesToDisplay.addAll(regulationTypeNamesToDisplay);

        try {
            regulationSectionDictionary = RegulationFacade.getRegulationSectionDictionary();
            initSectionTabs(regulationSectionDictionary);
        } catch (RestException e) {
            throw new RuntimeException(e);
        }
    }

    private void initSectionTabs(List<RegulationSection> regulationSectionDictionary) {
        for (RegulationSection section: regulationSectionDictionary) {
        	 for (String s : regulationTypeNamesToDisplay) {
        	       if(section.getName().contains(s)){
        	    	   Tab newTab = null;
        	        	if(section.getName().contains("PCI DSS"))
        	        	{
        	        		newTab = new Tab("PCI DSS");
            }
        	        	else if(section.getName().contains("HIPAA"))
        	        	{
        	        		newTab = new Tab("HIPAA");
        	        	} 
        	        	else if(section.getName().contains("DISA STIG"))
        	        	{
        	        		newTab = new Tab("DISA STIG");
        	        	} 
        	        	else if(section.getName().contains("NERC"))
        	        	{
        	        		newTab = new Tab("NERC");
        	        	} 
        	        	else if(section.getName().contains("CIS"))
        	        	{
        	        		newTab = new Tab("CIS");
        	        	} 
        	        	else if(section.getName().contains("SOX"))
        	        	{
        	        		newTab = new Tab("SOX");
        	        	}
        	        	else if(section.getName().contains("FERPA"))
        	        	{
        	        		newTab = new Tab("FERPA");
        	        	}
            newTab.setSelected(true);
            Tabpanel newTabPanel = new Tabpanel();
            newTabPanel.setParent(tbSections.getTabpanels());
            Map<String, Object> args = new HashMap<>();
            args.put(RegulationDetailsGridViewModel.SECTION_VALUES_ARG, new ListModelList<>(section.getValueList()));
            Component component = Executions.createComponents(GRID_INFO_ZUL_URL, newTabPanel, args);
            component.setParent(newTabPanel);
            newTabPanel.appendChild(component);
            tbSections.getTabs().appendChild(newTab);
            if(onlyCustomApplied)
            	onlyCustomApplied = false;
            	
        	        }
        	       }
        }
    }

    @Command("closeCommand")
    public void close() {
        regulationDetailsWindow.detach();
    }

    public static void show(Collection<String> regulationTypeNamesToDisplay) {
        Map args = new HashMap();
        args.put(REGULATION_TYPE_NAMES_TO_DISPLAY_ARG, regulationTypeNamesToDisplay);
        Window window = (Window) Executions.createComponents(ZUL_URL, null, args);
        window.doHighlighted();
    }
}

















