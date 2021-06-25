package com.idera.sqlcm.wizard;

import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.select.Selectors;

public abstract class StepBase<WVM extends AbstractWizardViewModel, WSM extends IWizardEntity>
        implements IStep<WVM, WSM> {

    private WVM mParentWizardViewModel;

    private Component mRootStepComponent;

    public StepBase() {
    }

    public WVM getParentWizardViewModel() {
        return mParentWizardViewModel;
    }

    @Override
    public final void init(WVM parentWizardViewModel, Component rootStepComponent) {
        if (parentWizardViewModel == null) {
            throw new IllegalArgumentException(" Parent parentWizardViewModel must not be null ");
        }
        mParentWizardViewModel = parentWizardViewModel;

        mRootStepComponent = rootStepComponent;
        Selectors.wireComponents(rootStepComponent, this, false);
        onAfterWire();
    }

    public void onAfterWire() {

    }

    @Override
    public Component getRootStepComponent() {
        return mRootStepComponent;
    }

    @Override
    public void setVisible(boolean visible) {
        mRootStepComponent.setVisible(visible);
    }

    @Override
    public boolean isFirst() {
        return false;
    }

    @Override
    public boolean isLast() {
        return getNextStepZul() == null;
    }

    @Override
    public String getTips() {
        return " @ # Tips # @ ";
    }

    @Override
    public void onBeforePrev(WSM wizardSaveEntity) {
        // do nothing
    }

    @Override
    public void onBeforeNext(WSM wizardSaveEntity) {
        // do nothing
    }

    @Override
    public void onCancel(WSM wizardSaveEntity) {
        // do nothing
    }

    @Override
    public void onFinish(WSM wizardSaveEntity) {
        // do nothing
    }

    @Override
    public void onShow(WSM wizardSaveEntity) {
        // do nothing
    }
}
