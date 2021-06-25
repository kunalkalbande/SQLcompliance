package com.idera.sqlcm.ui.instancesAlertsRule;

import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.AddPrivilegedUsersViewModel;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventCondition;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.EventType;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.MatchType;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyAppNameViewModel.App;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zhtml.S;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Messagebox;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Listitem;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;

public class SpecifyDatabaseViewModel implements
		AddPrivilegedUsersViewModel.DialogListener {
	public static final String ZUL_URL_specifyDatabase = "~./sqlcm/eventFiltersView/specifyDatabase.zul";
    private CMInstance instance;
    EventCondition eventCondition = new EventCondition();
    EventField eventtype = new EventField();
	protected List<CMEntity> entitiesList;
	protected ListModelList<CMEntity> entitiesModel;
	public String[]  targetString = {};
	private String help;
	
	@Wire
	Radiogroup rgAlertRules;
	
	@Wire
	Textbox objectNameMatch;
	
	@Wire
	private Window specifyDatabase; 
	
	public String getHelp() {
		return this.help;
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

	
	public static enum Category {
		LISTED(1, ELFunctions.getLabel(SQLCMI18NStrings.LISTED),1),//TODO AS ask .NET team id
		EXCEPT_LISTED(2, ELFunctions.getLabel(SQLCMI18NStrings.EXCEPT_LISTED),
				0);
    	private String label;
        private int index;
        private int matchCondition;
        private Category(int index, String label, int matchCondition) {
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
				Sessions.getCurrent().setAttribute("specifyDataBaseRadio",
						currentInterval.label);
                break;
            }
        }
    }
	
	//4.1.1.5
	@Command("submitChoice")
	public void submitChoice(@BindingParam("comp") Window x) throws Exception {
		eventCondition.set_blanks(false);
		eventCondition.set_inclusive(true);
		eventCondition.set_nulls(false);
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
		Sessions.getCurrent().setAttribute("dbMatchString",matchString);
		Sessions.getCurrent().setAttribute("specifyDataBaseList",dataList);
		x.detach();
	}

	
	//private Listitem objectNameMatch;
	private Listbox listObjectMatch;	
    
 
    public void setListObjectMatch(Listbox listObjectMatch) {
        this.listObjectMatch = listObjectMatch;
    }   
    
    String eventDatabaseName;
    
    public Listbox getListObjectMatch() {
        return listObjectMatch;
    }
    
    public String getEventDatabaseName() {
        return eventDatabaseName;
    }
 
    public void setEventDatabaseName(String eventDatabaseName) {
        this.eventDatabaseName = eventDatabaseName;
    }

	private ListModelList<Data> dataList = new ListModelList<>();
	
	
	@Command
    @NotifyChange("dataList")
    public void addItem() {
		String eventDatabaseName = this.eventDatabaseName;
		if(eventDatabaseName!=null && (!eventDatabaseName.isEmpty())){
			boolean chkPass = true;
			for (int j = 0; j < dataList.getSize(); j++) {
				if (dataList.get(j).getDataBaseName().toString()
						.equals(eventDatabaseName)) {
					WebUtil.showInfoBoxWithCustomMessage("The list already contains "
							+ eventDatabaseName);
					chkPass = false;
					break;
				}	
			}
			if (chkPass){
		  
			Data data = new Data(eventDatabaseName);
			dataList.add(data);
			setEventDatabaseName("");
			objectNameMatch.setValue("");
			}
		}
	}
	
	public String[] GetTargetString() {
		targetString = new String[dataList.getSize()];
		for (int j = 0; j < dataList.getSize(); j++) {
	    targetString[j] = dataList.get(j).getDataBaseName().toString();
	    }
		return targetString;
     }

    public ListModelList<Data> getDataList() {
    	 return dataList;
    	}
    
    	public void setDataList(ListModelList<Data> dataList) {
    	this.dataList = dataList;
    	}
    	
	    public class Data {
		String dataBaseName;
		
		public String getDataBaseName() {
			return dataBaseName;
		}
		
		public void setDataBaseName(String dataBaseName) {
			this.dataBaseName = dataBaseName;
		}
		
		public Data(String dataBaseName) {
			super();
			this.dataBaseName = dataBaseName;
		}
	}	
	    
	    @Command("onItemClick")
	    public void onItemClick() {
	        enableRemoveButtonIfSelected();
	    }

	    private void enableRemoveButtonIfSelected() {
	        Set selectedItems = dataList.getSelection();
	    }

	    @Command("onRemoveBtnClick")
	    public void onRemoveBtnClick() {
	        Utils.removeAllSelectedItems(dataList);
	        enableRemoveButtonIfSelected();
	        BindUtils.postNotifyChange(null, null, this, "dataList");
	    }    
	    
	    
	    @AfterCompose
		public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
	    	help = "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
			Selectors.wireComponents(view, this, false);
		if (Sessions.getCurrent().getAttribute("dbMatchString") != null) {
				GetData();
			   }
			}

	      public void GetData() {
		String strMatchString = (String) Sessions.getCurrent().getAttribute(
				"dbMatchString");
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
		    	  // rd_inclusive.setSelectedIndex(1);
		} else {
		    	   eventCondition.set_inclusive(true);
		    	   rgAlertRules.setSelectedIndex(0);
		    	   //chk_Nulls.setChecked(true);
		    	   //rd_inclusive.setSelectedIndex(0);
		    	   //chk_Blank.setC
		    	  }
		    	  
		    	  String strCount = (String) EventNodeDataValue.get("count");
		for (int i = 0; i < Integer.parseInt(strCount); i++) {
		    	   String dataNameMatch = (String) EventNodeDataValue.get("" + i);
		    	   Data data = new Data(dataNameMatch);
		    	   setEventDatabaseName(dataNameMatch);
		    	   addItem();
		    	  }  
		     }
       }  
	 }