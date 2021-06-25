package com.idera.sqlcm.ui.basepage;

import java.io.File;

import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.A;
import org.zkoss.zul.Div;
import org.zkoss.zul.Vlayout;

import com.idera.sqlcm.server.web.WebUtil;

public class NavigationBarComposer extends SelectorComposer<Vlayout> {

	private static final long serialVersionUID = 1L;

	@Wire
	private A homePageLink;

	@Wire
	private Div homePageNavDiv;

	@Wire
	private A auditedInstancesLink;

	@Wire
	private Div auditedInstancesNavDiv;

	@Wire
	private A instancesAlertslink;
	
	@Wire
	private Div instancesAlertsNavDiv;

	@Wire
	private A instancesAlertsLink;
	
		@Wire
	private A eventFiltersLink;
	
	@Wire
	private Div eventFiltersNavDiv;
	/*SQLCM Req4.1.1.6 New Event Filters View - End*/
		
		
	@Wire
	private A logsViewLink;
	
	@Wire
	private Div logsViewNavDiv;
	
	//4.1.1.7 New Logs View.
	
	/*@Wire
	private Div auditedDatabasesNavDiv;*/
	@Wire
	private A auditReportLink;

	@Wire
	private Div auditReportNavDiv;

	@Wire
	private A administrationLink;

	@Wire
	private Div administrationNavDiv;

	private void buildNavigationLink() {
		homePageLink.setHref(WebUtil
				.buildPathRelativeToCurrentProduct("index"));
		auditedInstancesLink.setHref(WebUtil
				.buildPathRelativeToCurrentProduct("auditedInstance"));

		logsViewLink.setHref(WebUtil
				.buildPathRelativeToCurrentProduct("instancesAlerts"));
		
		eventFiltersLink.setHref(WebUtil
				.buildPathRelativeToCurrentProduct("eventFiltersView"));

		instancesAlertsLink.setHref(WebUtil
				.buildPathRelativeToCurrentProduct("instancesAlerts"));
		
		if(Sessions.getCurrent().getAttribute("alert_type")!=null){
			String temp=(String)Sessions.getCurrent().getAttribute("alert_type");
			instancesAlertsLink.setHref(WebUtil
					.buildPathRelativeToCurrentProduct(temp));
		}
		else{
			instancesAlertsLink.setHref(WebUtil
				.buildPathRelativeToCurrentProduct("instancesAlerts"));
		}
		
		if(Sessions.getCurrent().getAttribute("log_type")!=null){
			String temp=(String)Sessions.getCurrent().getAttribute("log_type");
			logsViewLink.setHref(WebUtil
					.buildPathRelativeToCurrentProduct(temp));
		}
		else{
		logsViewLink.setHref(WebUtil
				.buildPathRelativeToCurrentProduct("activityLogsView"));
		}
		
		auditReportLink.setHref(WebUtil
				.buildPathRelativeToCurrentProduct("auditReport"));
				
		administrationLink.setHref(WebUtil
				.buildPathRelativeToCurrentProduct("administration"));
	}

	@Override
	public org.zkoss.zk.ui.metainfo.ComponentInfo doBeforeCompose(
			org.zkoss.zk.ui.Page page, org.zkoss.zk.ui.Component parent,
			org.zkoss.zk.ui.metainfo.ComponentInfo compInfo) {

		return compInfo;
	};

	@Override
	public void doAfterCompose(Vlayout component) throws Exception {
		super.doAfterCompose(component);

		buildNavigationLink();
		setNavElementStyle(homePageLink, homePageNavDiv);
		setNavElementStyle(auditedInstancesLink, auditedInstancesNavDiv);

		
		/*SQLCM Req4.1.1.6 New Event Filters View - Start*/
		setNavElementStyle(eventFiltersLink, eventFiltersNavDiv);
		/*SQLCM Req4.1.1.6 New Event Filters View - End*/
		

		//4.1.1.5 Enhanced Alerts View
	    setNavElementStyle(instancesAlertsLink, instancesAlertsNavDiv);
				//4.1.1.5 Enhanced Alerts View

		//4.1.1.7 New Logs View.
		setNavElementStyle(logsViewLink, logsViewNavDiv);
		//4.1.1.7 New Logs View.
		setNavElementStyle(auditReportLink, auditReportNavDiv);
		setNavElementStyle(administrationLink, administrationNavDiv);
		if(Sessions.getCurrent().getAttribute("ViewName")!=null)
		Sessions.getCurrent().removeAttribute("ViewName");
	}

	private void setNavElementStyle(A navLink, Div navDiv) {

		if (isCurrentPage(navLink.getHref())) {
			navDiv.setSclass("ccl-padding-t-16 ccl-padding-lr-24 ccl-nav-bar-element ccl-nav-bar-height ccl-nav-bar-element-current-page");
		} else {
			navDiv.setSclass("ccl-padding-t-16 ccl-padding-lr-24 ccl-nav-bar-element ccl-nav-bar-height ccl-nav-bar-element-other-page");
		}
	}

	private boolean isCurrentPage(String pageNameWithExtension) {
		// Check for valid page name
		if ((pageNameWithExtension != null)
				&& (!pageNameWithExtension.isEmpty())) {
			String requestPath = Executions.getCurrent().getDesktop()
					.getRequestPath();
			requestPath = new File(requestPath).getName();
			requestPath = removeExtension(requestPath);
			// the request path can be retrieved
			if ((requestPath != null) && (!requestPath.isEmpty())) {
				// get the page name without leading slash
				String pageName = new File(pageNameWithExtension).getName();
				pageName = removeExtension(pageName);
				// the passed page name is equals the current page name
				if (requestPath.equalsIgnoreCase(pageName)) {
					return true;
				}
			}
		}
		return false;
	}

	private String removeExtension(String page) {
		int lastPeriodPos = page.lastIndexOf('.');
		if (lastPeriodPos < 0)
			return page;
		return page.substring(0, lastPeriodPos);
	}

}
