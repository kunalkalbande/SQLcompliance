package com.idera.server;

import org.springframework.dao.DataAccessException;
import sun.misc.BASE64Encoder;

import java.io.UnsupportedEncodingException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;

public class PasswordEncoder implements org.springframework.security.providers.encoding.PasswordEncoder {
    protected static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory.getLogger(PasswordEncoder.class);
    protected static final org.apache.log4j.Logger debug = org.apache.log4j.Logger.getLogger(PasswordEncoder.class);

    @Override
    public String encodePassword(String password, Object salt) throws DataAccessException {
        MessageDigest md = null;
        try {
            md = MessageDigest.getInstance("SHA-1");
        } catch (NoSuchAlgorithmException ne) {
            // Reaching this is developer error and should be a runtime
            // exception
            throw new RuntimeException(ne);
        }
        try {
            md.update(password.getBytes("UTF-8"));
        } catch (UnsupportedEncodingException e) {
            // Reaching this is developer error and should be a runtime
            // exception
            throw new RuntimeException(e);
        }
        byte raw[] = md.digest();
        String hash = (new BASE64Encoder()).encode(raw);
        return hash;
    }

    @Override
    public boolean isPasswordValid(String encPass, String rawPass, Object salt) throws DataAccessException {
        boolean result = encPass.equals(this.encodePassword(rawPass, salt));
        debug.debug("PasswordEncoder: result = " + result);
        return result;
    }
}
