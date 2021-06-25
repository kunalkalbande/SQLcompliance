package com.idera.sqlcm.ui;

import java.util.Comparator;
import java.util.List;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMEntity.EntityType;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebConstants;

public abstract class BaseViewModel {

    // TODO IR move to package created by KM
	public enum Interval {
		ALL(0, "All"),
		ONE_DAY(1, ELFunctions.getLabel(SQLCMI18NStrings.ONE_DAY)),
		SEVEN_DAY(7, ELFunctions.getLabel(SQLCMI18NStrings.SEVEN_DAYS)),
		THIRTY_DAY(30, ELFunctions.getLabel(SQLCMI18NStrings.THIRTY_DAY));

		private String label;
		private int days;

		private Interval(int days, String label) {
			this.label = label;
			this.days = days;

		}

		public String getLabel() {
            return label;
		}
        
        public String getName() {
            return this.name();
        }

		public int getDays() {
			return days;
		}
	}

    // TODO IR move to package created by KM
    public enum Category {
        OVERALL_ACTIVITY(21, ELFunctions.getLabel(SQLCMI18NStrings.OVERALL_ACTIVITY)),//TODO AS ask .NET team id
        EVENT_ALERT(4, ELFunctions.getLabel(SQLCMI18NStrings.EVENT_ALERTS)),
        FAILED_LOGIN(6, ELFunctions.getLabel(SQLCMI18NStrings.FAILED_LOGIN)),
        SECURITY(10, ELFunctions.getLabel(SQLCMI18NStrings.SECURITY)),
        DDL(9, ELFunctions.getLabel(SQLCMI18NStrings.DDL)),
        PRIVILEGED_USER(5, ELFunctions.getLabel(SQLCMI18NStrings.PRIVILEGED_USER));

        private String label;
        private int index;

        private Category(int index, String label) {
            this.label = label;
            this.index = index;

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
    }

	protected int pageSize = WebConstants.DEFAULT_PAGE_SIZE;

	protected EntityType entityType;

	public final int getPageSize() {
		return pageSize;
	}

	public final void setPageSize(int value) {
		this.pageSize = value;
	}

	public abstract List<? extends CMEntity> getEntities();

	// Comparators-----------------
	protected final Comparator<CMEntity> sortByNameAsc = new Comparator<CMEntity>() {

		@Override
		public int compare(CMEntity left, CMEntity right) {
			if (left != null && right != null) {
				return Utils.compareStrings(
						left.getName(), right.getName(), true);
			}
			return 0;
		}
	};
	protected final Comparator<CMEntity> sortByNameDesc = new Comparator<CMEntity>() {

		@Override
		public int compare(CMEntity left, CMEntity right) {
			if (left != null && right != null) {
				return Utils.compareStrings(
						left.getName(), right.getName(), false);
			}
			return 0;
		}
	};

	public final Comparator<CMEntity> getSortByNameAsc() {
		return sortByNameAsc;
	}

	public final Comparator<CMEntity> getSortByNameDesc() {
		return sortByNameDesc;
	}
}
