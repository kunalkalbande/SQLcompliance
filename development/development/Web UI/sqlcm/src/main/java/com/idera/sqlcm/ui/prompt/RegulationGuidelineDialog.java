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

public class RegulationGuidelineDialog extends SelectorComposer<Window> {

    private static final long serialVersionUID = 1L;
    public static final String URL = "~./sqlcm/dialogs/saveRegulationGuideLineDialog.zul";
    public static final String USER_RESPONSE = "userResponse";
    public static final String PRIMARY_ERROR_MESSAGE = "primaryErrorMessage";
    public static final String LINK_MESSAGE = "linkMessageList";
    String linkText;

    @Wire
    protected Radio applyRegulationRadio;
    @Wire
    protected Radio applyCustomRegulationRadio;
    @Wire
    protected Button okButton;
    @Wire
    protected Button cancelButton;
    @Wire
    protected Textbox txtBox;
    
    
    protected AnnotateDataBinder binder;

    @SuppressWarnings("unchecked")
    @Override
    public void doAfterCompose(Window comp) throws Exception {
        super.doAfterCompose(comp);
        final Execution execution = Executions.getCurrent();
        this.binder = new AnnotateDataBinder(comp);
        this.binder.loadAll();
    }

    @Listen("onClick = #okButton")
    public void onClickOkButton(Event evt) {
		if (applyRegulationRadio.isChecked()
				|| !txtBox.getText().trim().equals("")) {
			if (applyRegulationRadio.isChecked()) {
				this.getSelf().setAttribute(USER_RESPONSE, "");
			} else {
				this.getSelf().setAttribute(USER_RESPONSE,
						txtBox.getText().trim());
			}
			this.getSelf().detach();
		}
    }

    @Listen("onClick = #cancelButton")
    public void onClickCancelButton(Event evt) {
        this.getSelf().detach();
    }
    
    @Listen("onCheck = #applyCustomRegulationRadio")
    public void onCheckApplyRadioCustom(Event evt) {
    	if(applyCustomRegulationRadio.isChecked()){
    		txtBox.setDisabled(false);
    	}
    	else{
    		txtBox.setDisabled(true);
    	}
    }
    
    @Listen("onCheck = #applyRegulationRadio")
    public void onCheckApplyRadio(Event evt) {
    	if(applyRegulationRadio.isChecked()){
    		txtBox.setDisabled(true);
    	}
    	else{
    		txtBox.setDisabled(false);
    	}
    }
}
