<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:fo="http://www.w3.org/1999/XSL/Format"
                xmlns:m="http://schemas.datacontract.org/2004/07/Chauffeur.Jenkins.Model">
  <xsl:output method="html" omit-xml-declaration="yes" encoding="UTF-8" indent="yes" />
  <xsl:template match="/m:package">
    <html>
      <head>
        <style type="text/css">
          body {
          font-family: 'Helvetica Neue', Helvetica, Arial;
          font-size: 14px;
          line-height: 20px;
          font-weight: 400;
          }

          table {
          width: 100%;
          display: table;
          border-collapse: collapse;
          }

          table, td, th {
          white-space: nowrap;
          border: 1px solid #a9a9a9;
          }

          tbody {
          background: #ffffff;
          padding: 6px;
          }

          td {
          padding: 6px 12px;
          }

          th {
          display: table-cell;
          vertical-align: middle;
          background: #27ae60;
          padding: 6px 12px;
          text-align: right;
          color: #ffffff;
          }

          tr:nth-child(even) {
          background: #f6f6f6;
          }

          caption {
          vertical-align: middle;
          color: #27ae60;
          font-weight: bold;
          padding: 6px 12px;
          font-size: 18px
          }

          span {
          font-size: 8px;
          text-transform: uppercase;
          color:dimgray;
          }
        </style>
      </head>
      <body>
        <div>
          <table id="table">
            <caption>

            </caption>
            <tbody>
              <tr>
                <th>Job:</th>
                <td>
                  <xsl:value-of select="m:job"/>
                </td>
              </tr>
              <tr>
                <th>Date:</th>
                <td>
                  <xsl:value-of select="m:date"/>
                </td>
              </tr>
              <tr>
                <th>Build:</th>
                <td>
                  <xsl:value-of select="m:build/m:id"/>
                </td>
              </tr>
              <tr>
                <th>Name:</th>
                <td>
                  <xsl:value-of select="m:build/m:fullDisplayName"/>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
