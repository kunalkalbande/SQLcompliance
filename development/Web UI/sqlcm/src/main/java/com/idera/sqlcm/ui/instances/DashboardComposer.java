package com.idera.sqlcm.ui.instances;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebConstants;

import org.apache.log4j.Logger;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zul.Include;
import org.zkoss.zul.Label;
import org.zkoss.zul.Timer;
import org.zkoss.zul.Window;

public class DashboardComposer extends SelectorComposer<Window> {

	private static final long serialVersionUID = 1L;
	public static final String ZUL_URL = "instances/maincontent.zul";
	private static final Logger logger = Logger.getLogger(DashboardComposer.class);

	private AnnotateDataBinder binder = null;

	@Wire
	private Include instanceList;
	@Wire
	Timer timer;
	@Wire
	Label timeLabel;

	@Override
	public void doAfterCompose(Window comp) throws Exception {

		super.doAfterCompose(comp);

		binder = new AnnotateDataBinder(comp);
		binder.bindBean("refreshInterval", WebConstants.REFRESH_INTERVAL);
		binder.loadAll();
	}

	private void refresh() {
		// Invalidate the lists to reload
		instanceList.invalidate();
		EventQueue<Event> q = EventQueues.lookup(
				WebConstants.INSTANCE_ALERTS_REFRESH_EVENT_QUEUE,
				EventQueues.DESKTOP, false);
		if (q != null) {
			logger.debug(WebConstants.INSTANCE_ALERTS_REFRESH_EVENT + " fired.");
			q.publish(new Event(WebConstants.INSTANCE_ALERTS_REFRESH_EVENT,
					null, null));
		}
		// Set refresh label
		timeLabel.setValue(String.format(
				ELFunctions.getMessage(SQLCMI18NStrings.UPDATED_AT),
				Utils.getFomatedCurrentDate()));
	}

	@Listen("onTimer=#timer")
	public void onTimer() {
		refresh();
	}
}
