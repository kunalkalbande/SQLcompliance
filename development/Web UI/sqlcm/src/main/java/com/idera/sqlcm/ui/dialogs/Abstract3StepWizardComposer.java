package com.idera.sqlcm.ui.dialogs;

import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Div;
import org.zkoss.zul.Vlayout;
import org.zkoss.zul.Window;

public abstract class Abstract3StepWizardComposer extends AbstractWizardComposer {

    static final long serialVersionUID = 1L;

    @Wire
    protected Vlayout step1Item;
    @Wire
    protected Vlayout step2Item;
    @Wire
    protected Vlayout step3Item;

    @Wire
    protected Div step1Tips;
    @Wire
    protected Div step2Tips;
    @Wire
    protected Div step3Tips;

    @Wire
    protected Div step1Content;
    @Wire
    protected Div step2Content;
    @Wire
    protected Div step3Content;

    @Wire
    protected Button saveButton;

    @Override
    public void doAfterCompose(Window comp) throws Exception {
        super.doAfterCompose(comp);
        this.activate(new StepComponents(this.step1Tips, this.step1Content, this.step1Item));
    }

    @Listen("onClick = a#step1Link")
    public void activateStep1() {
        this.activate(new StepComponents(this.step1Tips, this.step1Content, this.step1Item));
    }

    @Listen("onClick = a#step2Link")
    public void activateStep2() {
        this.activate(new StepComponents(this.step2Tips, this.step2Content, this.step2Item));
    }

    @Listen("onClick = a#step3Link")
    public void activateStep3() {
        this.activate(new StepComponents(this.step3Tips, this.step3Content, this.step3Item));
        this.updateSummaryValues();
    }

    @Override
    protected StepComponents getNextStep() {

        Div activeDiv = this.getActiveMainContentDiv();

        if (this.step1Content.equals(activeDiv)) {
            this.validatesTEP1();
            return new StepComponents(this.step2Tips, this.step2Content, this.step2Item);
        }
        else if (this.step2Content.equals(activeDiv)) {
            this.validatesTEP2();
            return new StepComponents(this.step3Tips, this.step3Content, this.step3Item);
        }

        return null;
    }

    @Override
    protected StepComponents getPrevStep() {

        Div activeDiv = this.getActiveMainContentDiv();

        if (this.step2Content.equals(activeDiv)) {
            return new StepComponents(this.step1Tips, this.step1Content, this.step1Item);
        }
        else if (this.step3Content.equals(activeDiv)) {
            return new StepComponents(this.step2Tips, this.step2Content, this.step2Item);
        }

        return null;
    }

    @Override
    protected StepComponents getFirstStep() {
        return new StepComponents(this.step1Tips, this.step1Content, this.step1Item);
    }

    @Override
    protected StepComponents getLastStep() {
        return new StepComponents(this.step3Tips, this.step3Content, this.step3Item);
    }

    public abstract void validate();

    @Override
    @Listen("onDoSave = window")
    public abstract void onDoSave(Event event);

    protected abstract void updateSummaryValues();

    public abstract void validatesTEP1();

    public abstract void validatesTEP2();

}
