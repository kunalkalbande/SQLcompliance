package com.idera.sqlcm.utils;

import java.io.BufferedReader;
import java.io.Closeable;
import java.io.IOException;
import java.io.Reader;
import java.util.ArrayList;
import java.util.List;

/**
 * CmCsvReader class.
 * 
 */
public class CmCsvReader implements Closeable {

    /** Buffered reader. */
    private BufferedReader bufferedReader;

    /** hash next flag. */
    private boolean hasNext = true;

    /** Cm CSV parser. */
    private CmCsvParser parser;

    /** Skip lines. */
    private int skipLines;

    /** Lines skipped flag. */
    private boolean linesSkipped;

    /**
     * The default line to start reading.
     */
    public static final int DEFAULT_SKIP_LINES = 0;

    /**
     * Constructs CSVReader using a comma for the separator.
     * 
     * @param reader the reader to an underlying CSV source.
     */
    public CmCsvReader(Reader reader) {
        this(reader, CmCsvParser.DEFAULT_SEPARATORS, CmCsvParser.DEFAULT_QUOTE_CHARACTER,
             CmCsvParser.DEFAULT_ESCAPE_CHARACTER);
    }

    /**
     * Constructs CSVReader with supplied separator.
     * 
     * @param reader the reader to an underlying CSV source.
     * @param separators the delimiter to use for separating entries.
     */
    public CmCsvReader(Reader reader, List<Character> separators) {
        this(reader, separators, CmCsvParser.DEFAULT_QUOTE_CHARACTER,
             CmCsvParser.DEFAULT_ESCAPE_CHARACTER);
    }

    /**
     * Constructs CSVReader with supplied separator and quote char.
     * 
     * @param reader the reader to an underlying CSV source.
     * @param separators the delimiter to use for separating entries
     * @param quotechar the character to use for quoted elements
     */
    public CmCsvReader(Reader reader, List<Character> separators, char quotechar) {
        this(reader, separators, quotechar, CmCsvParser.DEFAULT_ESCAPE_CHARACTER,
             DEFAULT_SKIP_LINES, CmCsvParser.DEFAULT_STRICT_QUOTES);
    }

    /**
     * Constructs CSVReader with supplied separator, quote char and quote handling behavior.
     *
     * @param reader the reader to an underlying CSV source.
     * @param separators the delimiter to use for separating entries
     * @param quotechar the character to use for quoted elements
     * @param strictQuotes sets if characters outside the quotes are ignored
     */
    public CmCsvReader(Reader reader, List<Character> separators, char quotechar,
                       boolean strictQuotes) {
        this(reader, separators, quotechar, CmCsvParser.DEFAULT_ESCAPE_CHARACTER,
             DEFAULT_SKIP_LINES, strictQuotes);
    }

    /**
     * Constructs CSVReader with supplied separator and quote char.
     *
     * @param reader the reader to an underlying CSV source.
     * @param separators the delimiter to use for separating entries
     * @param quotechar the character to use for quoted elements
     * @param escape the character to use for escaping a separator or quote
     */

    public CmCsvReader(Reader reader, List<Character> separators, char quotechar, char escape) {
        this(reader, separators, quotechar, escape, DEFAULT_SKIP_LINES,
             CmCsvParser.DEFAULT_STRICT_QUOTES);
    }

    /**
     * Constructs CSVReader with supplied separator and quote char.
     * 
     * @param reader the reader to an underlying CSV source.
     * @param separators the delimiter to use for separating entries
     * @param quotechar the character to use for quoted elements
     * @param line the line number to skip for start reading
     */
    public CmCsvReader(Reader reader, List<Character> separators, char quotechar, int line) {
        this(reader, separators, quotechar, CmCsvParser.DEFAULT_ESCAPE_CHARACTER, line,
             CmCsvParser.DEFAULT_STRICT_QUOTES);
    }

    /**
     * Constructs CSVReader with supplied separator and quote char.
     *
     * @param reader the reader to an underlying CSV source.
     * @param separators the delimiter to use for separating entries
     * @param quotechar the character to use for quoted elements
     * @param escape the character to use for escaping a separator or quote
     * @param line the line number to skip for start reading
     */
    public CmCsvReader(Reader reader, List<Character> separators, char quotechar, char escape,
                       int line) {
        this(reader, separators, quotechar, escape, line, CmCsvParser.DEFAULT_STRICT_QUOTES);
    }

    /**
     * Constructs CSVReader with supplied separator and quote char.
     * 
     * @param reader the reader to an underlying CSV source.
     * @param separators the delimiter to use for separating entries
     * @param quotechar the character to use for quoted elements
     * @param escape the character to use for escaping a separator or quote
     * @param line the line number to skip for start reading
     * @param strictQuotes sets if characters outside the quotes are ignored
     */
    public CmCsvReader(Reader reader, List<Character> separators, char quotechar, char escape,
                       int line, boolean strictQuotes) {
        this.bufferedReader = new BufferedReader(reader);
        this.parser = new CmCsvParser(separators, quotechar, escape, strictQuotes);
        this.skipLines = line;
    }

    /**
     * Reads the entire file into a List with each element being a String[] of tokens.
     * 
     * @return a List of String[], with each String[] representing a line of the file.
     * 
     * @throws java.io.IOException if bad things happen during the read
     */
    public List<String[]> readAll() throws IOException {

        List<String[]> allElements = new ArrayList<String[]>();
        while (hasNext) {
            String[] nextLineAsTokens = readNext();
            if (nextLineAsTokens != null)
                allElements.add(nextLineAsTokens);
        }
        return allElements;

    }

    /**
     * Reads the next line from the buffer and converts to a string array.
     * 
     * @return a string array with each comma-separated element as a separate entry.
     * 
     * @throws java.io.IOException if bad things happen during the read
     */
    public String[] readNext() throws IOException {

        String[] result = null;
        do {
            String nextLine = getNextLine();
            if (!hasNext) {
                return result; // should throw if still pending?
            }
            String[] r = parser.parseLineMulti(nextLine);
            if (r.length > 0) {
                if (result == null) {
                    result = r;
                } else {
                    String[] t = new String[result.length + r.length];
                    System.arraycopy(result, 0, t, 0, result.length);
                    System.arraycopy(r, 0, t, result.length, r.length);
                    result = t;
                }
            }
        } while (parser.isPending());
        return result;
    }

    /**
     * Reads the next line from the file.
     * 
     * @return the next line from the file without trailing newline
     * @throws java.io.IOException if bad things happen during the read
     */
    private String getNextLine() throws IOException {
        if (!this.linesSkipped) {
            for (int i = 0; i < skipLines; i++) {
                bufferedReader.readLine();
            }
            this.linesSkipped = true;
        }
        String nextLine = bufferedReader.readLine();
        if (nextLine == null) {
            hasNext = false;
        }
        return hasNext ? nextLine : null;
    }

    @Override
    public void close() throws IOException {
        bufferedReader.close();
    }

}
