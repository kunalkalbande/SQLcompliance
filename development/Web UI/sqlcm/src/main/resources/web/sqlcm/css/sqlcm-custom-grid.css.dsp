.sqlcm-grid {
    background-image: none;
    background: #C4BAA3;
    -webkit-transition: -webkit-box-shadow 0.2s ease-in;
    -moz-transition: -moz-box-shadow 0.2s ease-in;
    transition: box-shadow 0.2s ease-in;
    overflow-x: auto;
    border: 1px solid #cfcfcf !important;
}

.sqlcm-grid:focus,
.sqlcm-grid:active {
    box-shadow: 0 3px 6px 0 #969696;
    -webkit-box-shadow: 0 3px 6px 0 #969696;
    -moz-box-shadow: 0 3px 6px 0 #969696;
}

.sqlcm-grid div.z-header-bg {
    margin-right: 0;
}

.sqlcm-grid div.z-grid-body .z-cell {
    padding: 0;
}

.sqlcm-grid div.z-grid-header th.z-column div.z-footer-cnt,
.sqlcm-grid div.z-grid-header div.z-cell-cnt,
.sqlcm-grid div.z-grid-header div.z-column-cnt,
.sqlcm-grid div.z-grid-body div.z-cell-cnt {
    font-size: 1rem;
    padding: 1rem !important;
    text-transform: capitalize;
    text-overflow: ellipsis;
    white-space: nowrap;
    overflow: hidden;
    text-align: left;
}

.sqlcm-grid div.z-grid-header div.z-header-cnt span.z-header-img,
.sqlcm-grid div.z-grid-body .z-cell .z-cell-cnt span.z-rows-img {
    margin-right: 1rem;
    height: 1rem;
    width: 1rem;
}

.sqlcm-grid div.z-grid-body .z-cell .z-cell-cnt img {
    width: 1.25rem;
    height: 1.25rem;
}

.sqlcm-grid div.z-grid-body::-webkit-scrollbar {
    width: 0.75rem;
}

.sqlcm-grid div.z-grid-body::-webkit-scrollbar-track {
    background-color: transparent;
}

.sqlcm-grid div.z-grid-body::-webkit-scrollbar-thumb {
    box-shadow: inset 1px 1px 12px #C4BAA3;
    -webkit-box-shadow: inset 1px 1px 12px #C4BAA3;
    -moz-box-shadow: inset 1px 1px 12px #C4BAA3;
    border-radius: 8px;
    background-color: #C4BAA3;
}

.sqlcm-grid div.z-grid-body:focus::-webkit-scrollbar-thumb {
    box-shadow: inset 1px 1px 12px #00A5DB;
    -webkit-box-shadow: inset 1px 1px 12px #00A5DB;
    -moz-box-shadow: inset 1px 1px 12px #00A5DB;
    background-color: #00A5DB;
    border-radius: 8px;
}

.sqlcm-grid div.z-grid-header tr.z-columns,
.sqlcm-grid div.z-grid-header tr.z-columns.z-column-seld {
    background: #C4BAA3;
    background-image: none;
    height: 50px;
    border: 0;
    width: 100%;
}

.sqlcm-grid div.z-grid-header .z-columns th, .z-grid div.z-grid-body .z-columns th {
    position: relative;
    overflow: hidden;
    white-space: nowrap;
    color: #636363 !important;
    padding: 0;
}

.sqlcm-grid div.z-grid-header tr.z-columns th {
    border: none
}

div.z-grid-header th.z-column,
.sqlcm-grid div.z-grid-header th.z-column {
    border: none;
}
.sqlcm-grid .z-grid-header > table{
    background: #C4BAA3;
}
.sqlcm-grid.z-grid-header > table,
.sqlcm-grid.z-grid-body > table {
    table-layout: initial !important;
}

.sqlcm-grid div.z-grid-header th.z-column:hover,
.sqlcm-grid div.z-grid-header th.z-column:focus,
.sqlcm-grid div.z-grid-header th.z-column.z-column-sort:hover,
.sqlcm-grid div.z-grid-header th.z-column:active {
    background-color: #006089 !important;
    background-image: none;
    border-bottom: none !important;
}

.sqlcm-grid div.z-grid-header th.z-column:hover .z-column-cnt,
.sqlcm-grid div.z-grid-header th.z-columnr:focus .z-column-cnt,
.sqlcm-grid div.z-grid-header th.z-column:active .z-column-cnt {
    color: #FFF;
}

