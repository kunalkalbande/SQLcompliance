/***Start SQLCm 5.4***/
/*Requirement 4.1.4.1*/

package com.idera.sqlcm.wizard;

import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.select.Selectors;

import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;
import com.idera.sqlcm.ui.importAuditSetting.AddWizardImportEntity;

public abstract class ImportStepBase<WVM extends ImportAbstractWizardViewModel, WSM extends IWizardEntity>
        implements IImportStep<WVM, WSM> {

    private WVM mParentWizardViewModel;

    private Component mRootStepComponent;

    public ImportStepBase() {
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
    public void onBeforePrev() {
        // do nothing
    }

    @Override
    public void onBeforeNext(WSM wizardSaveEntity) {
        // do nothing
    }

    @Override
    public void onCancel() {
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

	public void onBeforePrev(AddWizardImportEntity wizardSaveEntity) {
		// TODO Auto-generated method stub
		
	}
}

/***End SQLCm 5.4***/