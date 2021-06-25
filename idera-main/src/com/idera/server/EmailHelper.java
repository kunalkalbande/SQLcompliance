package com.idera.server;

public class EmailHelper {

    public static boolean isValidEmailAddress(String inEmail) {
        boolean isValid = true;
        String email = inEmail.trim();
        // we only check for a @ and no semicolons
        if (email.length() < 3) // must be a semicolon and at least a user name
        // and domain
        {
            isValid = false;
        } else {
            if (email.contains(";") || email.contains(",")) {
                isValid = false;
            } else {
                // must contain exactly one @
                int count = 0;
                int pos = -1;
                for (int i = 0; i < email.length(); i++) {
                    if (email.charAt(i) == '@') {
                        count++;
                        pos = i;
                    }
                }
                if (count != 1 || pos == 0 || pos == (email.length() - 1)) {
                    isValid = false;
                } else if (pos == 0) {
                    // make sure that before and after @ is not blank
                    String user = email.substring(0, pos).trim();
                    String domain = email.substring(pos + 1).trim();
                    if (user.isEmpty() || domain.isEmpty()) {
                        isValid = false;
                    }
                }
            }
        }
        return isValid;
    }

    public static boolean IsValidEmailAddressList(String emailList) {
        boolean isValid = true;
        String separators = ";,";
        String[] emails = emailList.split(separators);
        if (emails.length == 0) {
            isValid = false;
        } else {
            for (String e : emails) {
                if (!e.isEmpty() && !isValidEmailAddress(e)) {
                    isValid = false;
                }
            }
        }
        return isValid;
    }
}
