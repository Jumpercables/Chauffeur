<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" omit-xml-declaration="yes" encoding="UTF-8" indent="yes" />
  <xsl:template match="/package">
    <html>
      <head>
        <style type="text/css">
          body {
          font-family: Calibri, Helvetica, sans-serif;
          }          
        </style>
      </head>
      <body>
        <div class="header">
          <p>
            Developers and Testers,
          </p>
          <p>
            Build <span>
              <xsl:value-of select="build/number"/>
            </span> of the Sempra Desktop Installer has been installed on the <xsl:value-of select="machine"/>
          </p>

          <div class="changesets">
            <ul>
              <xsl:for-each select="build/changeSet/items/changeSetItem">
                <xsl:sort select="date" data-type="number"/>
                <li>
                  <span>
                    <xsl:value-of select="comment"/>
                  </span>
                </li>
              </xsl:for-each>
            </ul>
          </div>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
