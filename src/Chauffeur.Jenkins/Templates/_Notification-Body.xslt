<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:fo="http://www.w3.org/1999/XSL/Format"
                xmlns:m="http://schemas.datacontract.org/2004/07/Chauffeur.Jenkins.Model">
  
  <xsl:output method="html" omit-xml-declaration="yes" encoding="UTF-8" indent="yes" />
  <xsl:template match="/package">    
    <html>
      <head>
        <style type="text/css">
          table a:link {
          color: #666;
          font-weight: bold;
          text-decoration: none;
          }
          table a:visited {
          color: #999999;
          font-weight: bold;
          text-decoration: none;
          }
          table a:active,
          table a:hover {
          color: #bd5a35;
          text-decoration: underline;
          }
          table {
          font-family: Arial, Helvetica, sans-serif;
          color: #666;
          font-size: 12px;
          text-shadow: 1px 1px 0px #fff;
          background: #eaebec;
          margin: 20px;
          border: #ccc 1px solid;
          border-radius: 3px;
          box-shadow: 0 1px 2px #d1d1d1;
          }
          table th {
          padding: 21px 25px 22px 25px;
          border-top: 1px solid #fafafa;
          border-botto 1px solid #e0e0e0;
          background: #ededed;
          }
          table th:first-child {
          text-align: left;
          padding-left: 20px;
          }
          table tr:first-child th:first-child {
          border-top-left-radius: 3px;
          }
          table tr:first-child th:last-child {
          border-top-right-radius: 3px;
          }
          table tr {
          text-align: center;
          padding-left: 20px;
          }
          table td:first-child {
          text-align: left;
          padding-left: 20px;
          border-left: 0;
          }
          table td {
          padding: 18px;
          border-top: 1px solid #ffffff;
          border-botto 1px solid #e0e0e0;
          border-left: 1px solid #e0e0e0;
          background: #fafafa;
          }
          table tr.even td {
          background: #f6f6f6;
          }
          table tr:last-child td {
          border-botto 0;
          }
          table tr:last-child td:first-child {
          border-bottom-left-radius: 3px;
          }
          table tr:last-child td:last-child {
          border-bottom-right-radius: 3px;
          }
        </style>
      </head>
      <body>

        <table>
          <thead>
            <th>Job</th>
            <th>Date</th>
            <th>Number</th>
            <th>Result</th>
          </thead>
          <tbody>
            <tr>
              <td>
                <xsl:value-of select="job"/>
              </td>
              <td>
                <xsl:value-of select="date"/>
              </td>
              <td>
                <xsl:value-of select="build/id"/>
              </td>
              <td>
                <xsl:value-of select="build/result"/>
              </td>
            </tr>
          </tbody>
        </table>


        <table>         
          <thead>
            <th>Changes</th>          
          </thead>
          <tbody>
            <tr>
              <td>
                <xsl:value-of select="job"/>
              </td>
              <td>
                <xsl:value-of select="date"/>
              </td>
              <td>
                <xsl:value-of select="build/id"/>
              </td>
              <td>
                <xsl:value-of select="build/result"/>
              </td>
            </tr>
          </tbody>
        </table>
      </body>
    </html>
  </xsl:template>  
</xsl:stylesheet>
