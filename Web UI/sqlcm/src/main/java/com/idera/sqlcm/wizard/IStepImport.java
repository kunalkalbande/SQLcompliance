/***Start SQLCm 5.4***/
/*Requirement 4.1.4.1*/

package com.idera.sqlcm.wizard;

import org.zkoss.zk.ui.Component;

/**
 * Each step must be class. DO NOT USE the same implementation of step 2 times or more!
 */
public interface IStepImport<WVM extends ImportWizardViewModel, WSE extends IWizardEntity> {

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

    /**
     *
     * @param wizardSaveEntity
     * @return true if you don't need to perform onCancel and close dialog
     */
    boolean onBeforeCancel(WSE wizardSaveEntity);
    void onCancel(WSE wizardSaveEntity);
    void onShow(WSE wizardSaveEntity);
}
/***End SQLCm 5.4***/