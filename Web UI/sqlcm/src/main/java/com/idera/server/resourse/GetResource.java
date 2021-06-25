package com.idera.server.resourse;

import java.io.InputStream;
import java.net.URL;

public class GetResource {

    public static InputStream getResource(String image) {
        return GetResource.class.getResourceAsStream(image);
    }

    public static URL getResourceURL(String image) {
        return GetResource.class.getResource(image);
    }
}