.sqlcm-grid div.z-grid-header div.z-column-cnt img,
.z-column-sort-img {
    float: right;
    width: 0.625rem;
    height: 0.5rem;
    margin-top: 0.375rem;
}

.sqlcm-grid div.z-grid-header th.z-column.z-column-sort-asc .z-column-sort-img {
    background-image: url(data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHN0eWxlPSIgIA0Kd2lkdGg6IDAuNTVyZW07IGhlaWdodDogMC41NXJlbTsgZmlsbDogI0ZGRjsNCiAgYmFja2dyb3VuZDogcmdiYSgwLCAwLCAwLCAwKTsiIA0Kdmlld0JveD0iMCAwIDIgMiI+DQo8cGF0aCBkPSJtMSwwbDEsMi0yLDAiLz4NCjwvc3ZnPg==);
    margin-top: 0.35rem;
    width: 0.55rem;
    height: 0.55rem;
    margin-left: 0;
}

.sqlcm-grid div.z-grid-header th.z-column.z-column-sort-dsc .z-column-sort-img {
    background-image: url(data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHN0eWxlPSIgIA0Kd2lkdGg6IDAuNTVyZW07IGhlaWdodDogMC41NXJlbTsgZmlsbDogI0ZGRjsNCiAgYmFja2dyb3VuZDogcmdiYSgwLCAwLCAwLCAwKTsiIA0Kdmlld0JveD0iMCAwIDIgMiI+DQo8cGF0aCBkPSJNMCAwIEwyIDAgTDEgMloiLz4NCjwvc3ZnPg==);
    width: 0.55rem;
    height: 0.55rem;
    margin-top: 0.35rem;
    margin-left: 0;
    background-position: 0 0;
}
.sqlcm-grid .z-column-sort-img {
    position: static;
}

.sqlcm-grid div.z-grid-body {
    -webkit-box-sizing: border-box;
    -moz-box-sizing: border-box;
    box-sizing: border-box;
    border: 1px solid rgba(0, 0, 0, 0);
    overflow: hidden;
    display: block;
    overflow-y: auto;
}

.sqlcm-grid div.z-grid-body:focus,
.sqlcm-grid div.z-grid-body:active {
    -webkit-box-sizing: border-box;
    -moz-box-sizing: border-box;
    box-sizing: border-box;
    border: 1px solid #00A5DB;
    display: block;
}

div.styled-list .z-grid-body .z-rows tr.z-row {
    background-color: #FFF !important;
}

.sqlcm-grid div.z-grid-body > table > tbody > tr.z-row td {
    border-bottom:  1px solid #D9D4D0;
    border-left: none;
}

.sqlcm-grid tr.z-row td.z-row-inner,
.sqlcm-grid tr.z-row td.z-cell,
.sqlcm-grid tr.z-group td.z-group-inner,
.sqlcm-grid tr.z-groupfoot td.z-groupfoot-inner {
    padding-right: 1rem !important;
    padding-left: 1rem !important;
    cursor: pointer;
    font-size: 16px;
    font-weight: normal;
    overflow: hidden;
}

.sqlcm-grid div.z-grid-body > table > tbody > tr.z-row.z-row-seld {
    background-image: none;
    background: #66A0B8;
}

.sqlcm-grid div.z-grid-body > table > tbody > tr.z-row.z-row-over {
    background-image: none;
}

.sqlcm-grid tr.z-row-over > td.z-row-inner {
    background-image: none;
    border-top: 0;
}

.sqlcm-grid tr.z-grid-odd,
.sqlcm-grid div.z-grid-body > table > tbody > tr.z-grid-odd td.z-row-inner{
    background: transparent;
}

.sqlcm-grid div.z-grid-body > table > tbody > tr.z-row.z-row-seld div.z-cell-cnt {
    color: #FFF;
}

.sqlcm-grid div.z-grid-header,
.sqlcm-grid div.z-grid-header tr,
.sqlcm-grid div.z-grid-footer {
    width: 100% !important;
}

.sqlcm-grid .grid-paging.z-paging .z-paging-btn {
    background-color: transparent;
}

.sqlcm-grid .grid-paging.z-paging .z-paging-btn button {
    background-position: center center;
    background-size: 1rem 1rem;
}

