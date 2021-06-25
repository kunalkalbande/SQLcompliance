package com.idera.sqlcm.server.web;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;

import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.event.EventListener;

import com.idera.common.ui.components.CustomMessageBox;
import com.idera.i18n.I18NStrings;

import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.SuspendNotAllowedException;
import org.zkoss.zk.ui.UiException;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Messagebox;

import com.idera.cwf.model.Product;
import com.idera.cwf.ui.dialogs.CustomMessageBox.UserResponse;
import com.idera.sqlcm.ui.prompt.CustomMessageBoxDialog;
import com.idera.sqlcm.ui.prompt.CustomMessageBoxDialog.UserResponseSelection;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.ui.prompt.*;

import org.zkoss.zul.Window;

public class WebUtil extends com.idera.server.web.WebUtil {

	public static String buildPathRelativeToCurrentProduct(String path) {
		Product product = getCurrentProduct();
		if (product == null) {
			logger.error(SQLCMI18NStrings.ERR_CURRENT_PRODUCT_IS_NULL);
			return path;
		}
		String rootContext = product.getRootContext();
		String instanceName = product.getInstanceName();
		if (path.startsWith("/")) {
			return String.format("%s/%s%s", rootContext, instanceName, path);
		}
		return String.format("%s/%s/%s", rootContext, instanceName, path);
	}

	public static void showErrorBox(Throwable ex, String msgKey,
			Object... varargs) {
		showErrorBox(false, ex, msgKey, varargs);
	}
	
	public static void showErrorBox(String msgKey, Object... varargs) {
        showErrorBox(null, msgKey, varargs);
    }

	public static void showErrorBox(boolean suspendExecution, Throwable ex,
			String msgKey, Object... varargs) {

		Clients.clearBusy();
		String msg;
		if (varargs.length == 0) {
			msg = ELFunctions.getMessage(msgKey);
		} else {
			msg = ELFunctions.getMessageWithParams(msgKey, varargs);
		}
		List<String> exStrings = getLocalizedExceptionStrings(ex);
		exStrings.add(0, msg);
		String title = ELFunctions.getLabel(SQLCMI18NStrings.ERROR);
		showBox(suspendExecution, null, exStrings, title, "/images/error_icon.svg",
				Arrays.asList(UserResponse.OK), false);
	}

	public static void showInfoBox(String msgKey, Object... varargs) {
		Clients.clearBusy();
		String msg;
		if (varargs.length == 0) {
			msg = ELFunctions.getMessage(msgKey);
		} else {
			msg = ELFunctions.getMessageWithParams(msgKey, varargs);
		}
		String title = ELFunctions.getLabel(SQLCMI18NStrings.INFORMATION);
		showBox(false, null, Arrays.asList(msg), title, "~./sqlcm/images/info-icon-wht.png",
			Arrays.asList(UserResponse.OK), false);
	}

	public static void showWarningBox(String msgKey, Object... varargs) {
		Clients.clearBusy();
		String msg;
		if (varargs.length == 0) {
			msg = ELFunctions.getMessage(msgKey);
		} else {
			msg = ELFunctions.getMessageWithParams(msgKey, varargs);
		}
		String title = ELFunctions.getLabel(SQLCMI18NStrings.WARNING);
		showBox(false, null, Arrays.asList(msg), title, "/images/warning_white.svg",
			Arrays.asList(UserResponse.OK), false);
	}
	
	public static void showReportWarningBox(String msgKey, Object... varargs) {
		Clients.clearBusy();
		String title = ELFunctions.getLabel(SQLCMI18NStrings.WARNING);
		showBox(false, null, Arrays.asList(msgKey), title, "/images/warning_white.svg",
			Arrays.asList(UserResponse.OK), false);
	}

	public static void removeAllOnClickEventListeners(Component component, String eventName) {
		for (EventListener eventListener : component.getEventListeners(eventName)) {
			component.removeEventListener(eventName, eventListener);
		}
	}

	public static void showSuccessBoxThatStopsExecution(String msgKey, Object... varargs) {
		if (logger != null)
			logger.info(msgKey, varargs);
		try {
			showMessageBox(true, Arrays.asList(ELFunctions.getMessageWithParams(msgKey, varargs)),
					ELFunctions.getLabel(I18NStrings.SUCCESS), Messagebox.NONE, Arrays.asList(UserResponse.OK), false);
		} catch (UiException ex) {
			ex.printStackTrace();
		}
	}

	public static UserResponse showMessageBox(boolean suspendExecution, List<String> msgKey, String title, String iconURL,
											  List<UserResponse> userResponseList, boolean translateMessages) {
		return showBox(suspendExecution, null, msgKey, title, iconURL, userResponseList, translateMessages);
	}

