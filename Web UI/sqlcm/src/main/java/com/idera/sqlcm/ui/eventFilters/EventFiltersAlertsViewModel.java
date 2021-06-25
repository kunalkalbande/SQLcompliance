package com.idera.sqlcm.ui.eventFilters;

import java.util.Comparator;

import org.apache.log4j.Logger;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zul.ListModelList;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMEntity.EntityType;
import com.idera.sqlcm.entities.CMEntity.RuleType;
import com.idera.sqlcm.entities.InstanceAlert;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.server.web.WebConstants;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.BaseViewModel;

public class EventFiltersAlertsViewModel extends BaseViewModel {

	private static final Logger logger = Logger.getLogger(EventFiltersAlertsViewModel.class);

	private ListModelList<InstanceAlert> entities;
	private RuleType alertType = RuleType.Event;

	@Init
	public void init(@BindingParam("entityType") String instanceType) {

		this.entityType = EntityType.valueOf(instanceType);
       EventListener<Event> listener = new EventListener<Event>() {
			@Override
			public void onEvent(Event arg0) throws Exception {
				logger.info("Instance refresh event called: " + arg0);
			
			}
		};
		EventQueue<Event> qInstance = EventQueues.lookup(
				WebConstants.DASHBOARD_INSTANCE_REFRESH_EVENT_QUEUE,
				EventQueues.DESKTOP, true);
		if (qInstance != null) {
			qInstance.subscribe(listener);
		}
		qInstance = EventQueues.lookup(
				WebConstants.INSTANCE_ALERTS_REFRESH_EVENT_QUEUE,
				EventQueues.DESKTOP, true);
		if (qInstance != null) {
			qInstance.subscribe(listener);
		}
	}

	@Override
	public ListModelList<InstanceAlert> getEntities() {
		return entities;
	}

	public String getEmptyMessage() {
		return ELFunctions.getLabel("SQLCM.Labels.no-data-available");
	}
	
	@Command
	public void reloadAlerts(@BindingParam("type") String type) {
		this.alertType = RuleType.valueOf(type);
		
	}

	@Command
	public void navigateInastanceDetails(@BindingParam("entity") InstanceAlert  entity) {
		if(entity != null) {
			String uri = "instanceView/" + entity.getInstanceId();
			uri = WebUtil.buildPathRelativeToCurrentProduct(uri);
			Executions.sendRedirect(uri);
		}
	}

	// Comparators
	private final Comparator<InstanceAlert> sortByAlertStatusAsc = new Comparator<InstanceAlert>() {

		@Override
		public int compare(InstanceAlert left, InstanceAlert right) {
			if (left != null && right != null && left.getAlertStatus() != null &&
					right.getAlertStatus() != null) {
				return left.getAlertStatus().compareTo(right.getAlertStatus());
			}
			return 0;
		}
	};
	private final Comparator<InstanceAlert> sortByAlertStatusDesc = new Comparator<InstanceAlert>() {

		@Override
		public int compare(InstanceAlert left, InstanceAlert right) {
			if (left != null && right != null && left.getAlertStatus() != null &&
					right.getAlertStatus() != null) {
				return right.getAlertStatus().compareTo(left.getAlertStatus());
			}
			return 0;
		}
	};
	private final Comparator<InstanceAlert> sortByAlertTypeAsc = new Comparator<InstanceAlert>() {

		@Override
		public int compare(InstanceAlert left, InstanceAlert right) {
			if (left != null && right != null && left.getAlertType() != null &&
					right.getAlertType() != null) {
				return left.getAlertType().compareTo(right.getAlertType());
			}
			return 0;
		}
	};
	private final Comparator<InstanceAlert> sortByAlertTypeDesc = new Comparator<InstanceAlert>() {

		@Override
		public int compare(InstanceAlert left, InstanceAlert right) {
			if (left != null && right != null && left.getAlertType() != null &&
					right.getAlertType() != null) {
				return right.getAlertType().compareTo(left.getAlertType());
			}
			return 0;
		}
	};
	private final Comparator<InstanceAlert> sortByInstanceNameAsc = new Comparator<InstanceAlert>() {

		@Override
		public int compare(InstanceAlert left, InstanceAlert right) {
			if (left != null && right != null) {
				return Utils.compareStrings(
						left.getInstanceName(), right.getInstanceName(), true);
			}
			return 0;
		}
	};

	private final Comparator<InstanceAlert> sortByInstanceNameDesc = new Comparator<InstanceAlert>() {

		@Override
		public int compare(InstanceAlert left, InstanceAlert right) {
			if (left != null && right != null) {
				return Utils.compareStrings(
						left.getInstanceName(), right.getInstanceName(), false);
			}
			return 0;
		}
	};
	private final Comparator<InstanceAlert> sortBySourceRuleAsc = new Comparator<InstanceAlert>() {

		@Override
		public int compare(InstanceAlert left, InstanceAlert right) {
			if (left != null && right != null) {
				return Utils.compareStrings(
						left.getSourceRule(), right.getSourceRule(), true);
			}
			return 0;
		}
	};
	private final Comparator<InstanceAlert> sortBySourceRuleDesc = new Comparator<InstanceAlert>() {

		@Override
		public int compare(InstanceAlert left, InstanceAlert right) {
			if (left != null && right != null) {
				return Utils.compareStrings(
						left.getSourceRule(), right.getSourceRule(), false);
			}
			return 0;
		}
	};
	private final Comparator<InstanceAlert> sortByTimeAsc = new Comparator<InstanceAlert>() {

		@Override
		public int compare(InstanceAlert left, InstanceAlert right) {
			if (left != null && right != null) {
				return Utils.compareDates(
						left.getTime(), right.getTime(), true);
			}
			return 0;
		}
	};
	private final Comparator<InstanceAlert> sortByTimeDesc = new Comparator<InstanceAlert>() {

		@Override
		public int compare(InstanceAlert left, InstanceAlert right) {
			if (left != null && right != null) {
				return Utils.compareDates(
						left.getTime(), right.getTime(), false);
			}
			return 0;
		}
	};

	public Comparator<InstanceAlert> getSortByAlertStatusAsc() {
		return sortByAlertStatusAsc;
	}

	public Comparator<InstanceAlert> getSortByAlertStatusDesc() {
		return sortByAlertStatusDesc;
	}

	public Comparator<InstanceAlert> getSortByAlertTypeAsc() {
		return sortByAlertTypeAsc;
	}

	public Comparator<InstanceAlert> getSortByAlertTypeDesc() {
		return sortByAlertTypeDesc;
	}

	public Comparator<InstanceAlert> getSortByInstanceNameAsc() {
		return sortByInstanceNameAsc;
	}

	public Comparator<InstanceAlert> getSortByInstanceNameDesc() {
		return sortByInstanceNameDesc;
	}

	public Comparator<InstanceAlert> getSortBySourceRuleAsc() {
		return sortBySourceRuleAsc;
	}

	public Comparator<InstanceAlert> getSortBySourceRuleDesc() {
		return sortBySourceRuleDesc;
	}

	public Comparator<InstanceAlert> getSortByTimeAsc() {
		return sortByTimeAsc;
	}

	public Comparator<InstanceAlert> getSortByTimeDesc() {
		return sortByTimeDesc;
	}

}