.sqlcm-grid .grid-paging.z-paging .z-paging-btn button:focus,
.sqlcm-grid .grid-paging.z-paging .z-paging-btn button:active {
    color: #FFFFFF;
    border: none;
    background-color: #006089;
    box-shadow: 0 3px 6px 0 #969696;
    -webkit-box-shadow: 0 3px 6px 0 #969696;
    -moz-box-shadow: 0 3px 6px 0 #969696;
}

.sqlcm-grid .grid-paging.z-paging .z-paging-btn-disd button:focus,
.sqlcm-grid .grid-paging.z-paging .z-paging-btn-disd button:active {
    background-color: transparent;
    box-shadow: none;
    -webkit-box-shadow: none;
    -moz-box-shadow: none;
}

.sqlcm-grid .grid-dropdown.z-menupopup {
    padding: 0;
    border: none;
}

.sqlcm-grid .grid-dropdown.z-menupopup ul.z-menupopup-cnt {
    background-image: none;
    background: #C4BAA3;
    padding: 0;
}

.sqlcm-grid .grid-dropdown.z-menupopup ul.z-menupopup-cnt li.z-menuitem {
    background: #C4BAA3;
    padding: 0.25rem;
}

.sqlcm-grid .grid-dropdown.z-menupopup ul.z-menupopup-cnt li.z-menuitem.z-menuitem-over,
.sqlcm-grid .grid-dropdown.z-menupopup ul.z-menupopup-cnt li.z-menuitem.z-menuitem-over a.z-menuitem-cnt {
    background-image: none;
    background: #006089;
    color: #FFF;
}

.sqlcm-grid .grid-dropdown.z-menupopup ul.z-menupopup-cnt li.z-menuitem-over div.z-menuitem-cl,
.sqlcm-grid .grid-dropdown.z-menupopup ul.z-menupopup-cnt li.z-menuitem-over div.z-menuitem-cr {
    background-image: none;
}

.sqlcm-grid .z-column-cnt .z-column-img,
.sqlcm-grid .z-menupopup-cnt .z-menuitem-cnt-unck .z-menuitem-img,
.sqlcm-grid div.z-grid-body .z-row-img-checkbox,
.sqlcm-grid div.z-grid-body .z-row-seld > td > .z-cell-cnt > .z-row-img-checkbox,
.sqlcm-grid .z-column-cnt .z-column-img.z-column-img-seld,
.sqlcm-grid .z-column-cnt .z-column-img.grid-indeterminate,
.sqlcm-grid .z-menupopup-cnt a.z-menuitem-cnt-ck .z-menuitem-img {
    display: inline-block;
    background-position: left center;
    background-repeat: no-repeat;
    background-size: 0.90rem 0.90rem;
    width: 1rem;
    height: 1rem;
    -webkit-box-sizing: border-box;
    -moz-box-sizing: border-box;
    box-sizing: border-box;
    border: 1px solid #483E2F;
    background-image: none;
    background-color: #FFF;
}

.sqlcm-grid .grid-dropdown.z-menupopup .z-menupopup-cnt .z-menuitem a.z-menuitem-cnt {
    background: #C4BAA3;
    color: #000;
    padding: 0.125rem 1rem 0.125rem 0.5rem;
}

.sqlcm-grid .ccl-grid-column-active {
    background-color: #C4BAA3;
    color: #636363;
}

.sqlcm-grid .ccl-grid-column-inactive {
    background-color: #006089;
    color: white;
}

.sqlcm-grid .z-column-cnt .z-column-img,
.sqlcm-grid div.z-grid-body .z-row-img-checkbox {
    background-image: none;
    background-color: #FFF;
}

.sqlcm-grid div.z-grid-body .z-row-seld > td > .z-cell-cnt > .z-row-img-checkbox,
.sqlcm-grid .z-column-cnt .z-column-img.z-column-img-seld,
.sqlcm-grid .z-column-cnt .z-column-img.grid-indeterminate.z-column-img-seld,
.sqlcm-grid .z-menupopup-cnt a.z-menuitem-cnt-ck .z-menuitem-img {
    background-color: #FFFF;
    background-image: url(data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz4KPHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHN0eWxlPSJiYWNrZ3JvdW5kOiAjZmZmOyIgdmVyc2lvbj0iMSIgdmlld0JveD0iMCAwIDMxMCAzMTAiPgogIDxwb2x5Z29uIGZpbGw9IiM0ODNFMkYiIHBvaW50cz0iOTMgMjc3IDAgMTg0IDM5IDE0NSAxMDkgMjE1IDI2OCAzMyAzMTAgNjkgMTI3IDI3NyAiLz4KPC9zdmc+Cg==);
}

