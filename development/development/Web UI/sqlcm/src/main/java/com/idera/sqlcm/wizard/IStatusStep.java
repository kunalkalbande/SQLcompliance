package com.idera.sqlcm.wizard;

import org.zkoss.zk.ui.Component;

import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;

/**
 * Each step must be class. DO NOT USE the same implementation of step 2 times or more!
 */
public interface IStatusStep<WVM extends AbstractStatusWizardViewModel, WSE extends IWizardEntity> {

    /**
     * @return next step class OR null if step is last
     */
    String getNextStepZul();

    Component getRootStepComponent();

    WVM getParentWizardViewModel();

    void setVisible(boolean visible);

    void init(WVM parentWizardViewModel, Component rootStepComponent);
    void onAfterWire();

    boolean isFirst();
    boolean isLast();
    boolean isValid();
    String getTips();
    String getHelpUrl();

    void onBeforePrev(WSE wizardSaveEntity);

    /**
     * Will be called after click on Next or Finish button.
     *
     * @param wizardSaveEntity
     */
    void onBeforeNext(WSE wizardSaveEntity);

    /**
     * Will be called after click on Finish button, and only in case current step is last.
     *
     * @param wizardSaveEntity
     */
    void onFinish(WSE wizardSaveEntity);
    void onCancel(WSE wizardSaveEntity);
    void onShow(WSE wizardSaveEntity);

	void onCancel(AddAlertRulesSaveEntity wizardSaveEntity);
}
