package com.idera.sqlcm.ui.instancesAlertsRule;

import com.idera.sqlcm.Utils;
import com.idera.sqlcm.common.grid.CommonFacade;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.entities.CMPermission;
import com.idera.sqlcm.entities.CMPermissionInfo;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.instances.CMInstanceProperties;
import com.idera.sqlcm.enumerations.DefaultDBPermission;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.converter.PermissionStatusToCssStyleConverter;
import com.idera.sqlcm.ui.converter.PermissionStatusToImagePathConverter;
import com.idera.sqlcm.ui.converter.PermissionStatusToLabelConverter;
import com.idera.sqlcm.ui.dialogs.AddPrivilegedUsersViewModel;
import com.idera.sqlcm.ui.dialogs.CMThresholdAdapter;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventCondition;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.EventType;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.MatchType;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.permissionFailDialog.PermissionFailConfirmViewModel;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyDatabaseViewModel.Category;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyDatabaseViewModel.Data;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.Converter;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Messagebox;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.TreeMap;

public class SpecifyHostNameViewModel implements
		AddPrivilegedUsersViewModel.DialogListener {
	EventCondition eventCondition = new EventCondition();
    EventField eventtype = new EventField();
    String[]  targetString = {};
	protected Map<String, Object> filterRequest = new TreeMap<>();
	
	@Wire
	Textbox objectNameMatch;

	public static final String INSTANCE_ID = "instance-id";
	private CMInstance instance;
	
	@Wire
	Radiogroup rgAlertRules;
	
	private String help;
	private String optimizePerformanceLink;

	public String getHelp() {
		return this.help;
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		help = "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
		Selectors.wireComponents(view, this, false);
		if (Sessions.getCurrent().getAttribute("hostMatchString") != null) {
			GetData();
		   }
		}

    
    
	@Command
	public void closeDialog(@BindingParam("comp") Window x) {
		x.detach();
	}

	@Override
	public void onOk(long instanceId,
			List<CMInstancePermissionBase> selectedPermissionList) {
	}

	@Override
	public void onCancel(long instanceId) {
	}	

	public class PermissionFailConfirmDialogListenerImpl implements
			PermissionFailConfirmViewModel.PermissionFailConfirmDialogListener {
		@Override
		public void onIgnore() {
			/* getNextButton().setDisabled(false); */
		}

		@Override
		public void onReCheck() {
			// do nothing
		}
	}
	
	public static enum Category {
		LISTED(1, ELFunctions.getLabel(SQLCMI18NStrings.LISTED),1),//TODO AS ask .NET team id
		EXCEPT_LISTED(2, ELFunctions.getLabel(SQLCMI18NStrings.EXCEPT_LISTED),
				0);
    	private String label;
        private int index;
        private int matchCondition;
        private Category(int index, String label,int matchCondition) {
            this.label = label;
            this.index = index;
            this.matchCondition = matchCondition;
        }

        public String getLabel() {
            return label;
        }

        public String getName() {
            return this.name();
        }
        

        public int getIndex() {
            return index;
        }
        
        public int getMatchCondition() {
            return matchCondition;
        }
        
    }
	  private Category currentInterval = Category.LISTED;
	  private ListModelList<Category> intervalListModelList;
	  private void initIntervalList(int selectedIndex) {
	        intervalListModelList = new ListModelList<>();
	        intervalListModelList.add(Category.LISTED);
	        intervalListModelList.add(Category.EXCEPT_LISTED);
	        currentInterval = intervalListModelList.get(selectedIndex);
	        intervalListModelList.setSelection(Arrays.asList(currentInterval));
	    }
	
	@Command("selectAddEventFilter")
	public void selectAddEventFilter(
			@BindingParam("radioGroup") Radiogroup radioGroup)
			throws RestException {
    	int iSelected = radioGroup.getSelectedIndex();
    	initIntervalList(iSelected);
    	Set<Category> selectedIntervals = intervalListModelList.getSelection(); // must contain only 1 item because single selection mode.
        if (selectedIntervals != null && !selectedIntervals.isEmpty()) {
            for (Category i : selectedIntervals) {
                currentInterval = i;
				Sessions.getCurrent().setAttribute("specifyHostRadio",
						currentInterval.label);
                break;
            }
        }
    }
	
	//4.1.1.5
		@Command("submitChoice")
		public void submitChoice(@BindingParam("comp") Window x) throws Exception {
			eventCondition.set_blanks(false);
			eventCondition.set_nulls(false);
			eventCondition.set_inclusive(true);
			eventCondition.set_boolValue(true);
			eventtype.setDataFormat(MatchType.String);
			eventtype.set_type(EventType.SqlServer);
		eventCondition.set_inclusive(rgAlertRules.getSelectedIndex() == 0 ? true
				: false);
		if (dataList != null && (!dataList.isEmpty())) {
				eventCondition.set_targetStrings(GetTargetString());
			}
		String matchString = eventCondition.UpdateMatchString(eventtype,
				eventCondition);
			Sessions.getCurrent().setAttribute("hostMatchString",matchString);
			Sessions.getCurrent().setAttribute("specifyHostList",dataList);
			x.detach();
		}
		
	public String[] GetTargetString() {
			targetString = new String[dataList.getSize()];
			for (int j = 0; j < dataList.getSize(); j++) {
		    targetString[j] = dataList.get(j).getHostName().toString() ;
		    }
			return targetString;
	     }
		
    String hostNameMatch;
    public String getHostNameMatch() {
        return hostNameMatch;
    }
 
    public void setHostNameMatch(String hostNameMatch) {
        this.hostNameMatch = hostNameMatch;
    }

    private ListModelList<Host> dataList = new ListModelList<>();
	
    @Command
    @NotifyChange("dataList")
    public void addItem() {
    	String hostNameMatch = this.hostNameMatch;
    	if(hostNameMatch!=null && (!hostNameMatch.isEmpty())){
			boolean chkPass = true;
			for (int j = 0; j < dataList.getSize(); j++) {
				if (dataList.get(j).getHostName().toString()
						.equals(hostNameMatch)) {
					WebUtil.showInfoBoxWithCustomMessage("The list already contains "
							+ hostNameMatch);
					chkPass = false;
					break;
				}	
			}
			if (chkPass){
			Host host = new Host(hostNameMatch);
			dataList.add(host);
			setHostNameMatch("");
			objectNameMatch.setValue("");
			}
		}
	}
	
    public ListModelList<Host> getDataList() {
    	 return dataList;
    	}
    	public void setDataList(ListModelList<Host> dataList) {
    	this.dataList = dataList;
    	}
	    public class Host {
		String hostName;
		
		public String getHostName() {
			return hostName;
		}
		public void setHostName(String hostName) {
			this.hostName = hostName;
		}
		public Host(String hostName) {
			super();
			this.hostName = hostName;
		}
	}	
	    
	    @Command("onItemClick")
	    public void onItemClick() {
	        enableRemoveButtonIfSelected();
	    }

	    private void enableRemoveButtonIfSelected() {
	        Set selectedItems = dataList.getSelection();
	        if (selectedItems != null && selectedItems.size() > 0) {
	           // removeBtn.setDisabled(false);
	        } else {
	            //removeBtn.setDisabled(true);
	        }
	    }

	    @Command("onRemoveBtnClick")
	    public void onRemoveBtnClick() {
	        Utils.removeAllSelectedItems(dataList);
	        enableRemoveButtonIfSelected();
	        BindUtils.postNotifyChange(null, null, this, "dataList");
	    }    
	    
	    
	    
	    public void GetData() {
		String strMatchString = (String) Sessions.getCurrent().getAttribute(
				"hostMatchString");
		if(!strMatchString.isEmpty()){
	    	  Map<String, String> EventNodeDataValue = new HashMap<String, String>();
	    	  String active = strMatchString;
	    	  int index = strMatchString.indexOf("(");

	    	  try {
	    	   while (index != -1) {
	    	    String sKey, sValue;
	    	    String length;

	    	    sKey = active.substring(0, index);
	    	    active = active.substring(index + 1);
	    	    index = active.indexOf(")");
	    	    length = active.substring(0, index);
	    	    active = active.substring(index + 1);
	    	    sValue = (active.subSequence(0, Integer.parseInt(length))
	    	      .toString());
	    	    active = (active.substring(Integer.parseInt(length)).toString());
	    	    EventNodeDataValue.put(sKey, sValue);
	    	    index = active.indexOf("(");
	    	   }
	    	  } catch (Exception e) {
	    	   try {
	    	    throw new Exception("Improperly formed KeyValue string.", e);
	    	   } catch (Exception e1) {
	    	    // TODO Auto-generated catch block
	    	    e1.printStackTrace();
	    	   }
	    	  }
	    	  if (active.length() > 0)
	    	   try {
	    	    throw new Exception("Improperly formed KeyValue string.");
	    	   } catch (Exception e) {
	    	    // TODO Auto-generated catch block
	    	    e.printStackTrace();
	    	   }
	    	  
	    	  String strBlanks = (String) EventNodeDataValue.get("blanks");
		if (strBlanks.equals("0")) {
	    	   eventCondition.set_blanks(false);
		} else {
	    	   eventCondition.set_blanks(true);
	    	  }
	    	  
	    	  String strNulls = (String) EventNodeDataValue.get("nulls");
		if (strNulls.equals("0")) {
	    	   eventCondition.set_nulls(false);
		} else {
	    	   eventCondition.set_nulls(true);
	    	  }
	    	  
	    	  String strInclusive = (String) EventNodeDataValue.get("include");
		if (strInclusive.equals("0")) {
	    	   eventCondition.set_inclusive(false);
	    	   rgAlertRules.setSelectedIndex(1);
		} else {
	    	   eventCondition.set_inclusive(true);
	    	   //chk_Nulls.setChecked(true);
	    	   rgAlertRules.setSelectedIndex(0);
	    	   //chk_Blank.setC
	    	  }
	    	  
	    	  String strCount = (String) EventNodeDataValue.get("count");
		for (int i = 0; i < Integer.parseInt(strCount); i++) {
	    	   String hostNameMatch = (String) EventNodeDataValue.get("" + i);
	    	   Host host = new Host(hostNameMatch);
	    	   setHostNameMatch(hostNameMatch);
	    	   addItem();
	    	  }  
	     }
	   }
}