	public static boolean showMessageBoxWithUserPreference(String message, String titleKey) {
		return UserResponse.YES.equals(showMessageBoxWithUserPreference(Arrays.asList(message), titleKey, "/images/confirm_question.svg")) ;
	}

	public static UserResponse showMessageBoxWithUserPreference(List<String> messages, String titleKey, String icon) {
		Clients.clearBusy();
		return showBox(true, null, messages, titleKey, icon, Arrays.asList(UserResponse.YES, UserResponse.NO), true);
	}



	public static UserResponse showConfirmationBoxWithIconAndCancel(List<String> messages, String titleKey, String icon) {
		return showBox(true, null, messages, titleKey, icon, Arrays.asList(UserResponse.YES, UserResponse.NO, UserResponse.CANCEL), true);
	}

	public static UserResponse showConfirmationBoxWithCancel(String message, String titleKey) {
		return showConfirmationBoxWithIconAndCancel(Arrays.asList(message), titleKey, Messagebox.NONE);
	}

	public static void showErrorBoxWithCustomMessage(String message) {
		showBox(false, message, getLocalizedExceptionStrings(null), ELFunctions.getLabel(I18NStrings.ERROR), "/images/error_icon.svg",
				Arrays.asList(UserResponse.OK), false);
	}

	public static void showErrorBoxWithCustomMessage(String message, String savedLabel) {
		showBox(false, message, getLocalizedExceptionStrings(null), ELFunctions.getLabel(savedLabel), "/images/error_icon.svg",
				Arrays.asList(UserResponse.OK), false);
	}

	public static void showInfoBoxWithCustomMessage2(String msg, String icon) {
		Clients.clearBusy();
		String title = ELFunctions.getLabel(SQLCMI18NStrings.INFORMATION);
		showBox2(false, null, Arrays.asList(msg), icon,title, "~./sqlcm/images/info-icon-wht.png",
				Arrays.asList(UserResponse.OK), false);
	}
	
	private static void showBox2(boolean b, Object object, List<String> asList,
			String icon, String title, String information,
			List<UserResponse> asList2, boolean c) {
		// TODO Auto-generated method stub
		
	}

	public static void showInfoBoxWithCustomMessage(String msg) {
		Clients.clearBusy();
		String title = ELFunctions.getLabel(SQLCMI18NStrings.INFORMATION);
		showBox(false, null, Arrays.asList(msg), title, "~./sqlcm/images/info-icon-wht.png",
				Arrays.asList(UserResponse.OK), false);
	} 
	
	public static void showErrorWithCustomMessage(String msg,String link) {
		Clients.clearBusy();
		String title = ELFunctions.getLabel(SQLCMI18NStrings.ERROR);
		showBox(true, null, Arrays.asList(msg),link, title, "/images/error_icon.svg",
				Arrays.asList(UserResponseSelection.OK), false);
	} 	
	
	public static void showErrorWithCustomMessage(String msg) {
		Clients.clearBusy();
		String title = ELFunctions.getLabel(SQLCMI18NStrings.ERROR);
		showBox(true, null, Arrays.asList(msg), title, "/images/error_icon.svg",
				Arrays.asList(UserResponse.OK), false);
	} 
	
	public static void showWarningWithCustomMessage(String msg) {
		Clients.clearBusy();
		String title = ELFunctions.getLabel(SQLCMI18NStrings.WARNING);
		showBox(true, null, Arrays.asList(msg), title, "/images/warning_white.svg",
				Arrays.asList(UserResponse.OK), false);
	} 
	
	public static UserResponse showConfirmationBoxWithIcon(String msg, String icon) {
		Clients.clearBusy();
		String title = ELFunctions.getLabel(SQLCMI18NStrings.INFORMATION);
		return showBox(false, null, Arrays.asList(msg),  icon,title, Arrays.asList(UserResponse.OK), true);
	}
	
	public static void showWarningBoxWithCustomMessage(String msg, String title) {
		Clients.clearBusy();
		showBox(true, null, Arrays.asList(msg), title, "/images/warning_white.svg",
				Arrays.asList(UserResponse.OK), false);
	}

