package com.idera.sqlcm.ui.instancesAlertsRule;

import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Set;

import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Window;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CategoryData;
import com.idera.sqlcm.entities.CategoryRequest;
import com.idera.sqlcm.entities.CategoryResponse;
import com.idera.sqlcm.facade.AlertRulesFacade;


public class ExcludeCertainEventType{
	
	HashMap<String,String> hm=new HashMap<String,String>();
	
	List<CategoryData> entitiesListEvents;

	String events[];
	
	@Wire
	Listbox entitiesListBoxEvents;
    
    public List<CategoryData> getEntitiesListEvents() {
		return entitiesListEvents;
	}

	public void setEntitiesListEvents(List<CategoryData> entitiesListEvents) {
		this.entitiesListEvents = entitiesListEvents;
	}

	
	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		Selectors.wireComponents(view, this, false);
		String id = "";
 		if(Sessions.getCurrent().getAttribute("eventTypeId")!=null)
 		id=(String)Sessions.getCurrent().getAttribute("eventTypeId");
 		if(!id.equals("101"))
 		{	
 			if(id.equals(""))
 				id = "SECURITY";  			
 			try{
 				boolean tmp = false;
     			AlertRulesFacade alertRulesFacade = new AlertRulesFacade();
     			CategoryResponse categoryResponse = new CategoryResponse();
     			CategoryRequest categoryRequest = new CategoryRequest();
     			categoryRequest.setCategory(id);
     			categoryResponse = alertRulesFacade.getCategoryInfo(categoryRequest);
     			entitiesListEvents = categoryResponse.getCategoryTable();
     			
     			Collections.sort(entitiesListEvents, new Comparator<CategoryData>() {
     		        @Override
     		        public int compare(final CategoryData object1, final CategoryData object2) {
     		            return object1.getName().compareTo(object2.getName());
     		        }
     		       } );
     			
     			if(Sessions.getCurrent().getAttribute("ExecludeCertainEventIds")!=null 
     					&& !Sessions.getCurrent().getAttribute("ExecludeCertainEventIds").toString().isEmpty()){
     				events = Sessions.getCurrent().getAttribute("ExecludeCertainEventIds").toString().split(",");
     				tmp=true;
     			}
     			for(int i=0;i<entitiesListEvents.size();i++){
     				entitiesListEvents.get(i).setCheckBool(false);
     				if(tmp){
	     				for(int j=0;j<events.length;j++){
	     					if(Integer.parseInt(events[j])==entitiesListEvents.get(i).getEvtypeid()){
	     						hm.put(events[j], events[j]);
	     						entitiesListEvents.get(i).setCheckBool(true);
	     						break;
	     					}
	     				}
     				}
     			}
     			entitiesListBoxEvents.setModel(new ListModelList<CategoryData>(entitiesListEvents));
 			}
 			catch(RestException e){}
 		}
	}
	
	
	@Command
	public void closeDialog(@BindingParam("comp") Window x) {
		x.detach();
	}
	
	@Command("submitList")
	public void submitList(@BindingParam("comp") Window x) {
		 Set keys = hm.keySet();
	     Iterator itr = keys.iterator();
	     	String matchString="";
	     	int i=0;
	        String key="";
	        String value="";
	        while(itr.hasNext())
	        {
	        	key = (String)itr.next();
	            value = (String)hm.get(key);
	        	if(i!=0){
	        		matchString+=","+value;	        		
	        	}
	        	else{
	        		matchString=value;
	        	}	            
	            i++;
	        }
	        if(!matchString.equals(" "))
            {
	        	Sessions.getCurrent().setAttribute("ExecludeCertainEventIds",matchString);
            	matchString="include(1)0value("+matchString.length()+")"+matchString;
            	Sessions.getCurrent().setAttribute("Type",matchString);
            }
	        else
	        	Sessions.getCurrent().removeAttribute("ExecludeCertainEventIds");
		
		x.detach();
	}
	

	@Command("oncheckentity")
	public void oncheckentity(@BindingParam("eventid") String eventId){
		
		if(eventId.equals(hm.get(eventId))){
			hm.remove(eventId);
		}
		else{
			hm.put(eventId, eventId);
		}
	}
	
	@Command("selectAll")
	public void selectAll(){
		for(int i=0;i<entitiesListEvents.size();i++){
				entitiesListEvents.get(i).setCheckBool(true);
				int temp = entitiesListEvents.get(i).getEvtypeid();
				hm.put(Integer.toString(temp), Integer.toString(temp));
			}
			entitiesListBoxEvents.setModel(new ListModelList<CategoryData>(entitiesListEvents));
	}
	
	@Command("clearAll")
	public void clearAll(){
		for(int i=0;i<entitiesListEvents.size();i++){
			entitiesListEvents.get(i).setCheckBool(false);			
		}
		hm.clear();
		entitiesListBoxEvents.setModel(new ListModelList<CategoryData>(entitiesListEvents));
	}
	


}
