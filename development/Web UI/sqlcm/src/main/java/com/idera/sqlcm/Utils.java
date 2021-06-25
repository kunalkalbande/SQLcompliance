package com.idera.sqlcm;

import java.io.File;
import java.io.IOException;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.*;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.idera.GlobalConstants;
import com.idera.server.web.session.SessionUtil;
import com.idera.sqlcm.server.web.WebConstants;

import org.apache.log4j.Logger;
import org.zkoss.util.TimeZones;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zul.AbstractTreeModel;
import org.zkoss.zul.ListModelList;

/**
 * Utility class
 * 
 * @author
 */
public class Utils {

	private static final Logger log = Logger.getLogger(Utils.class);

	/**
	 * Constructor prevent initialization.
	 */
	private Utils() {

	}

	private static Calendar getCalendar() {
		TimeZone zone = getClientTimeZone();
		Calendar cal = Calendar.getInstance(zone,
				SessionUtil.getSelectedLocale());
		return cal;
	}

	public static TimeZone getClientTimeZone() {
		Integer ofsmins = (Integer) Sessions.getCurrent().getAttribute(
			GlobalConstants.IDERA_WEB_CONSOLE_TZ_OFFSET);
		ofsmins = ofsmins / 60 / 1000;
		return TimeZones.getTimeZone(ofsmins);
	}

	public static Date parseDate(String date, String format) {
		try {
			SimpleDateFormat sdf = new SimpleDateFormat(format);
			return sdf.parse(date);
		} catch (Exception e) {
			log.error(e);
		}
		return null;
	}

	/**
	 * Method to parse date into default format <br/>
	 * <b>MMM dd yyyy HH:mm</b>
	 * @param date
	 * @return
	 */
	public static Date parseDate(String date) {
		return parseDate(date, GlobalConstants.DATE_FORMAT);
	}
	
	public static Date parseDateOfUnknownFormat(String date) throws Exception {
	
		    if (date != null && !date.isEmpty())
		    {
		        SimpleDateFormat[] formats =
		                new SimpleDateFormat[] {new SimpleDateFormat("yyyy-M-d"),new SimpleDateFormat("MM-dd-yyyy"), new SimpleDateFormat("yyyyMMdd"),
		                        new SimpleDateFormat("MM/dd/yyyy")};
		        Date parsedDate = null;
		        for (int i = 0; i < formats.length; i++)
		        {
		            try
		            {
		                parsedDate = formats[i].parse(date);
		                return parsedDate;
		            }
		            catch (ParseException e)
		            {
		                continue;
		            }
		        }
		    }
		    throw new Exception("Unknown date format: '" + date + "'");
	
	}

	public static String getFormatedDate(Date date) {
		return getFomatedDate(date, WebConstants.DATE_FORMAT);
	}

	public static String getFomatedShortDate(Date date) {
		return getFomatedDate(date, WebConstants.SHORT_DATE_FORMAT);
	}

	public static String getFomatedDate(Date date, String format) {
		if (date != null) {
			SimpleDateFormat sdf = new SimpleDateFormat(
					format);
			return sdf.format(date);
		}
		return null;
	}

	public static String getFomatedCurrentDate() {
		return getFormatedDate(getCalendar().getTime());
	}

	public static String getFomatedTime(Date date) {
		return getFomatedDate(date, WebConstants.TIME_FORMAT);
	}

	public static String getFormattedShortTime(Date date) {
		return getFomatedDate(date, WebConstants.SHORT_TIME_FORMAT);
	}

	public static String getFormattedDuration(Date date) {
		return getFomatedDate(date, WebConstants.DURATION_FORMAT);
	}

	/**
	 * Method to compare strings
	 * @param left
	 * @param right
	 * @param isAsc
	 * @return (-n) less, (0) equals, (+n) greater
	 */
	public static int compareStrings(String left, String right, boolean isAsc) {
		int res = 0;
		if (left != null && right != null) {
			res = left.compareTo(right);
		}
		return (res * (isAsc ? 1 : -1));
	}

