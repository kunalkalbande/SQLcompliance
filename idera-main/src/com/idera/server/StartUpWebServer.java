package com.idera.server;

import com.idera.WebServerProperties;
import com.idera.core.util.GlobalUtil;
import com.idera.i18n.I18NStrings;
import com.idera.i18n.I18NUtil;
import com.idera.i18n.LangProperties;
import com.idera.server.web.WebConstants;
import com.sun.net.httpserver.*;
import org.apache.log4j.Logger;

import javax.net.ssl.KeyManagerFactory;
import javax.net.ssl.SSLContext;
import javax.net.ssl.TrustManagerFactory;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.InetSocketAddress;
import java.nio.charset.Charset;
import java.security.KeyStore;
import java.util.List;
import java.util.Locale;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class StartUpWebServer {
    // private static final mazz.i18n.Logger logger =
    // mazz.i18n.LoggerFactory.getLogger(StartUpWebServer.class);
    // private static final org.apache.log4j.Logger debug =
    // org.apache.log4j.Logger.getLogger(StartUpWebServer.class);

    private static final Logger logger = Logger.getLogger(StartUpWebServer.class);
    private static final String STARTUP_LOGO_BASE64_IMG = "<img style=\"border: 0; vertical-align: middle; width: 64px; float: left;\" src=\"data:image/png;base64,"
            + "iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAYAAABXAvmHAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA2"
            + "ZpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3pr"
            + "YzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMD"
            + "ExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3"
            + "LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG"
            + "1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hh"
            + "cC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtcE1NOk"
            + "9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDozRDEwNEMwRTkyMjdFMzExQjZDOUI4ODlDODVFMzFBRiIgeG1wTU06RG9jdW1l"
            + "bnRJRD0ieG1wLmRpZDpBMkMxQjY4ODMwNTkxMUUzQTY3MDlFQjFBRDM3OUNEQSIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZD"
            + "pBMkMxQjY4NzMwNTkxMUUzQTY3MDlFQjFBRDM3OUNEQSIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgQ1M2IChX"
            + "aW5kb3dzKSI+IDx4bXBNTTpEZXJpdmVkRnJvbSBzdFJlZjppbnN0YW5jZUlEPSJ4bXAuaWlkOjM0NkE1QkQ5NTEzMEUzMTFCRT"
            + "JBOTQ0NjI4Q0IxM0FBIiBzdFJlZjpkb2N1bWVudElEPSJ4bXAuZGlkOjNEMTA0QzBFOTIyN0UzMTFCNkM5Qjg4OUM4NUUzMUFG"
            + "Ii8+IDwvcmRmOkRlc2NyaXB0aW9uPiA8L3JkZjpSREY+IDwveDp4bXBtZXRhPiA8P3hwYWNrZXQgZW5kPSJyIj8+QLuyIAAADO"
            + "xJREFUeNq8WntwFdUd/nbv+5V78yYkIeGRQAARiOADZVBBx2q1dBSkRax1pFpm6rSOVWZ02s5YBf5xxmk7ijN1FLSoFCwKlIeo"
            + "EBhjCkEIr4SEhISEkNxHct97d+/2d3bvK+Qm9wbQHQ57dvfsnu/7vc+54T6t5HC9x6xVT9wR8gVfCrjc80JuT17Q2aeLDA7wYi"
            + "ioPNcaTdDZ7FFjfkHE6HC4DHZ7g95kWH/2ky1Hrndu7loJzFq1cq6vz/2us7ll1kBbi7b03p8gb+ZsWCsmw1JSBp0jD1qTCZAB"
            + "MRhE2ONEoLsTvo42uE8dx6UDu5EzcYromDT5hDHH+kzztq3HfhQCN69cscjVefmD7vpvyysffgzlix+EY2oN4Yx9R1b+pXYgy7"
            + "Hr1H5UxkDLGXR9uQsXv9iKwtm1PZbiopWtO7Yf+EEIzHnyFxZPd9/Bzrq6uVXLV2HKo78EbzQmQcbRJXEr13KCSyoheQhZiUyt"
            + "/bMtaN26GcW1848Z83Lvbvv8s8EbRuCmZY8u76j/3+aCm2u1U5Yug6m4RJk9jmMkgHISfeJ5/J6cOjZ2z3+5Bx07t8N18qiYN6"
            + "1mdeeXe9+7bgJTH3rgn60HvnmqZsUTmPTw0qSEU6WdKukE+KuAx8elEkiMV69ZXybT6ty/C21bP0RR7fz3e44c/NVo+LSjPZy8"
            + "+J49zbv23Hf72ldQePMcIByAnEogFWgKsCEmooBOakQdn3w3eY+5BSMQRcntd8Fgd+DU228+WTh3fknfse/uH7MGJt+9cJ/EaR"
            + "dXLl4M+4SKFFCxSRMmcTW4oVpIvTdkTEILyTFRAh8lDciSTGSi8HV1offbOkjh4H7nycYl6XBqljmGE6hesugfrV8dWj7nqVWw"
            + "jx8HWYyQp0XUMzU5tZ94Jih9OSIgSv1oJIKoINAj1iKQ4veU5+o4ORI/C+p9pR9W3osKYWj0BsofOejc8/mk/JmzioNXendm1M"
            + "DUO2cvbW04s23e6qdQNKMmKaVU6Q8xC/UekxwzgShJT6SzJKnXEkk1bu+IBVs+1jh6wCW0qb6raEGK0ntME1Hl3kB7Ky7u/g/s"
            + "VTWPu0+f+HhEAlNrK82X2vqcE++9xzhp4YIU0FeZAVRnU3DFgIs0UYT6fMEEGCZMh6GoHDqrAxqjGWIogIjXTVGmE962JoS6zi"
            + "vOp6WpNdQ4xX7Y9wmwOJwAI+VsOgH32VOCeXxZvudsky+tE/sGgp8UluYaJ9aMB4RAMopEY+Dj/TgBlmVpEpEmQOFEOOY9AGNx"
            + "xfBIQSRYMxWWouCm2+DtbkfXV9vh72qBkeeJiAxNTBgqaCIQOyuNvm+rqEDI2aePDHq+oKGLhmmganbFnLamrqOLHp3PmXLt4M"
            + "tmQJHNEOeLmQtTN03GgIdFwDTvIThql4y5DGg/uBM9+z6FiY/CwHPgmUnFACsExBQS1ASfD911X4PjMJeuG4doYNDle79q9gTO"
            + "bDOSI4UgOzsBW2Ey0cRMSVbAq5IPEwnbfb+Btbr2muqpyoUPQp+Ti3Ob3oSFkSBNaGNaYOFUSiEgSZLq2FoN3Rc/YPk1QaBqTk"
            + "V128nOm25ZPDPxcXnwCjitgYxUlzCXBAEmDfq4eeHKUcF7Ll+C50o3jBYbiiZWg+f5YWPGz74DAVc/Wj/+O2yExsgcPKoSkEWJ"
            + "wLL5JEUjfioENeQ4oiDP5HiumnA1KwRC/vD6yppSesinlAf0ortH0UJKoFecKkLg5dIZsN+SPr+4Cfjed9ah/+xxGMhLmaMa7P"
            + "m49YnnMfnWRcPGT7nnYXTWH4DnTANyiKOepuLlGAHmxHQOuZ0QfWp5xGt4JtT11F2qiMTT772vrGrccCQsvnv7VJNi8ZmaJIQQ"
            + "CgfhuHtlWvBeZx8+Wvs0PKe/Q75GQpFORLFOgtXXi4a31qLtyP607818/Lfw+APwUQsHA2BrCUpgiLIINuhGhMrx+MHzzG9lxe"
            + "n46rmV070uv9lRaEtvByxBBYg5gZfJBkUiwI+rgmn85LTDv970N8ikuVwCn0/g87USCpRGfT6CpvfeUBLb1UchmZhp4gwMBgII"
            + "EviIQiAEKeBTCAyJ/RwLLrCwIpkXQpFnS6cUj+5tjETYTyoN04fDMEy7fcShrYd2kUNKsGtE2OnMiNh5EY7YPd1gL/rPNaZ9t/"
            + "y2xfAFQkQgBEEB74fo9yJp10NJ0PEMHwqE78wtyskcMqKSYj4CacI0YUbaIRKVCGzlZYQIEySFCOsbqOkRgZndoxZ296V9v2T6"
            + "XPiCYYSohUkDIpUWIxZxavS/jReCkXJbrjmrsKdkXHIoQ0Fp2ucarQ72wiJoyAH5WONYi0bVGC9LSrMUpX8/t7QcoYgIQRLVTJ"
            + "xG8kkGyv9lTAN2S052BOJJTGseWWPVi34KIaqWFWI0nvjUWokCCvTjKpFXPSvtu2Z7rlpHxVPPaAsZlYGD+YDWYNZnnXwYIInW"
            + "BSMdCyhUGsuq4ItI8AlUEkeiCFLY9VMLyFpUPfcGqZ9P+26YbD4qJyJ2hqWYWqXwYoSqfp1mTBk05OwZ8ZnBYsVD6/6FvLt+Dp"
            + "eogTMowhkik6iYjZo/fYi8mtET31g3JbRjGs2qR4rB3raTsJROGXGc0WbHgt9RuHzuLwi6emGw2qGzZA4UfR2tJFI18bFQz8mZ"
            + "MfEkfVqHSFkR4Dn144OnDmc1XqPTw1pcnhV4drQ11EEXK7GVXMVl3HCQeb1RJ4YDQnYaoKajNO4/fQSC140bebCQeXrvNhgJvU"
            + "7RAgd+VOjqa7zRbBjwDway1gCVSxTXI+j6cssNJXB051ZInn6YaAJWWrPFzmgKiK0RPbzepOv0ugNj8gFWoPUf2IzQgPOGgBco"
            + "aX3zzgZYdTzMGvX7CoHMGuhiGqhzXxnM2oSYkxlISkYxhHMfrr8hBPa+vQFRVw9sRMBCyPVscZPB/mNhtp75wMZL53uznox9WE"
            + "cSMus1CDbuxcVDO64LfOuxb3H0o7fh0PPIIQJGEg6dFGFxoxJQGGzkm4+1N9nyLAFPnzfrSZkWmK3mGDRo3/QaBrparwm8z92P"
            + "f7/yLOw6Sqn0LSuFIGPMgUe1f1aWcGB236g4uqPAtrer5XLWZpTQAiVAKxfBibd+D8HvHbPdv//8SvCeXuQReDuz/5j0+Qz2z8"
            + "oSGqEsLBQCRovhpfYzl5Q1aLYkmJQMZK82MiW9qwP1656FGA5mWY7I+PjVNfCd/x75Rg1yDbziwAnpZyyMowzEiwkCLY0dzQXj"
            + "c5taT1wcU1ZmpmSmie0kQa7zBOrf/INSUmcCv/W1F9B1eDcKjFoioEUOCYGZJPteptwlUdKl9XATWw8nCLAjJ8+6quX4RTngDY"
            + "3JlFQSGjgIiHj6EI6sW0PrhvCI4LevX4vz/92CQpNWkb5dz0yHUx03g+mwzQWqGmRazKxKBJV4p+V4R+O4yoJdbU2d2WsBal7Q"
            + "kz9YSIq5RCJy5jAO/nX1MHOK0oJo2+svovmLTSgi8Ez6zHEthDweNjOZToTWCrSg3x3fExpCgB1Wu2lZX5cr2NbUdU0krEQij8"
            + "DJ5xvw9Z9/nXBskTTy6avP4cIekryZwJtonFGj2D3Luuz9TOCFsMg2uoJkPo8NCeupF+eOtgfGVRQ80nz0gkxExkRCGyPBnDqP"
            + "pMt3fI+vXl6OK2ca8dkLj8NVvwfjLDoUE0H2XHFaTZaSF2iVFhRkjVbzM/KBwKjb667egbbyqSW60/VtC8mkQIlu6NIo+YtQak"
            + "pXzomd51jVGqWC78K+T6hi6VUcnZmMnQhayeYNitTVgi35+4C65xqNNcXmCbzX5YfOoH2diLyb1e8DA/3eA2VVxdMG+n0zKcTC"
            + "YNRnJBDfQudSwiyrZ1hdw+zcwnKGVnVY8ttkuJRHJhAhswn6Qszut1B/TdrKYCS1dbX0rohGo5sP72hEf7d7zD7BSmIGmmVrJv"
            + "0cPZ9w2GxsPhwU4Lo8wASzmZa9K0Zcc6TTQHLD1799wrQSI+WJBSQFTtn8yqCB+LUaZlUJaTk1QWliU3Gpv6ul0QDNC88Vr2yy"
            + "GjaEAsKaUWuzTBK9eLZn7YSpJfeTRoJnGy4gNIbFT4IElywPuFEXNRKcJHWvJxA055geCfrCL2csLrNa6jV17SOHLhJCwr66Hc"
            + "dw4VT2ZUdW+00kdWePBx1nuum70n6zzVjkHwh8/oP8qcHEGaW1voHARrLPueMnF6G4PB9Whzmh/vhPTom+slWevKeOU+/5B0Pw"
            + "9A3C3TsAs810UmfUPu3uHWz4Uf7Yo3J6aXXIH97gdfuX+AeD5sKyPNhyLTBR1NKb9dBqNcouMiMgChIECofhQJiiShgkXbDy3W"
            + "DWB2hBtU+r1/zR2e1p/lH+2CPdUTGtZKYQEldTtLhTCEfKI+FIDtmzjsyMi+3ny1qdJkJtUKvXdmp1fB0lpY1XOl1N1zv3/wUY" + "ADgeob0EMOVxAAAAAElFTkSuQmCC" + "\"/>";

    // @formatter:off
    private static final String RELOAD_JAVASCRIPT = "<script type=\"text/javascript\">" +

    "var req;" +

    "function checkStatus() {" +

    "var loadURL = '/images/IDontExist';" +

    "initRequest();" +

    "if( req != false ) {" + "req.onreadystatechange = processReqChange;" + "req.open(\"GET\", loadURL, true);" + "req.send(\"\");" + "}" +

    "}" +

    "function processReqChange() {" + "if (req.readyState == 4) {" + "if (req.status == 404) {" + "window.location = document.location.protocol + \"//\" + document.location.host;"
                    + "}" + "else {" + "setTimeout(\"checkStatus()\", 2000);" + "}" + "}" + "}" +

                    "function initRequest() {" +

                    "req = false;" +

                    "if(window.XMLHttpRequest && !(window.ActiveXObject)) {" + "try {" + "req = new XMLHttpRequest();" + "}" + "catch(e) {" + "req = false;" + "}" + "}"
                    + "else if(window.ActiveXObject) {" + "try {" + "req = new ActiveXObject(\"Msxml2.XMLHTTP\");" + "}" + "catch(e) {" + "try {"
                    + "req = new ActiveXObject(\"Microsoft.XMLHTTP\");" + "}" + "catch(e) {" + "req = false;" + "}" + "}" + "}" +

                    "}" +

                    "setTimeout(\"checkStatus()\", 500);" +

                    "</script>";

    private static final String STARTUP_PAGE_HTML = "<html>" + "<head>"
                    + RELOAD_JAVASCRIPT
                    + "</head>"
                    + "<body>"
                    + "<div style=\""
                    + "overflow: auto; "
                    + "width: 640px;"
                    + "margin: 0 auto 0 auto; "
                    + "padding: 0px; "
                    + "border: 2px solid blue; "
                    + "background-color: #DDDDFF; "
                    + "text-align: center; "
                    + "vertical-align: top;\">"
                    + "<span style=\"vertical-align: top; padding: 8px;font-weight: bold; font-size: larger; background-color: #DDDDFF; width: 620px; height: 54px; clear: both;\">"
                    + STARTUP_LOGO_BASE64_IMG
                    + "<span style=\"vertical-align: middle; text-align: center; float: left; clear: right; width: 536px; margin-top: 10px; margin-left: 10px;\">%s</span>"
                    + "</span>" + "<br/>" + "<hr style=\"clear: both; color: blue; background: blue;\"/>"
                    + "<span style=\"width: 600px; font-size: small; padding: 5px 0; float: left; clear: both;\">%s</span>"
                    + "<span style=\"width: 600px; font-size: small; padding: 5px 0; float: left; clear: both;\">%s</span>"
                    + "<span style=\"width: 600px; font-size: small; padding: 5px 0; float: left; clear: both; margin-bottom: 10px;\">%s</span>" + "</div>" + "</body>" + "</html>";
    // @formatter:on

    private static final Pattern COOKIE_VALUE_PATTERN = Pattern.compile(".*" + WebConstants.IDERA_CWF_COOKIE_NAME + "=\"([^\"]+)\".*");

    private static final String ACCEPT_REQUEST_HEADER = "Accept";
    private static final String COOKIE_REQUEST_HEADER = "Cookie";

    private static final int SUCCESS_RESPONSE_CODE = 200;
    private static final String CONTENT_TYPE_RESPONSE_HEADER = "Content-Type";
    private static final String CONTENT_TYPE_RESPONSE_VALUE = "text/html; charset=UTF-8;";

    private static final int REDIRECT_RESPONSE_CODE = 302;
    private static final String REDIRECT_LOCATION_RESPONSE_HEADER = "Location";
    private static final String REDIRECT_LOCATION_RESPONSE_VALUE = "/";

    private static final Charset UTF8_CHARSET = Charset.forName("UTF-8");

    private boolean started = false;

    private Locale serverLocale = Locale.ENGLISH;

    private HttpServer httpServer;
    private HttpsServer sslServer;

    private static final StartUpWebServer INSTANCE = new StartUpWebServer();

    private StartUpWebServer() {
        super();
    }

    public static StartUpWebServer getInstance() {
        return INSTANCE;
    }

    public Locale getServerLocale() {
        return this.serverLocale;
    }

    public void setServerLocale(Locale serverLocale) {
        this.serverLocale = serverLocale;
    }

    public void startup() throws Exception {
        if (this.started) {
            return;
        }
        WebServerProperties webProperties = ServerFacade.getWebServerProperties();
        if (webProperties.getStartupWebServerEnabled() == false) {
            return;
        }
        if (webProperties.getHttpEnabled()) {
            this.httpServer = HttpServer.create();
            this.httpServer.createContext("/", new StartUpPageHandler(this.serverLocale));
            this.httpServer.bind(new InetSocketAddress(webProperties.getHttpPort()), 1000);
            this.httpServer.start();
            logger.info("Started HTTP interface for Startup Web Server");
            this.started = true;
        }
        if (webProperties.getSslEnabled()) {
            this.sslServer = HttpsServer.create();
            this.sslServer.setHttpsConfigurator(new HttpsConfigurator(getSSLContext(webProperties.getSslKeystorePath())));
            this.sslServer.createContext("/", new StartUpPageHandler(this.serverLocale));
            this.sslServer.bind(new InetSocketAddress(webProperties.getSslPort()), 1000);
            this.sslServer.start();
            logger.info("Started HTTPs interface for Startup Web Server");
            this.started = true;
        }
    }

    public void shutdown() throws IOException {
        if (!this.started) {
            return;
        }
        WebServerProperties webProperties = ServerFacade.getWebServerProperties();
        if (webProperties.getStartupWebServerEnabled() == false) {
            return;
        }
        logger.info("Initiating shuting down the Startup HTTP sever");
        if (this.httpServer != null) {
            this.httpServer.stop(0);
            this.httpServer = null;
        }
        if (this.sslServer != null) {
            this.sslServer.stop(0);
            this.sslServer = null;
        }
        logger.info("shutdown Startup HTTP sever, completed.");
        this.started = false;
        // Hard wait, so that HTTPServer goes down gracefully before starting
        // the tomcat server.
        GlobalUtil.wait(webProperties.getStartupWebServerWaitTime());
    }

    public boolean isStarted() {
        return this.started;
    }

    private static class StartUpPageHandler implements HttpHandler {

        private Locale contentLocale;

        public StartUpPageHandler(Locale contentLocale) {
            this.contentLocale = contentLocale;
        }

        @Override
        public void handle(HttpExchange httpExchange) throws IOException {
            try {
                Headers reqHeaders = httpExchange.getRequestHeaders();
                List<String> acceptHeaders = reqHeaders.get(ACCEPT_REQUEST_HEADER);
                boolean acceptHtml = false;
                if (acceptHeaders != null && !acceptHeaders.isEmpty()) {
                    for (String header : acceptHeaders) {
                        if (header != null && (header.contains("text/[x]?html") || header.contains("*/*"))) {
                            acceptHtml = true;
                            break;
                        }
                    }
                }
                List<String> cookieHeaders = reqHeaders.get(COOKIE_REQUEST_HEADER);
                String selectedLocaleString = null;
                if (cookieHeaders != null && !cookieHeaders.isEmpty()) {
                    COOKIE:
                    for (String header : cookieHeaders) {
                        Matcher matcher = COOKIE_VALUE_PATTERN.matcher(header);
                        if (matcher.matches()) {
                            String[] keyValuePairs = matcher.group(1).split(WebConstants.IDERA_CWF_COOKIE_VALUES_DELIMITER);
                            for (String keyValuePair : keyValuePairs) {
                                String[] keyValue = keyValuePair.split(WebConstants.IDERA_CWF_COOKIE_KEY_VALUE_DELIMITER);
                                if (keyValue[0].equals(WebConstants.IDERA_CWF_COOKIE_SELECTED_LOCALE_KEY)) {
                                    selectedLocaleString = keyValue[1];
                                    break COOKIE;
                                }
                            }
                        }
                    }
                }
                Locale selectedLocale = LangProperties.parseLocaleString(selectedLocaleString);
                if (selectedLocale == null) {
                    selectedLocale = this.contentLocale;
                }
                InputStream request = httpExchange.getRequestBody();
                try {
                    while (request.available() > 0) {
                        request.read();
                    }
                } finally {
                    request.close();
                }
                httpExchange.getRequestBody().close();
                if (acceptHtml) {
                    String pageContent = getPageContent(selectedLocale);
                    httpExchange.getResponseHeaders().add(CONTENT_TYPE_RESPONSE_HEADER, CONTENT_TYPE_RESPONSE_VALUE);
                    byte[] contentBytes = pageContent.getBytes(UTF8_CHARSET);
                    httpExchange.sendResponseHeaders(SUCCESS_RESPONSE_CODE, contentBytes.length);
                    OutputStream response = httpExchange.getResponseBody();
                    try {
                        response.write(contentBytes);
                    } finally {
                        response.close();
                    }
                } else {
                    httpExchange.getResponseHeaders().add(REDIRECT_LOCATION_RESPONSE_HEADER, REDIRECT_LOCATION_RESPONSE_VALUE);
                    httpExchange.sendResponseHeaders(REDIRECT_RESPONSE_CODE, 0);
                    httpExchange.getResponseBody().close();
                }
            } finally {
                httpExchange.close();
            }
        }
    }

    private static SSLContext getSSLContext(String keyStorePath) throws Exception {
        char[] passphrase = "password".toCharArray();
        KeyStore ks = KeyStore.getInstance("JKS");
        FileInputStream fis = new FileInputStream(keyStorePath);
        try {
            ks.load(fis, passphrase);
        } finally {
            fis.close();
        }
        KeyManagerFactory kmFactory = KeyManagerFactory.getInstance(KeyManagerFactory.getDefaultAlgorithm());
        kmFactory.init(ks, passphrase);
        TrustManagerFactory tmFactory = TrustManagerFactory.getInstance(TrustManagerFactory.getDefaultAlgorithm());
        tmFactory.init(ks);
        SSLContext sslContext = SSLContext.getInstance("TLSv1");
        sslContext.init(kmFactory.getKeyManagers(), tmFactory.getTrustManagers(), null);
        return sslContext;
    }

    private static String getPageContent(Locale locale) {
        return String
                .format(STARTUP_PAGE_HTML, I18NUtil.getLocalizedMessage(locale, I18NStrings.SERVER_IS_CURRENTLY_STARTING_OR_DOWN_FOR_MAINTENANCE),
                        I18NUtil.getLocalizedMessage(locale, I18NStrings.THIS_MAY_TAKE_SEVERAL_MINUTES),
                        I18NUtil.getLocalizedMessage(locale, I18NStrings.IF_YOU_ARE_THE_ADMINISTRATOR_CHECK_THE_LOG_FILES_FOR_DETAILS),
                        I18NUtil.getLocalizedMessage(locale, I18NStrings.OTHERWISE_PLEASE_CHECK_BACK_SOON_OR_CONTACT_YOUR_ADMINISTRATOR_FOR_HELP));
    }
}
