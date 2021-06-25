package com.idera.sqlcm.ui.dialogs;

import java.io.File;
import java.io.IOException;
import java.net.URL;

import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.entities.CMResponse;
import com.idera.sqlcm.entities.License;
import com.idera.sqlcm.facade.LicenseFacade;
import com.idera.sqlcm.facade.RefreshDurationFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.instancesAlerts.InstancesAlertsGridViewModel;
import com.idera.sqlcm.utils.SQLCMConstants;

import org.zkoss.bind.BindUtils;
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
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Button;
import org.zkoss.zul.Intbox;
import org.zkoss.zul.Messagebox;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;

public class ManageConfigRefreshViewModel {
	public static final String ZUL_URL = "~./sqlcm/dialogs/manageConfigRefresh.zul";
	
	@Wire
	private Intbox newRefresh;
	
	@Wire
	private Button saveButton;
	
	private String timeDuration;
	
	private String help;
	
	private int refreshDuration;

	public int getRefreshDuration() {
		return refreshDuration;
	}

	public void setRefreshDuration(int refreshDuration) {
		this.refreshDuration = refreshDuration;
	}

	public String getHelp() {
		return help;
	}

	public void setTimeDuration(String timeDuration) {
		this.timeDuration = timeDuration;
	}
	
	 public static void showManageConfigRefresh() {
	        Window window = (Window) Executions.createComponents(ZUL_URL, null, null);
	        window.doHighlighted();
	    }
	 
	  @Command
	  public void closeDialog(@BindingParam("comp") Window x) {
	      x.detach();
	  }
	  
	  @AfterCompose
	    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
	        Selectors.wireComponents(view, this, false);
	        help = "http://wiki.idera.com/x/LQQsAw";
	        initDuration();
	    }
	  
	  @Command
	  public void enableSave(){
		  if(newRefresh.getValue() != null && !newRefresh.getValue().equals("")){
			  saveButton.setDisabled(false);
		  }
	  }
	  
	  @Command
	    public void applyNewDuration() {
	        try {
	        	timeDuration = newRefresh.getValue().toString();
	        	if(isValidDuration(timeDuration))
	        	{
	        		int duration=Integer.parseInt(timeDuration);
		        	if(duration >=30 && duration<=3600)
		        	{
		        		String result=RefreshDurationFacade.setRefreshDuration(duration);
		        		if(result.equals("OK"))
		        		{
		        			 WebUtil.showInfoBox(SQLCMI18NStrings.MESSAGE_APPLY_REFRESH_DURATION_SUCCESS);
		                } else {
		                    WebUtil.showErrorBox(new RestException(), SQLCMI18NStrings.MESSAGE_FAILED_TO_APPLY_REFRESH_DURATION,
		                    		timeDuration);
		                }
		        	}
		        	else
		        	{		        		
		        		WebUtil.showInfoBoxWithCustomMessage("The value should be between 30 and 3600.");
		        	}
	        	}
				else
				{
				WebUtil.showInfoBoxWithCustomMessage("Invalid duration.");
				}
	        } catch (RestException e) {
	            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_APPLY_NEW_REFRESH_VALUE);
	        }
	    }


	  @Command
	    public void enableButtons() {
	    }
	  
	  @Command
	    public void initDuration() {
	        try {
	        String refreshDuration= RefreshDurationFacade.getRefreshDuration();
	        int x=Integer.parseInt(refreshDuration);
	        //int y=x*1000;
	        setRefreshDuration(x);	           
	        } catch (RestException e) {
	            //WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_APPLY_NEW_CM_LICENSE);
	        }
	    }

	  public boolean isValidDuration(String duration) {
			try
		    {
		        Integer.parseInt(duration);
		        return true;
		    } catch (NumberFormatException ex)
		    {
		        return false;
		    }
			
		}
	  
	  @Command
	  public void setValue(){		  
		  if(refreshDuration < 30 || refreshDuration > 3600)
		  {
			  Clients.showNotification("The value should be between 30 and 3600.", "warning",
      				newRefresh, "end_center", 3000);
      		if(refreshDuration <30)
      			refreshDuration = 30;
      		else 
      			refreshDuration = 3600;
      		BindUtils.postNotifyChange(null, null, this, "refreshDuration");
		  }
	  }
	  
}
