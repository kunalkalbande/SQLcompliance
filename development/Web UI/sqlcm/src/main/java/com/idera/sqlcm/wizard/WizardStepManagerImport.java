/***Start SQLCm 5.4***/
/*Requirement 4.1.4.1*/

package com.idera.sqlcm.wizard;

import org.apache.log4j.Logger;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zul.Panelchildren;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.NoSuchElementException;

public class WizardStepManagerImport {

    private static final Logger logger = Logger.getLogger(WizardStepManagerImport.class);
    public static final String STEP_VM_ID = "step_vm";

    private ImportWizardViewModel mParentWizardViewModel;

    private LinkedList<IStepImport> mStepsStack; // used to handle back button.
    private HashMap<String, IStepImport> mRegisteredSteps;
    private List<String> mZulStepList = new ArrayList<>(10);

    WizardStepManagerImport(ImportWizardViewModel parentWizardViewModel) {
        if (parentWizardViewModel == null) {
            throw new IllegalArgumentException(" Parent parentWizardViewModel must not be null ");
        }
        mParentWizardViewModel = parentWizardViewModel;
        mRegisteredSteps = new HashMap<>(initialStepCapacity());
        mStepsStack = new LinkedList<>();
    }

    protected int initialStepCapacity() {
        return 10;
    }

    public WizardStepManagerImport registerStep(String zulPath) {
        if (zulPath == null) {
            logger.info(" zulPath is null ");
            return this;
        }

        mZulStepList.add(zulPath);

        return this;
    }

    void initSteps(Panelchildren mainPanelChildren) {

        Component stepRootComponent;
        for (String zulPath: mZulStepList) {
            stepRootComponent = Executions.createComponents(zulPath, mainPanelChildren, null);
            Object viewModelStepObj = stepRootComponent.getAttribute(STEP_VM_ID);
            if (viewModelStepObj == null) {
                throw new RuntimeException(
                        String.format(" Root component of wizard step file '%s' must contain viewModel with id '%s' ",
                                zulPath, STEP_VM_ID));
            }

            IStepImport stepViewModel;
            try {
                stepViewModel = (IStepImport) viewModelStepObj;
            } catch (ClassCastException e) {
                throw new RuntimeException(
                        String.format(" Step viewModel class in '%s' must be heir of '%s' ",
                                zulPath, IStepImport.class.getName()));
            }
            stepViewModel.init(mParentWizardViewModel, stepRootComponent);
            mRegisteredSteps.put(zulPath, stepViewModel);

            // add step to stack if it first
            if (stepViewModel.isFirst()) {
                if (mStepsStack.size() == 0) {
                    mStepsStack.addFirst(stepViewModel);
                } else {
                    throw new RuntimeException(" Must be only one first step! ");
                }
                stepRootComponent.setVisible(true);

            } else {
                stepRootComponent.setVisible(false);
            }
        }

        if(mStepsStack.size() == 0) {
            throw new RuntimeException(" First step does not exist! ");
        }

    }

    IStepImport getFirst() {
        return mStepsStack.peekFirst();
    }

    IStepImport next(boolean removeCurrentStepFromStack) {
        // get current step. must not be null.
        IStepImport currentStep = mStepsStack.peekFirst();
        String nextStepZul = currentStep.getNextStepZul();
        if (nextStepZul != null) {
            if (removeCurrentStepFromStack) {
                mStepsStack.removeFirst();
            }
            // visibility = false for current step component
            currentStep.setVisible(false);
            // get next step class if not null. IF null return
            // add new step to stack. visibility = true
            IStepImport stepToDisplay = mRegisteredSteps.get(nextStepZul);
            if (stepToDisplay == null) {
                throw new RuntimeException(String.format(" Step Zul with path %s is not registered ", nextStepZul));
            }
            stepToDisplay.setVisible(true);
            mStepsStack.addFirst(stepToDisplay);
            return stepToDisplay;
        }
        return null;
    }

    IStepImport next() {
        return next(false);
    }

    IStepImport prev() {
        // get current step. must not be null.
        if (mStepsStack.size() > 1) {
            // do mStepsStack.poll() to get current and remove from stack.
            IStepImport currentStep = mStepsStack.poll();
            // set component visibility = false of polled step
            currentStep.setVisible(false);

            // do peek first to get component. set visibility to true.
            IStepImport stepToDisplay = mStepsStack.peekFirst();
            stepToDisplay.setVisible(true);
            return stepToDisplay;
        }
        return null;
    }

    IStepImport getCurrentStep() {
        return mStepsStack.peekFirst();
    }

    IStepImport getStepOrException(Class<? extends IStepImport> stepClass) throws NoSuchElementException {
        IStepImport result = mRegisteredSteps.get(stepClass);
        if (result == null) {
            throw new NoSuchElementException(String.format(" Step with class name '%s' not found ", stepClass.getName()));
        }
        return result;
    }

}

/***End SQLCm 5.4***/