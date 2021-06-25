package com.idera.sqlcm.utils;

import java.io.IOException;
import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;

/**
 * CmCsvParser class, this class support multiple separators.
 * 
 */
public class CmCsvParser {

    /** Semi-colon. */
    private static final char SEMI_COLON = ';';

    /** Comma. */
    private static final char COMMA = ',';

    /** Separators. */
    private List<Character> separators;

    /** Quote char. */
    private char quotechar;

    /** Escape. */
    private char escape;

    /** Strict quotes. */
    private boolean strictQuotes;

    /** Pending. */
    private String pending;

    private boolean inField = false;

    private final boolean ignoreLeadingWhiteSpace;

    /** The default separators to use if none is supplied to the constructor. */
    public static final List<Character> DEFAULT_SEPARATORS = new LinkedList<Character>();

    /** Initial read size. */
    public static final int INITIAL_READ_SIZE = 128;

    /**
     * The default quote character to use if none is supplied to the constructor.
     */
    public static final char DEFAULT_QUOTE_CHARACTER = '"';

    /**
     * The default escape character to use if none is supplied to the constructor.
     */
    public static final char DEFAULT_ESCAPE_CHARACTER = '\\';

    /**
     * The default strict quote behavior to use if none is supplied to the constructor
     */
    public static final boolean DEFAULT_STRICT_QUOTES = false;

    /**
     * The default leading whitespace behavior to use if none is supplied to the constructor
     */
    public static final boolean DEFAULT_IGNORE_LEADING_WHITESPACE = true;

    /**
     * This is the "null" character - if a value is set to this then it is ignored. I.E. if the
     * quote character is set to null then there is no quote character.
     */
    public static final char NULL_CHARACTER = '\0';

    /**
     * Initialize default separators.
     * 
     */
    static {
        DEFAULT_SEPARATORS.add(COMMA);
        DEFAULT_SEPARATORS.add(SEMI_COLON);
    }

    /**
     * Constructs CmCsvParser using a comma and semi-colon for the separators.
     */
    public CmCsvParser() {
        this(DEFAULT_SEPARATORS, DEFAULT_QUOTE_CHARACTER, DEFAULT_ESCAPE_CHARACTER);
    }

    /**
     * Constructs CmCsvParser with supplied separator.
     *
     * @param separator the delimiter to use for separating entries.
     */
    public CmCsvParser(List<Character> separators) {
        this(separators, DEFAULT_QUOTE_CHARACTER, DEFAULT_ESCAPE_CHARACTER);
    }

    /**
     * Constructs CmCsvParser with supplied separator and quote char.
     *
     * @param separator the delimiter to use for separating entries
     * @param quotechar the character to use for quoted elements
     */
    public CmCsvParser(List<Character> separators, char quotechar) {
        this(separators, quotechar, DEFAULT_ESCAPE_CHARACTER);
    }

    /**
     * Constructs CmCsvParser with supplied separator and quote char.
     *
     * @param separator the delimiter to use for separating entries
     * @param quotechar the character to use for quoted elements
     * @param escape the character to use for escaping a separator or quote
     */
    public CmCsvParser(List<Character> separators, char quotechar, char escape) {
        this(separators, quotechar, escape, DEFAULT_STRICT_QUOTES);
    }

    /**
     * Constructs CmCsvParser with supplied separator and quote char. Allows setting the
     * "strict quotes" flag
     *
     * @param separator the delimiter to use for separating entries
     * @param quotechar the character to use for quoted elements
     * @param escape the character to use for escaping a separator or quote
     * @param strictQuotes if true, characters outside the quotes are ignored
     */
    public CmCsvParser(List<Character> separators, char quotechar, char escape,
                       boolean strictQuotes) {
        this(separators, quotechar, escape, strictQuotes, DEFAULT_IGNORE_LEADING_WHITESPACE);
    }

    /**
     * Constructs CmCsvParser with supplied separator and quote char. Allows setting the
     * "strict quotes" and "ignore leading whitespace" flags
     *
     * @param separator the delimiter to use for separating entries
     * @param quotechar the character to use for quoted elements
     * @param escape the character to use for escaping a separator or quote
     * @param strictQuotes if true, characters outside the quotes are ignored
     * @param ignoreLeadingWhiteSpace if true, white space in front of a quote in a field is ignored
     */
    public CmCsvParser(List<Character> separators, final char quotechar, final char escape,
                       final boolean strictQuotes, boolean ignoreLeadingWhiteSpace) {
        if (anyCharactersAreTheSame(separators, quotechar, escape)) {
            throw new UnsupportedOperationException(
                                                    "The separator, quote, and escape characters must be different!");
        }

        if (separators.contains(NULL_CHARACTER)) {
            throw new UnsupportedOperationException("The separator character must be defined!");
        }

        this.separators = separators;
        this.quotechar = quotechar;
        this.escape = escape;
        this.strictQuotes = strictQuotes;
        this.ignoreLeadingWhiteSpace = ignoreLeadingWhiteSpace;
    }

    /**
     * Verify if some character is the same.
     * 
     * @param separators the list of separators
     * @param quotechar the quote char
     * @param escape the escape char
     * 
     * @return
     */
    private boolean anyCharactersAreTheSame(List<Character> separators, char quotechar, char escape) {
        return separators.contains(quotechar) || separators.contains(escape)
                || isSameCharacter(quotechar, escape);
    }

