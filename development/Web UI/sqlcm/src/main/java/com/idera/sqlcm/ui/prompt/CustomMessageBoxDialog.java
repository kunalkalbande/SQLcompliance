package com.idera.sqlcm.ui.prompt;

import org.zkoss.zk.ui.Execution;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zkplus.databind.BindingListModelList;
import org.zkoss.zul.*;

import java.util.ArrayList;
import java.util.List;

public class CustomMessageBoxDialog extends SelectorComposer<Window> {

    private static final long serialVersionUID = 1L;
    public static final String URL = "~./sqlcm/dialogs/customMessageBox.zul";
    public static final String STRING_TITLE = "title";
    public static final String MESSAGE_LIST = "messageList";
    public static final String BUTTON_LIST = "buttonList";
    public static final String USER_RESPONSE = "userResponse";
    public static final String ICON_URL = "iconURL";
    public static final String PRIMARY_ERROR_MESSAGE = "primaryErrorMessage";
    public static final String LINK_MESSAGE = "linkMessageList";
    String linkText;

    @Wire
    protected Label titleMessage;
    @Wire
    protected Label primaryCause;
    @Wire
    protected Grid messageGrid;
    @Wire
    protected Button okButton;
    @Wire
    protected Button yesButton;
    @Wire
    protected Button noButton;
    @Wire
    protected Button cancelButton;

    public static enum UserResponseSelection {
        YES, NO, CANCEL, OK
    }

    public ListModelList<String> messageModel = new BindingListModelList<String>(new ArrayList<String>(), false);
    
    
    protected AnnotateDataBinder binder;

    @SuppressWarnings("unchecked")
    @Override
    public void doAfterCompose(Window comp) throws Exception {
        super.doAfterCompose(comp);
        final Execution execution = Executions.getCurrent();
        /*if (execution.getArg().get(STRING_TITLE) != null && (execution.getArg().get(STRING_TITLE) instanceof String)) {
            this.titleMessage.setValue((String) execution.getArg().get(STRING_TITLE));
        }*/
        if (execution.getArg().get(MESSAGE_LIST) != null && (execution.getArg().get(MESSAGE_LIST) instanceof List)) {
            for (Object tmp : (List<Object>) execution.getArg().get(MESSAGE_LIST)) {
                if (tmp instanceof String) {
                    this.messageModel.add((String) tmp);
                }
            }
        }
        
        if (execution.getArg().get(BUTTON_LIST) != null && (execution.getArg().get(BUTTON_LIST) instanceof List)) {
            for (Object tmp : (List<Object>) execution.getArg().get(BUTTON_LIST)) {
                if (!(tmp instanceof UserResponseSelection)) {
                    continue;
                }
                UserResponseSelection userResponse = (UserResponseSelection) tmp;
                switch (userResponse) {
                    case YES:
                        this.yesButton.setVisible(true);
                        break;
                    case NO:
                        this.noButton.setVisible(true);
                        break;
                    case CANCEL:
                        this.cancelButton.setVisible(true);
                        break;
                    case OK:
                        this.okButton.setVisible(true);
                        break;
                }
            }
        }
        this.binder = new AnnotateDataBinder(comp);
        boolean isIconVisible = false;
        if (execution.getArg().get(ICON_URL) != null && execution.getArg().get(ICON_URL) instanceof String) {
            this.binder.bindBean("iconURL", execution.getArg().get(ICON_URL));
            isIconVisible = true;
        }
        
        if (execution.getArg().get(STRING_TITLE) != null && (execution.getArg().get(STRING_TITLE) instanceof String)) {
            this.binder.bindBean("titleText", execution.getArg().get(STRING_TITLE));
        }       

        if (execution.getArg().get(LINK_MESSAGE) != null && (execution.getArg().get(LINK_MESSAGE) instanceof String)) {
            this.binder.bindBean("linkMessage", execution.getArg().get(LINK_MESSAGE));
            linkText = execution.getArg().get(LINK_MESSAGE).toString();
        }          
        
        this.binder.bindBean("isIconVisible", isIconVisible);
        if (execution.getArg().get(PRIMARY_ERROR_MESSAGE) != null && execution.getArg().get(PRIMARY_ERROR_MESSAGE) instanceof String) {
            this.primaryCause.setValue((String) execution.getArg().get(PRIMARY_ERROR_MESSAGE));
        }
        this.binder.bindBean("messageListModel", this.messageModel);
        this.binder.loadAll();
    }

    @Listen("onClick = #yesButton")
    public void onClickYesButton(Event evt) {
        this.getSelf().setAttribute(USER_RESPONSE, UserResponseSelection.YES);
        this.getSelf().detach();
    }

    @Listen("onClick = #okButton")
    public void onClickOkButton(Event evt) {
        this.getSelf().setAttribute(USER_RESPONSE, UserResponseSelection.OK);
        this.getSelf().detach();
    }

    @Listen("onClick = #cancelButton")
    public void onClickCancelButton(Event evt) {
        this.getSelf().setAttribute(USER_RESPONSE, UserResponseSelection.CANCEL);
        this.getSelf().detach();
    }

    @Listen("onClick = #noButton")
    public void onClickNoButton(Event evt) {
        this.getSelf().setAttribute(USER_RESPONSE, UserResponseSelection.NO);
        this.getSelf().detach();
    }
    
    @Listen("onClick = #hyperLink")
    public void onClickhyperLink(Event evt) {
    	Executions.getCurrent().sendRedirect(linkText, "_blank");
    }
}