.sqlcm-grid .z-column-cnt .z-column-img.grid-indeterminate {
    background-color: #FFFF;
    background-image: url('data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz4KPHN2ZyB2ZXJzaW9uPSIxIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyODAgMjgwIj4KPHBhdGggZmlsbD0iI0UxRENEMSIgZD0iTTAsMHYyODBoMjgwVjBIMHogTTIyNiwyMjVoLTE3MHYtMTcwaDE3MFYyMjV6Ii8+Cjwvc3ZnPgo=');
}

.idera-grid-footer {
    background: #C4BAA3!important;
    width: 100% !important;
    -webkit-box-sizing: border-box !important;
    -moz-box-sizing: border-box;
    box-sizing: border-box;
}

.idera-grid-footer > .z-hlayout {
    padding: 0.5rem;
    vertical-align: center;
}

.idera-grid-footer > .z-hlayout > .z-hlayout-inner {
    margin: 0.5rem;
}

.idera-grid-footer > .z-hlayout > .z-hlayout-inner:last-child {
    float: right;
    margin: 0;
}

.idera-grid-footer > .z-hlayout .z-hlayout-inner span.z-label,
.grid-page-size.z-spinner input.z-spinner-inp,
.grid-paging.z-paging .z-paging-text {
    font-size: 1rem;
    font-family: ${fontFamilyC};
}

.grid-page-size.z-spinner input.z-spinner-inp,
.grid-paging.z-paging table tbody tr td input.z-paging-inp {
    width: 1rem;
    outline: none;
    border: none;
    font-size: 1rem;
}

.grid-paging.z-paging {
    border: none;
    background: #C4BAA3;
}

.grid-page-size.z-spinner input.z-spinner-inp + i.z-spinner-btn {
    border: none;
}

.idera-grid-footer .z-paging.grid-paging {
    margin-right: 2rem;
}

.idera-grid-footer .z-spinner-inp {
    height: inherit;
    padding: 0 2px;
    line-height: 1.2;
}

div.idera-grid.z-listbox-header div.z-auxheader-cnt {
    background-color: #C4BAA3;
}

/* filter style */

.idera-grid div.z-listbox-body .grid-group-header {
    background: none;
    font-weight: bold;
}

.idera-grid div.z-listbox-header tr.z-auxhead {
    background-image: none;
}

.idera-grid .foodFooter {
    background-color: #FFF;
    font-weight: bold;
    padding-left: 1rem;
}

.idera-grid div.z-listbox-body .z-listgroup-seld {
    background: #66A0B8;
}

.idera-grid div.z-listbox-body .z-listgroup-seld .z-listcell-cnt {
    color: #FFF;
}

.idera-grid div.z-listbox-body .grid-group-header .z-listgroup-img-checkbox {
    border: 1px solid #483E2F;
    background: none;
}

.idera-grid div.z-listbox-body .z-listgroup-seld .z-listgroup-img-checkbox {
    background-image: url(data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz4KPHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHN0eWxlPSJiYWNrZ3JvdW5kOiAjZmZmOyIgdmVyc2lvbj0iMSIgdmlld0JveD0iMCAwIDMxMCAzMTAiPgogIDxwb2x5Z29uIGZpbGw9IiM0ODNFMkYiIHBvaW50cz0iOTMgMjc3IDAgMTg0IDM5IDE0NSAxMDkgMjE1IDI2OCAzMyAzMTAgNjkgMTI3IDI3NyAiLz4KPC9zdmc+Cg==);
}


.idera-button-primary-focus{
    color: #FFFFFF !important;
    border: none;
    background-color: #006089;
    box-shadow: 0 3px 6px 0 #969696;
    -webkit-box-shadow: 0 3px 6px 0 #969696;
    -moz-box-shadow: 0 3px 6px 0 #969696;

        text-transform: lowercase;
        font-size: 1rem;
        font-family: ${fontFamilyC};
        height: 2rem;
        font-weight: 600;
        padding-left: 1rem;
        padding-right: 1rem;
        margin: 0.625rem;
        vertical-align: middle;
        cursor: pointer;
}
.idera-button-primary-focus .z-button,
.idera-button-primary-focus tr td {
    color: #FFFFFF !important;
    }

html {
	overflow-x: auto !important;
}