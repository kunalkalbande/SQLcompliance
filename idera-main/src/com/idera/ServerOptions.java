package com.idera;

import org.apache.commons.cli.CommandLine;
import org.apache.commons.cli.HelpFormatter;
import org.apache.commons.cli.Option;
import org.apache.commons.cli.Options;

public class ServerOptions {
    public static void processOptions(CommandLine line, Options options, String[] boolOptionsList, String[] argOptionsList, String progName) throws Exception {
        int i;
        int j;
        boolean found = false;
        int numOptions = 0;
        String name = null;
        Option opts[] = null;
        opts = line.getOptions();
        numOptions = opts.length;
        try {
            for (i = 0; i < numOptions; i++) {
                name = opts[i].getLongOpt();
                found = false;
                if (name != null) {
                    if (name.equals("help")) {
                        HelpFormatter formatter = new HelpFormatter();
                        formatter.printHelp(progName, options);
                        System.setProperty("help", "TRUE");
                        found = true;
                        break;
                    }
                    if (found == false) {
                        for (j = 0; j < boolOptionsList.length; j++) {
                            if (name.equals(boolOptionsList[j])) {
                                System.setProperty(boolOptionsList[j], "TRUE");
                                found = true;
                                break;
                            }
                        }
                    }
                    if (found == false) {
                        for (j = 0; j < argOptionsList.length; j++) {
                            if (name.equals(argOptionsList[j])) {
                                System.setProperty(argOptionsList[j], opts[i].getValue());
                                found = true;
                                break;
                            }
                        }
                    }
                }
            }
        } catch (SecurityException se) {
            System.err.println("Setting system property failed.  Reason: " + se.getMessage());
            throw se;
        } catch (Exception e) {
            System.err.println("Internal error: " + e.getMessage());
        }
    }
}
