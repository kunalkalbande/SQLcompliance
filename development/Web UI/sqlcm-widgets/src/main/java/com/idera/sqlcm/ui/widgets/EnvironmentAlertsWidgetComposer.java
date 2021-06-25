package com.idera.sqlcm.ui.widgets;

import com.idera.sqlcm.entities.EnvironmentAlertsDetailed;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zul.A;
import org.zkoss.zul.Label;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.common.dashboard.Composers.DashboardBaseWidgetComposer;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqlcm.entities.EnvironmentAlerts;

/**
 * 
 * @author Amarendra
 *
 */
public class EnvironmentAlertsWidgetComposer extends DashboardBaseWidgetComposer {

	private static final long serialVersionUID = 1L;
	
	protected static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory
			.getLogger(EnvironmentAlertsWidgetComposer.class);

	public static final String ZUL_URL = "widgets/environment-alerts-widget.zul";

	public static final String AUDITED_INSTANCE = "auditedInstance";

	protected AnnotateDataBinder binder;
	
	private EnvironmentAlertsDetailed environmentAlerts;
	
	@Wire
	Label total;
	
	@Wire
	Label severe;
	
	@Wire
	Label high;
	
	@Wire
	Label medium;
	
	@Wire
	Label low;
	
	@Wire
	A instanceTitle;

	@Wire
	Label totalDb;

	@Wire
	Label severeDb;

	@Wire
	Label highDb;

	@Wire
	Label mediumDb;

	@Wire
	Label lowDb;

	@Wire
	A databaseTitle;
	
	@Override
	public void doAfterCompose(DashboardWidget widget) throws Exception {
		super.doAfterCompose(widget);
	}

	@Override
	public TypeReference<EnvironmentAlertsDetailed> getModelType() {
		return new TypeReference<EnvironmentAlertsDetailed>() {
		};
	}

	@Override
	public void setWidgetData(Object obj)  {
		
		if(obj != null){
			environmentAlerts = (EnvironmentAlertsDetailed) obj;
			EnvironmentAlerts envAlert = environmentAlerts.getAlertsStatus();
			total.setValue(envAlert.getAuditedInstances().total + "");
			severe.setValue(envAlert.getAuditedInstances().severe + "");
			high.setValue(envAlert.getAuditedInstances().high + "");
			medium.setValue(envAlert.getAuditedInstances().medium + "");
			low.setValue(envAlert.getAuditedInstances().low + "");
			logger.info("Instance Url :", config.getNavigationLink());
			instanceTitle.setHref(config.getNavigationLink() + AUDITED_INSTANCE);

			totalDb.setValue(envAlert.getAuditedDatabases().total + "");
			severeDb.setValue(envAlert.getAuditedDatabases().severe + "");
			highDb.setValue(envAlert.getAuditedDatabases().high + "");
			mediumDb.setValue(envAlert.getAuditedDatabases().medium + "");
			lowDb.setValue(envAlert.getAuditedDatabases().low + "");
			logger.info("Database Url :", config.getNavigationLink());
			databaseTitle.setHref(config.getNavigationLink() + AUDITED_INSTANCE);
		}
	}

	@Override
	public String getEventName() {
		return String.format("%d:%s", config.getId(),
				EnvironmentAlertsWidgetComposer.class.getName());
	}

	@Override
	public String getDataURI() {
		String url = String.format("%s%s", config.getProduct().getRestUrl(),
				config.getDataURI());
		return url;
	}
}
