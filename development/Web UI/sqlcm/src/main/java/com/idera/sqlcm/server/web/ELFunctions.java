package com.idera.sqlcm.server.web;

import com.idera.i18n.I18NUtil;
import com.idera.server.web.session.SessionUtil;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.ui.dialogs.EventPropertiesLabelMapper;
import com.idera.sqlcm.utils.SQLCMConstants;
import org.apache.commons.lang.StringUtils;

import java.text.DateFormat;
import java.util.Collection;
import java.util.Date;
import java.util.Locale;

public class ELFunctions {

    private static final String DEFAULT_DELIMITER = ", ";
    public static final String N_A = getLabel(SQLCMI18NStrings.N_A);
    public static final String NONE = getLabel(SQLCMI18NStrings.NONE);

    public enum IconSize {

		SMALL("small", "16x16"),
		MEDIUM("medium", "24x24"),
		LARGE("large", "32x32"),
		XL("xl", "48x48");

		private String stringValue;
		private String pixelValue;

		IconSize(String stringValue, String pixelValue) {
			this.stringValue = stringValue;
			this.pixelValue = pixelValue;
		}

		public String getStringValue() {
			return stringValue;
		}

		public String getPixelValue() {
			return pixelValue;
		}

		static IconSize fromString(String string) {

			for( IconSize size : IconSize.values() ) {
				if( size.stringValue.equals(string) ) return size;
			}

			return null;
		}
	}

	public static String getImageURL(String iconType, String iconSize) {

		IconSize size = IconSize.fromString(iconSize);

		if( size == null ) throw new IllegalArgumentException("Invalid icon size, must be: small, medium, or large");

		StringBuilder builder = new StringBuilder(SQLCMConstants.BASE_IMAGE_URL);
		builder.append("/").append(iconType).append("-");
		builder.append(size.getPixelValue()).append(SQLCMConstants.DEFAULT_IMAGE_EXT);

		return builder.toString();
	}

	public static String getImageURLWithoutSize(String iconType) {
		StringBuilder builder = new StringBuilder(SQLCMConstants.BASE_IMAGE_URL);
		builder.append("/").append(iconType).append(SQLCMConstants.DEFAULT_IMAGE_EXT);
		return builder.toString();
	}

	public static String getLabel(String labelKey) {
		return getLabel(SessionUtil.getSelectedLocale(), labelKey);
	}

	public static String getUpperCaseLabel(String labelKey) {
		return getUpperCaseLabel(SessionUtil.getSelectedLocale(), labelKey);
	}

    public static String getLowerCaseLabel(String labelKey) {
        return getLowerCaseLabel(SessionUtil.getSelectedLocale(), labelKey);
    }

	public static String getMessage(String msgKey) {
		return getMessage(SessionUtil.getSelectedLocale(), msgKey);
	}

	public static String getUpperCaseMessage(String msgKey) {
		return getMessage(SessionUtil.getSelectedLocale(), msgKey).toUpperCase(SessionUtil.getSelectedLocale());
	}

	public static String getUpperCaseMessageWithParams(String msgKey, Object... varargs) {
		return getMessageWithParams(SessionUtil.getSelectedLocale(), msgKey, varargs).toUpperCase(SessionUtil.getSelectedLocale());
	}

	public static String getLabel(Locale locale, String labelKey) {
		return I18NUtil.getLocalizedMessage(locale, labelKey);
	}

    public static String getLabelWithParams(Locale locale, String labelKey, Object... varargs) {
        return I18NUtil.getLocalizedMessage(locale, labelKey, varargs);
    }

    public static String getLabelWithParams(String labelKey, Object... varargs) {
        return getLabelWithParams(SessionUtil.getSelectedLocale(), labelKey, varargs);
    }

	public static String getUpperCaseLabel(Locale locale, String labelKey) {
		return I18NUtil.getLocalizedMessage(locale, labelKey).toUpperCase(locale);
	}

    public static String getLowerCaseLabel(Locale locale, String labelKey) {
        return I18NUtil.getLocalizedMessage(locale, labelKey).toLowerCase(locale);
    }

	public static String getMessage(Locale locale, String msgKey) {
		return I18NUtil.getLocalizedMessage(locale, msgKey);
	}

	public static String getMessageWithParams(String msgKey, Object... varargs) {
		return I18NUtil.getLocalizedMessage(SessionUtil.getSelectedLocale(), msgKey, varargs);
	}

	public static String getMessageWithParams(Locale locale, String msgKey, Object... varargs) {
		return I18NUtil.getLocalizedMessage(locale, msgKey, varargs);
	}

	public static String browserVersionContentString()	{
		return "IE=9";
	}

	public static String getShortDateTime(Date date) {
		try {
			if (date == null) {
				return "";
			}
			return DateFormat.getDateInstance(DateFormat.SHORT,  Locale.getDefault()).format(date);

		} catch(Exception e) {
            //TODO Handle this
			throw e;
		}
	}

	public static String getDefaultStringIfNull(String str) {
		if (str == null || str.length() == 0) {
			return N_A;
		}
		return str;
	}

	public static String buildPathRelativeToCurrentProduct(String str)	{
		return WebUtil.buildPathRelativeToCurrentProduct(str);
	}

    public static String listToString(Collection<String> listToJoin, String delimiter) {
        if (listToJoin == null || listToJoin.size() == 0) {
            return NONE;
        }
        return StringUtils.join(listToJoin, (delimiter == null) ? DEFAULT_DELIMITER : delimiter);
    }

    public static String listToString(Collection<String> listToJoin) {
        return listToString(listToJoin, DEFAULT_DELIMITER);
    }

	public static String getPropertyLabel(String propertyName) {
		return EventPropertiesLabelMapper.getInstance().getLabelByPropertyName(propertyName);
	}

}