	/**
	 * Method to compare string dates
	 * @param left
	 * @param right
	 * @param isAsc
	 * @return (-n) less, (0) equals, (+n) greater
	 */
	public static int compareDates(String left, String right, boolean isAsc) {
		return compareDates(parseDate(left), parseDate(right), isAsc);
	}

	/**
	 * Method to compare dates
	 * @param left
	 * @param right
	 * @param isAsc
	 * @return (-n) less, (0) equals, (+n) greater
	 */
	public static int compareDates(Date left, Date right, boolean isAsc) {
		int res = 0;
		if (left != null && right != null) {
			res = left.compareTo(right);
		}
		return (res * (isAsc ? 1 : -1));
	}

    public static long parseInstanceIdArg() throws NumberFormatException {
        String instIdArg = Executions.getCurrent().getParameter(WebConstants.PRM_INSTANCE_ID);
        try {
            return Long.parseLong(instIdArg);
        } catch (NumberFormatException e) {
            log.info(" Unparsable instance id " + instIdArg);
            throw e;
        }
    }

	public static long parseDatabaseIdArg() throws NumberFormatException {
		String databaseIdArg = Executions.getCurrent().getParameter(WebConstants.PRM_DATABASE_ID);
		try {
			return Long.parseLong(databaseIdArg);
		} catch (NumberFormatException e) {
			log.info(" Unparsable database id " + databaseIdArg);
			throw e;
		}
	}

    /**
     * Helper method to get single selected item from zkoss {@link org.zkoss.zul.ListModelList}
     *
     * THIS METHOD WILL NOT WORK FOR MULTI SELECTION MODE ! Single item will be returned randomly.
     *
     * @param listModelList
     * @param <T>
     * @return selected item of <T> type or null if list does not have selected items
     */
    public static <T> T getSingleSelectedItem(ListModelList<T> listModelList) {
        if (listModelList == null) {
            return null;
        }
        Set<T> selectedItemsSet = listModelList.getSelection();
        if (selectedItemsSet != null && !selectedItemsSet.isEmpty()) {
            for (T item : selectedItemsSet) {
                return item;
            }
        }
        return null;
    }

    /**
     * Helper method to get single selected item from zkoss {@link org.zkoss.zul.AbstractTreeModel}
     *
     * THIS METHOD WILL NOT WORK FOR MULTI SELECTION MODE ! Single item will be returned randomly.
     *
     * @param treeModel
     * @param <T>
     * @return selected item of <T> type or null if list does not have selected items
     */
    public static <T> T getSingleSelectedItem(AbstractTreeModel<T> treeModel) {
        if (treeModel == null) {
            return null;
        }
        Set<T> selectedItemsSet = treeModel.getSelection();
        if (selectedItemsSet != null && !selectedItemsSet.isEmpty()) {
            for (T item : selectedItemsSet) {
                return item;
            }
        }
        return null;
    }

    public static <T> ListModelList<T> createZkModelListFromList(List<T> list) {
        if (list == null) {
            return null;
        }
        return new ListModelList<T>(list);
    }

    @SuppressWarnings({ "unchecked", "rawtypes" })
	public static void removeAllSelectedItems(final ListModelList<?> listModelList) {
        if (listModelList == null || listModelList.size() == 0 || listModelList.getSelection().size() == 0) {
            return;
        }
        // copied selected to new array to avoid ConcurrentModificationException
        listModelList.removeAll(new ArrayList(listModelList.getSelection()));
    }

	public static String objectToJson(Object o) {
		try {
			return new com.fasterxml.jackson.databind.ObjectMapper().writer().writeValueAsString(o);
		} catch (JsonProcessingException e) {
			new RuntimeException(e);
		}
		return "";
	}

	public static <T> T getMockDataFromFile(String filePath, TypeReference<T> responseObjectTypeReference) {
		Objects.requireNonNull(filePath, " filePath must not be null! ");
		Objects.requireNonNull(responseObjectTypeReference, " responseObjectTypeReference must not be null! ");
		try {
			T response = new ObjectMapper().readValue(new File(filePath), responseObjectTypeReference);
			return response;
		} catch(IOException e) {
			throw new RuntimeException(e);
		}
	}
}
