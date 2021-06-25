package com.idera.sqlcm.ui;

import java.util.Comparator;

import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.InstanceStatus;

public abstract class InstanceStatusViewModel extends BaseViewModel {

	// Comparators
	protected final Comparator<InstanceStatus> sortByEnabledEventCategoryAsc = new Comparator<InstanceStatus>() {

		@Override
		public int compare(InstanceStatus left, InstanceStatus right) {
			if (left != null && right != null) {
				return left.getEventCategories() - right.getEventCategories();
			}
			return 0;
		}
	};
	protected final Comparator<InstanceStatus> sortByEnabledEventCategoryDesc = new Comparator<InstanceStatus>() {

		@Override
		public int compare(InstanceStatus left, InstanceStatus right) {
			if (left != null && right != null) {
				return right.getEventCategories() - left.getEventCategories();
			}
			return 0;
		}
	};
	protected final Comparator<InstanceStatus> sortByCollectedEventsAsc = new Comparator<InstanceStatus>() {

		@Override
		public int compare(InstanceStatus left, InstanceStatus right) {
			if (left != null && right != null) {
				return left.getCollectedEventCount() - right.getCollectedEventCount();
			}
			return 0;
		}
	};
	protected final Comparator<InstanceStatus> sortByCollectedEventsDesc = new Comparator<InstanceStatus>() {

		@Override
		public int compare(InstanceStatus left, InstanceStatus right) {
			if (left != null && right != null) {
				return right.getCollectedEventCount() - left.getCollectedEventCount();
			}
			return 0;
		}
	};
	protected final Comparator<InstanceStatus> sortByAgentStatusAsc = new Comparator<InstanceStatus>() {

		@Override
		public int compare(InstanceStatus left, InstanceStatus right) {
			if (left != null && right != null && left.getAgentStatus() != null &&
					right.getAgentStatus() != null) {
				return left.getAgentStatus().compareTo(right.getAgentStatus());
			}
			return 0;
		}
	};
	protected final Comparator<InstanceStatus> sortByAgentStatusDesc = new Comparator<InstanceStatus>() {

		@Override
		public int compare(InstanceStatus left, InstanceStatus right) {
			if (left != null && right != null && left.getAgentStatus() != null &&
					right.getAgentStatus() != null) {
				return right.getAgentStatus().compareTo(left.getAgentStatus());
			}
			return 0;
		}
	};
	protected final Comparator<InstanceStatus> sortByNoOfDatabaseAsc = new Comparator<InstanceStatus>() {

		@Override
		public int compare(InstanceStatus left, InstanceStatus right) {
			if (left != null && right != null) {
				return left.getDatabaseCount() - right.getDatabaseCount();
			}
			return 0;
		}
	};
	protected final Comparator<InstanceStatus> sortByNoOfDatabaseDesc = new Comparator<InstanceStatus>() {

		@Override
		public int compare(InstanceStatus left, InstanceStatus right) {
			if (left != null && right != null) {
				return right.getDatabaseCount() - left.getDatabaseCount();
			}
			return 0;
		}
	};
	protected final Comparator<InstanceStatus> sortByTimeAsc = new Comparator<InstanceStatus>() {

		@Override
		public int compare(InstanceStatus left, InstanceStatus right) {
			if (left != null && right != null) {
				return Utils.compareDates(
						left.getLastHeartbeat(), right.getLastHeartbeat(), true);
			}
			return 0;
		}
	};
	protected final Comparator<InstanceStatus> sortByTimeDesc = new Comparator<InstanceStatus>() {

		@Override
		public int compare(InstanceStatus left, InstanceStatus right) {
			if (left != null && right != null) {
				return Utils.compareDates(
						left.getLastHeartbeat(), right.getLastHeartbeat(), false);
			}
			return 0;
		}
	};

	public Comparator<InstanceStatus> getSortByEnabledEventCategoryAsc() {
		return sortByEnabledEventCategoryAsc;
	}

	public Comparator<InstanceStatus> getSortByEnabledEventCategoryDesc() {
		return sortByEnabledEventCategoryDesc;
	}

	public Comparator<InstanceStatus> getSortByCollectedEventsAsc() {
		return sortByCollectedEventsAsc;
	}

	public Comparator<InstanceStatus> getSortByCollectedEventsDesc() {
		return sortByCollectedEventsDesc;
	}

	public Comparator<InstanceStatus> getSortByAgentStatusAsc() {
		return sortByAgentStatusAsc;
	}

	public Comparator<InstanceStatus> getSortByAgentStatusDesc() {
		return sortByAgentStatusDesc;
	}

	public Comparator<InstanceStatus> getSortByNoOfDatabaseAsc() {
		return sortByNoOfDatabaseAsc;
	}

	public Comparator<InstanceStatus> getSortByNoOfDatabaseDesc() {
		return sortByNoOfDatabaseDesc;
	}

	public Comparator<InstanceStatus> getSortByTimeAsc() {
		return sortByTimeAsc;
	}

	public Comparator<InstanceStatus> getSortByTimeDesc() {
		return sortByTimeDesc;
	}
}
