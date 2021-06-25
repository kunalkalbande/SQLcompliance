/***Start SQLCm 5.4***/
/*Requirement 4.1.4.1*/

package com.idera.sqlcm.wizard;

import org.zkoss.zk.ui.Component;

import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;

/**
 * Each step must be class. DO NOT USE the same implementation of step 2 times or more!
 */
public interface IImportStep<WVM extends ImportAbstractWizardViewModel, WSE extends IWizardEntity> {

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

    void onBeforePrev();

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
    void onCancel();
    void onShow(WSE wizardSaveEntity);
}

/***End SQLCm 5.4***/