	public static UserResponse showBox(boolean suspendExecution, String primaryFailureMessage, List<String> msgKey, String title,
									   String iconURL, List<UserResponse> userResponseList, boolean translateMessages) {
		Clients.clearBusy();
		final HashMap<String, Object> map = new HashMap<>();
		if (translateMessages) {
			List<String> messageList = new ArrayList<>();
			map.put(CustomMessageBox.STRING_TITLE, ELFunctions.getLabel(title));
			for (String tmp : msgKey) {
				messageList.add(ELFunctions.getLabel(tmp));
			}
			map.put(CustomMessageBox.MESSAGE_LIST, messageList);
		} else {
			map.put(CustomMessageBox.STRING_TITLE, title);
			map.put(CustomMessageBox.MESSAGE_LIST, msgKey);
		}
		map.put(CustomMessageBox.BUTTON_LIST, userResponseList);
		map.put(CustomMessageBox.ICON_URL, iconURL);
		map.put(CustomMessageBox.PRIMARY_ERROR_MESSAGE, primaryFailureMessage);

		Window window = (Window) Executions.createComponents(CustomMessageBox.URL, null, map);

		if (!suspendExecution) {
			window.doHighlighted();
			return UserResponse.OK;
		}

		try {
			window.doModal();
		} catch (SuspendNotAllowedException e) {
			logger.error(e, I18NStrings.COULD_NOT_SUSPEND_EXECUTION_TO_DISPLAY_ERROR_BOX);
		} catch (UiException e) {
			logger.error(e, I18NStrings.RECEIVED_INTERRUPT_WHILE_SUSPENDED_DISPLAYING_AN_ERROR);
		}

		return window.getAttribute(CustomMessageBox.USER_RESPONSE) == null || !(window
				.getAttribute(CustomMessageBox.USER_RESPONSE) instanceof UserResponse)
				? UserResponse.OK : (UserResponse) window.getAttribute(CustomMessageBox.USER_RESPONSE);
	}
	
	public static UserResponseSelection showBox(boolean suspendExecution,
			String primaryFailureMessage, List<String> msgKey,String hyperLinkKey,String title,
			String iconURL, List<UserResponseSelection> userResponseList,
			boolean translateMessages) {
		Clients.clearBusy();
		final HashMap<String, Object> map = new HashMap<>();
		if (translateMessages) {
			List<String> messageList = new ArrayList<>();
			map.put(CustomMessageBoxDialog.STRING_TITLE, ELFunctions.getLabel(title));
			for (String tmp : msgKey) {
				messageList.add(ELFunctions.getLabel(tmp));
			}
			map.put(CustomMessageBoxDialog.MESSAGE_LIST, messageList);
			map.put(CustomMessageBoxDialog.LINK_MESSAGE, ELFunctions.getLabel(hyperLinkKey));
		} else {
			map.put(CustomMessageBoxDialog.STRING_TITLE, title);
			map.put(CustomMessageBoxDialog.MESSAGE_LIST, msgKey);
			map.put(CustomMessageBoxDialog.LINK_MESSAGE, hyperLinkKey);
		}
		map.put(CustomMessageBoxDialog.BUTTON_LIST, userResponseList);
		map.put(CustomMessageBoxDialog.ICON_URL, iconURL);
		map.put(CustomMessageBoxDialog.PRIMARY_ERROR_MESSAGE, primaryFailureMessage);

		Window window = (Window) Executions.createComponents(
				CustomMessageBoxDialog.URL, null, map);

		if (!suspendExecution) {
			window.doHighlighted();
			return UserResponseSelection.OK;
		}

		try {
			window.doModal();
		} catch (SuspendNotAllowedException e) {
			logger.error(
					e,
					I18NStrings.COULD_NOT_SUSPEND_EXECUTION_TO_DISPLAY_ERROR_BOX);
		} catch (UiException e) {
			logger.error(
					e,
					I18NStrings.RECEIVED_INTERRUPT_WHILE_SUSPENDED_DISPLAYING_AN_ERROR);
		}

		return window.getAttribute(CustomMessageBoxDialog.USER_RESPONSE) == null
				|| !(window.getAttribute(CustomMessageBoxDialog.USER_RESPONSE) instanceof UserResponseSelection) ? UserResponseSelection.OK
				: (UserResponseSelection) window
						.getAttribute(CustomMessageBoxDialog.USER_RESPONSE);
	}
	
	//Regulation Guideline
	public static String RegulationGuidlineSaveDialog() {
		Clients.clearBusy();
		String title = ELFunctions.getLabel(SQLCMI18NStrings.ERROR);
		return showRegulationGuidelineSaveDialog(true, null, false);
	} 
	
	public static String showRegulationGuidelineSaveDialog(boolean suspendExecution,
			String primaryFailureMessage,
			boolean translateMessages) {
		Clients.clearBusy();
		final HashMap<String, Object> map = new HashMap<>();		

		Window window = (Window) Executions.createComponents(
				RegulationGuidelineDialog.URL, null, map);

		if (!suspendExecution) {
			window.doHighlighted();
			return null;
		}

		try {
			window.doModal();
		} catch (SuspendNotAllowedException e) {
			logger.error(
					e,
					I18NStrings.COULD_NOT_SUSPEND_EXECUTION_TO_DISPLAY_ERROR_BOX);
		} catch (UiException e) {
			logger.error(
					e,
					I18NStrings.RECEIVED_INTERRUPT_WHILE_SUSPENDED_DISPLAYING_AN_ERROR);
		}

		return window.getAttribute("userResponse") == null?null:(String)window.getAttribute("userResponse");
	}
}