    /**
     * Verify if are the same characters.
     * 
     * @param c1 character 01
     * @param c2 character 02
     * 
     * @return true is are the same
     */
    private boolean isSameCharacter(char c1, char c2) {
        return c1 != NULL_CHARACTER && c1 == c2;
    }

    /**
     * 
     * @return true if something was left over from last call(s)
     */
    public boolean isPending() {
        return this.pending != null;
    }

    public String[] parseLineMulti(String nextLine) throws IOException {
        return parseLine(nextLine, true);
    }

    public String[] parseLine(String nextLine) throws IOException {
        return parseLine(nextLine, false);
    }

    /**
     * Parses an incoming String and returns an array of elements.
     * 
     * @param nextLine the string to parse
     * @param multi
     * @return the comma tokenized list of elements, or null if nextLine is null
     * @throws java.io.IOException if bad things happen during the read
     */
    protected String[] parseLine(String nextLine, boolean multi) throws IOException {

        if (!multi && pending != null) {
            pending = null;
        }

        if (nextLine == null) {
            if (pending != null) {
                String s = pending;
                pending = null;
                return new String[] {s};
            } else {
                return null;
            }
        }

        List<String> tokensOnThisLine = new ArrayList<String>();
        StringBuilder sb = new StringBuilder(INITIAL_READ_SIZE);
        boolean inQuotes = false;
        if (pending != null) {
            sb.append(pending);
            pending = null;
            inQuotes = true;
        }
        for (int i = 0; i < nextLine.length(); i++) {

            char c = nextLine.charAt(i);
            if (c == this.escape) {
                if (isNextCharacterEscapable(nextLine, inQuotes || inField, i)) {
                    sb.append(nextLine.charAt(i + 1));
                    i++;
                }
            } else if (c == quotechar) {
                if (isNextCharacterEscapedQuote(nextLine, inQuotes || inField, i)) {
                    sb.append(nextLine.charAt(i + 1));
                    i++;
                } else {
                    // inQuotes = !inQuotes;

                    // the tricky case of an embedded quote in the middle: a,bc"d"ef,g

                    if (!strictQuotes) {
                        if (i > 2 // not on the beginning of the line
                                && !this.separators.contains(nextLine.charAt(i - 1)) // not at the
                                                                                     // beginning
                                // of an escape sequence
                                && nextLine.length() > (i + 1)
                                && !this.separators.contains(nextLine.charAt(i + 1)) // not at the
                                                                                     // end of an
                        // escape sequence
                        ) {

                            if (ignoreLeadingWhiteSpace && sb.length() > 0 && isAllWhiteSpace(sb)) {
                                sb.setLength(0); // discard white space leading up to quote
                            } else {
                                sb.append(c);
                                // continue;
                            }

                        }
                    }

                    inQuotes = !inQuotes;
                }
                inField = !inField;
            } else if (this.separators.contains(c) && !inQuotes) {
                tokensOnThisLine.add(sb.toString());
                sb.setLength(0); // start work on next token
                inField = false;
            } else {
                if (!strictQuotes || inQuotes) {
                    sb.append(c);
                    inField = true;
                }
            }
        }
        // line is done - check status
        if (inQuotes) {
            if (multi) {
                // continuing a quoted section, re-append newline
                sb.append("\n");
                pending = sb.toString();
                sb = null; // this partial content is not to be added to field list yet
            } else {
                throw new IOException("Un-terminated quoted field at end of CSV line");
            }
        }
        if (sb != null) {
            tokensOnThisLine.add(sb.toString());
        }
        return tokensOnThisLine.toArray(new String[tokensOnThisLine.size()]);

    }

    /**
     * precondition: the current character is a quote or an escape
     * 
     * @param nextLine the current line
     * @param inQuotes true if the current context is quoted
     * @param i current index in line
     * @return true if the following character is a quote
     */
    private boolean isNextCharacterEscapedQuote(String nextLine, boolean inQuotes, int i) {
        return inQuotes // we are in quotes, therefore there can be escaped quotes in here.
                && nextLine.length() > (i + 1) // there is indeed another character to check.
                && nextLine.charAt(i + 1) == quotechar;
    }

    /**
     * precondition: the current character is an escape
     * 
     * @param nextLine the current line
     * @param inQuotes true if the current context is quoted
     * @param i current index in line
     * @return true if the following character is a quote
     */
    private boolean isNextCharacterEscapable(String nextLine, boolean inQuotes, int i) {
        return inQuotes // we are in quotes, therefore there can be escaped quotes in here.
                && nextLine.length() > (i + 1) // there is indeed another character to check.
                && (nextLine.charAt(i + 1) == quotechar || nextLine.charAt(i + 1) == this.escape);
    }

    /**
     * precondition: sb.length() > 0
     *
     * @param sb A sequence of characters to examine
     * @return true if every character in the sequence is whitespace
     */
    protected boolean isAllWhiteSpace(CharSequence sb) {
        boolean result = true;
        for (int i = 0; i < sb.length(); i++) {
            char c = sb.charAt(i);

            if (!Character.isWhitespace(c)) {
                return false;
            }
        }
        return result;
    }
}
