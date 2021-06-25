package com.idera.sqlcm.ui.widgets;

import java.util.List;

import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zul.Button;
import org.zkoss.zul.Intbox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.common.dashboard.Composers.DashboardBaseWidgetComposer;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqlcm.beEntities.AuditedInstanceBE;

//Start 5.3.1 Audited Instances widget 1)Multiple instances 2)Link to Instance View 3)Configurable instance limit

public class SQLcmAuditedInstance extends DashboardBaseWidgetComposer {
private static final long serialVersionUID = 1L;
	
	protected static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory
			.getLogger(SQLcmAuditedInstance.class);

	public static final String ZUL_URL = "widgets/sqlcm-audited-instances-widget.zul";

	public static final String AUDITED_INSTANCE = "auditedInstance";

	protected AnnotateDataBinder binder;

	@Wire
	Listbox instanceModel;

	private List<AuditedInstanceBE> auditedInstanceBE;
	
	private String baseURL;
	
	@Wire
	Intbox limit;
	
	@Wire
	Button save;
	
	public String getBaseURL() {
		return baseURL;
	}

	public void setBaseURL(String baseURL) {
		this.baseURL = baseURL;
	}

	public List<AuditedInstanceBE> getAuditedInstanceBE() {
		return auditedInstanceBE;
	}

	public void setAuditedInstanceBE(List<AuditedInstanceBE> auditedInstanceBE) {
		this.auditedInstanceBE = auditedInstanceBE;
	}
	
	private ListModelList<AuditedInstanceBE> auditedInstances=new ListModelList<AuditedInstanceBE>();
	
	 public ListModelList<AuditedInstanceBE> getAuditedInstances() {
		return auditedInstances;
	}

	public void setAuditedInstances(
			ListModelList<AuditedInstanceBE> auditedInstances) {
		this.auditedInstances = auditedInstances;
	}	
	
	@Override
	public void doAfterCompose(DashboardWidget widget) throws Exception {			
		super.doAfterCompose(widget);
		baseURL=config.getNavigationLink() + "instanceView/";
		Thread.sleep(3000);
	}

	@Override
	public TypeReference<List<AuditedInstanceBE>> getModelType() {
		return new TypeReference<List<AuditedInstanceBE>>() {
		};
	}
	
	
	@Override
	public void setWidgetData(Object obj)  {
		if(obj != null){
			setAuditedInstanceBE((List<AuditedInstanceBE>) obj);			
			for(int i=0; i<auditedInstanceBE.size();i++){			
				auditedInstances.add(auditedInstanceBE.get(i));					
			}
			instanceModel.setModel(auditedInstances);
		}
	}

	@Override
	public String getEventName() {
		return String.format("%d:%s", config.getId(),
				SQLcmAuditedInstance.class.getName());
	}

	@Override
	public String getDataURI() {
		String url = config.getProduct().getRestUrl() + "/GetAuditedInstancesWidgetData";
		return url;
	}
	
	@Listen("onClick = #save")
	public void save(){
		int tempIntValue = (limit.getValue()!=null)?limit.getValue():0;		
		auditedInstances.clear();
		if(auditedInstanceBE.size()<tempIntValue 
				|| tempIntValue==0){
			tempIntValue=auditedInstanceBE.size();
		}
		for(int i=0; i<tempIntValue; i++){
			auditedInstances.add(auditedInstanceBE.get(i));					
		}
		instanceModel.setModel(auditedInstances);		
	}
}

//Ends 5.3.1 Audited Instances widget 1)Multiple instances 2)Link to Instance View 3)Configurable instance limit 